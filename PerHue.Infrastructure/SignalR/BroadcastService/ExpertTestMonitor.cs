using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.AI;
using PerHue.Infrastructure.Services;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.SignalR.BroadcastService
{
	public class ExpertTestMonitor : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<ExpertTestMonitor> _logger;
		private readonly IConfiguration _configuration;

		private readonly int _maxRetries;
		private readonly int _requiredResponses;
		private readonly int _daysToWait;
		private readonly decimal _ratingDeduction;
		private readonly decimal _ratingWarningThreshold;

		public ExpertTestMonitor(
			IServiceScopeFactory scopeFactory,
			ILogger<ExpertTestMonitor> logger,
			IConfiguration configuration)
		{
			_scopeFactory = scopeFactory;
			_logger = logger;
			_configuration = configuration;

			_maxRetries = _configuration.GetValue<int>("ExpertTestSettings:MaxRetries");
			_requiredResponses = _configuration.GetValue<int>("ExpertTestSettings:RequiredResponses");
			_daysToWait = _configuration.GetValue<int>("ExpertTestSettings:DaysToWait");
			_ratingDeduction = _configuration.GetValue<decimal>("ExpertTestSettings:RatingDeduction");
			_ratingWarningThreshold = _configuration.GetValue<decimal>("ExpertTestSettings:RatingWarningThreshold");
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation($"Expert Test Monitor started.");

			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation("Expert Test Monitor is running a check.");

				using (var scope = _scopeFactory.CreateScope())
				{
					var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
					var aiAnalysisService = scope.ServiceProvider.GetRequiredService<IAIImageAnalysisService>();
					var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

					try
					{
						// 1. Handle Normal Pending Requests
						var pendingTestRequests = await unitOfWork.TestRequestRepository.GetPendingRequestsAsync();
						foreach (var testRequest in pendingTestRequests)
						{
							await HandleRequestRetriesAndFinalization(unitOfWork, aiAnalysisService, emailService, testRequest);
						}

						// 2. [ADDED] Handle Reviewing Requests
						var reviewingTestRequests = await unitOfWork.TestRequestRepository.FindAsync(t => t.Status == TestRequestStatus.Reviewing.ToString());
						foreach (var testRequest in reviewingTestRequests)
						{
							// Updated to pass aiAnalysisService
							await HandleReviewRetries(unitOfWork, emailService, aiAnalysisService, testRequest);
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error occurred in Expert Test Monitor.");
					}
				}

				// Check every 24 hours
				await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
			}

			_logger.LogInformation("Expert Test Monitor is stopping.");
		}

		// [ADDED] Logic for handling Review Expiration
		private async Task HandleReviewRetries(
			IUnitOfWork unitOfWork,
			EmailService emailService,
			IAIImageAnalysisService aiService, // Added Service
			TestRequest testRequest)
		{
			var expertRequests = await unitOfWork.ExpertTestRequestRepository.GetRequestsByTestIdAsync(testRequest.Id);

			// Filter for Pending Reviews
			var pendingReviews = expertRequests.Where(etr => etr.Status == ExpertTestRequestStatus.PendingReview.ToString()).ToList();
			var expiredReviewsCount = expertRequests.Count(etr => etr.Status == ExpertTestRequestStatus.ReviewExpired.ToString());

			bool needsNewReviewer = false;
			bool changesMade = false;

			foreach (var req in pendingReviews)
			{
				var deadline = req.CreatedDate.AddDays(_daysToWait);
				var timeRemaining = deadline - DateTime.UtcNow;

				if (timeRemaining.TotalSeconds <= 0)
				{
					_logger.LogInformation($"Review Request for Test {testRequest.Id}, Expert {req.ExpertId} has expired.");

					// A. Mark as ReviewExpired
					req.Status = ExpertTestRequestStatus.ReviewExpired.ToString();
					await unitOfWork.ExpertTestRequestRepository.UpdateAsync(req);
					changesMade = true;

					// B. Apply Penalty (Reused logic)
					await ApplyExpertPenalty(unitOfWork, emailService, req.ExpertId, testRequest.Id, _ratingDeduction);

					expiredReviewsCount++;
					needsNewReviewer = true;
				}
				else if (timeRemaining.TotalHours <= 24)
				{
					// Send Warning Notification
					await SendDeadlineWarning(unitOfWork, req.ExpertId, testRequest.Id);
				}
			}

			// C. Reassignment Logic for Review
			if (needsNewReviewer && expiredReviewsCount <= _maxRetries)
			{
				var assignedExpertIds = expertRequests.Select(etr => etr.ExpertId).ToList();
				var allExperts = await unitOfWork.ExpertRepository.GetAllAsync();

				// Find expert who is NOT already assigned to this test (neither analysis nor review)
				var newExpert = allExperts.FirstOrDefault(e => !assignedExpertIds.Contains(e.Id) && e.Id != testRequest.UserAccountId);

				if (newExpert != null)
				{
					_logger.LogInformation($"Reassigning Review for Test {testRequest.Id} to Expert {newExpert.Id}.");
					var newReviewRequest = new ExpertTestRequest
					{
						ExpertId = newExpert.Id,
						TestRequestId = testRequest.Id,
						Status = ExpertTestRequestStatus.PendingReview.ToString(),
						CreatedDate = DateTime.Now
					};
					await unitOfWork.ExpertTestRequestRepository.CreateAsync(newReviewRequest);

					var notification = new Notification
					{
						Title = $"New Review Request #{testRequest.Id}",
						Content = "You have been selected to review a color analysis. Please respond within 2 days.",
						Receiver = newExpert.Id,
						TestRequestId = testRequest.Id,
						ReceivedTime = DateTime.Now,
						IsRead = false,
						Type = "ReviewRequest"
					};
					await unitOfWork.NotificationRepository.CreateAsync(notification);
					changesMade = true;
				}
				else
				{
					_logger.LogWarning($"Review TestRequest {testRequest.Id}: No new experts available for retry.");
				}
			}
			// D. Fallback Logic: If max retries exceeded for reviews, let AI handle it
			else if (needsNewReviewer && expiredReviewsCount > _maxRetries)
			{
				await PerformReviewFallbackAsync(unitOfWork, aiService, testRequest);
				return; // Exit as the request is finalized
			}

			if (changesMade)
			{
				await unitOfWork.SaveChangesWithTransactionAsync();
			}
		}

		private async Task HandleRequestRetriesAndFinalization(
			IUnitOfWork unitOfWork,
			IAIImageAnalysisService aiService,
			EmailService emailService,
			TestRequest testRequest)
		{
			var expertRequests = await unitOfWork.ExpertTestRequestRepository.GetRequestsByTestIdAsync(testRequest.Id);
			var completedResponses = await unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequest.Id);
			var pending = expertRequests.Where(etr => etr.Status == ExpertTestRequestStatus.Pending.ToString()).ToList();
			var expiredCount = expertRequests.Count(etr => etr.Status == ExpertTestRequestStatus.Expired.ToString());

			bool needsNewExpert = false;
			bool changesMade = false;

			// 1. Check for expired pending requests AND Upcoming Deadlines
			foreach (var req in pending)
			{
				var deadline = req.CreatedDate.AddDays(_daysToWait);
				var timeRemaining = deadline - DateTime.UtcNow;

				if (timeRemaining.TotalSeconds <= 0)
				{
					_logger.LogInformation($"TestRequest {testRequest.Id} for Expert {req.ExpertId} has expired.");

					// Mark as Expired
					req.Status = ExpertTestRequestStatus.Expired.ToString();
					await unitOfWork.ExpertTestRequestRepository.UpdateAsync(req);
					changesMade = true;

					// Penalize Expert
					await ApplyExpertPenalty(unitOfWork, emailService, req.ExpertId, testRequest.Id, _ratingDeduction);

					expiredCount++;
					needsNewExpert = true;
				}
				else if (timeRemaining.TotalHours <= 24)
				{
					await SendDeadlineWarning(unitOfWork, req.ExpertId, testRequest.Id);
				}
			}

			// 2. Handle retries using _maxRetries
			if (needsNewExpert && expiredCount <= _maxRetries)
			{
				var assignedExpertIds = expertRequests.Select(etr => etr.ExpertId).ToList();
				var allExperts = await unitOfWork.ExpertRepository.GetAllAsync();

				var newExpert = allExperts.FirstOrDefault(e => !assignedExpertIds.Contains(e.Id) && e.Id != testRequest.UserAccountId);

				if (newExpert != null)
				{
					_logger.LogInformation($"Retrying TestRequest {testRequest.Id}: assigning to new Expert {newExpert.Id}.");
					var newExpertRequest = new ExpertTestRequest
					{
						ExpertId = newExpert.Id,
						TestRequestId = testRequest.Id,
						Status = ExpertTestRequestStatus.Pending.ToString(),
						CreatedDate = DateTime.Now
					};
					await unitOfWork.ExpertTestRequestRepository.CreateAsync(newExpertRequest);

					var notification = new Notification
					{
						Title = $"New Test Request #{testRequest.Id}",
						Content = "You need to respond within 2 days from the time you receive the request.",
						Receiver = newExpert.Id,
						TestRequestId = testRequest.Id,
						ReceivedTime = DateTime.Now,
						IsRead = false,
						Type = "TestRequest"
					};
					await unitOfWork.NotificationRepository.CreateAsync(notification);

					await unitOfWork.SaveChangesWithTransactionAsync();
					return;
				}
			}

			// 3. Fallback Trigger
			var currentExpertRequests = await unitOfWork.ExpertTestRequestRepository.GetRequestsByTestIdAsync(testRequest.Id);
			var pendingCount = currentExpertRequests.Count(etr => etr.Status == ExpertTestRequestStatus.Pending.ToString());

			int requiredResponses = _configuration.GetValue<int>("ExpertTestSettings:RequiredResponses");
			if (requiredResponses == 0) requiredResponses = 3;

			if (expiredCount > _maxRetries && pendingCount == 0 && completedResponses.Count() < requiredResponses)
			{
				await PerformAiFallbackAsync(unitOfWork, aiService, testRequest, completedResponses.Count());
				return;
			}

			if (changesMade)
			{
				await unitOfWork.SaveChangesWithTransactionAsync();
			}
		}

		// Helper to extract Penalty Logic
		private async Task ApplyExpertPenalty(IUnitOfWork unitOfWork, EmailService emailService, int expertId, int testRequestId, decimal deduction)
		{
			try
			{
				var expert = await unitOfWork.ExpertRepository.GetByIdAsync(expertId);
				var expertUser = await unitOfWork.UserRepository.GetByIdAsync(expertId);

				if (expert != null && expertUser != null)
				{
					expert.Rating -= deduction;
					if (expert.Rating < 0) expert.Rating = 0;
					await unitOfWork.ExpertRepository.UpdateAsync(expert);

					var penaltyNotification = new Notification
					{
						Title = "Deadline Missed",
						Content = $"You have exceeded the time limit to respond to Test Request #{testRequestId}. {deduction} rating points have been deducted.",
						Receiver = expert.Id,
						TestRequestId = testRequestId,
						ReceivedTime = DateTime.UtcNow,
						Type = "Penalty",
						IsRead = false
					};
					await unitOfWork.NotificationRepository.CreateAsync(penaltyNotification);

					if (expert.Rating < _ratingWarningThreshold)
					{
						var emailSubject = "Action Required: Low Expert Rating Warning";
						var emailBody = $@"
                                    <h3>Warning: Low Rating Alert</h3>
                                    <p>Dear {expertUser.Fullname ?? expertUser.Username},</p>
                                    <p>Your expert rating has dropped to <strong>{expert.Rating:F1}</strong>.</p>
                                    <p>Please ensure you respond to pending requests promptly.</p>";

						await emailService.SendEmailAsync(expertUser.Email, emailSubject, emailBody);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error processing penalty for Expert {expertId}");
			}
		}

		// Helper to extract Warning Logic
		private async Task SendDeadlineWarning(IUnitOfWork unitOfWork, int expertId, int testRequestId)
		{
			var expertNotifications = await unitOfWork.NotificationRepository.GetByReceiverAsync(expertId);
			var alreadyWarned = expertNotifications.Any(n => n.TestRequestId == testRequestId && n.Type == "DeadlineWarning");

			if (!alreadyWarned)
			{
				var warningNotification = new Notification
				{
					Title = $"Deadline Warning: Test #{testRequestId}",
					Content = $"You have less than 24 hours remaining to respond to Request #{testRequestId}.",
					Receiver = expertId,
					TestRequestId = testRequestId,
					ReceivedTime = DateTime.UtcNow,
					Type = "DeadlineWarning",
					IsRead = false
				};
				await unitOfWork.NotificationRepository.CreateAsync(warningNotification);
				await unitOfWork.SaveChangesWithTransactionAsync(); // Save immediately for warnings
			}
		}

		private async Task PerformAiFallbackAsync(
			IUnitOfWork unitOfWork,
			IAIImageAnalysisService aiService,
			TestRequest testRequest,
			int currentResponseCount)
		{
			_logger.LogInformation($"TestRequest {testRequest.Id}: Fallback to AI.");

			// Refund Logic
			var subscription = await unitOfWork.UserSubscriptionRepository.GetSubscriptionForRefundAsync(testRequest.UserAccountId);
			if (subscription != null)
			{
				subscription.RemainingUses++;
				if (subscription.RemainingUses > 0 && !subscription.Status) subscription.Status = true;
				await unitOfWork.UserSubscriptionRepository.UpdateAsync(subscription);

				var refundNotification = new Notification
				{
					Title = "Service Credit Refunded",
					Content = "We were unable to match you with experts in time. 1 credit has been refunded.",
					Receiver = testRequest.UserAccountId,
					TestRequestId = testRequest.Id,
					ReceivedTime = DateTime.Now,
					IsRead = false,
					Type = "Refund"
				};
				await unitOfWork.NotificationRepository.CreateAsync(refundNotification);
			}

			try
			{
				var aiRequest = new Application.Models.AiTest.GeminiAnalysisRequest
				{
					ImageUrls = testRequest.AiPictures.Select(p => p.Source).ToList(),
					HairColor = testRequest.HairColor,
					EyesColor = testRequest.EyesColor,
					LipsColor = testRequest.LipsColor,
					SkinColor = testRequest.SkinColor
				};

				var aiResultModel = await aiService.AnalyzeColorTypeAsync2(aiRequest);
				var colorType = await unitOfWork.ColorTypeRepository.GetByNameAsync(aiResultModel.ColorType);

				var aiTestResult = new AiTestResult
				{
					Id = testRequest.Id,
					Date = DateTime.Now,
					Note = "AI Assistant Analysis (Fallback)",
					SuggestedColor = string.Join(",", aiResultModel.SuggestedColorHexCodes),
					AvoidedColor = string.Join(",", aiResultModel.AvoidedColorHexCodes),
					ColorTypeId = colorType?.Id ?? aiResultModel.ColorTypeId
				};

				await unitOfWork.AiTestResultRepository.CreateAsync(aiTestResult);
				await FinalizeTestAsync(unitOfWork, testRequest, "Expert & AI Analysis Completed");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"AI Fallback failed for TestRequest {testRequest.Id}.");
				testRequest.Status = TestRequestStatus.Failed.ToString();
				await unitOfWork.TestRequestRepository.UpdateAsync(testRequest);
				await unitOfWork.SaveChangesWithTransactionAsync();
			}
		}

		private async Task PerformReviewFallbackAsync(
			IUnitOfWork unitOfWork,
			IAIImageAnalysisService aiService,
			TestRequest testRequest)
		{
			_logger.LogInformation($"Review TestRequest {testRequest.Id}: Fallback to AI.");

			try
			{
				var aiRequest = new Application.Models.AiTest.GeminiAnalysisRequest
				{
					ImageUrls = testRequest.AiPictures.Select(p => p.Source).ToList(),
					HairColor = testRequest.HairColor,
					EyesColor = testRequest.EyesColor,
					LipsColor = testRequest.LipsColor,
					SkinColor = testRequest.SkinColor
				};

				var aiResultModel = await aiService.AnalyzeColorTypeAsync2(aiRequest);
				var colorType = await unitOfWork.ColorTypeRepository.GetByNameAsync(aiResultModel.ColorType);

				// Check if AI result already exists
				var existingAiResult = await unitOfWork.AiTestResultRepository.GetByIdAsync(testRequest.Id);

				if (existingAiResult == null)
				{
					var aiTestResult = new AiTestResult
					{
						Id = testRequest.Id,
						Date = DateTime.Now,
						Note = "AI Review (Fallback)",
						SuggestedColor = string.Join(",", aiResultModel.SuggestedColorHexCodes),
						AvoidedColor = string.Join(",", aiResultModel.AvoidedColorHexCodes),
						ColorTypeId = colorType?.Id ?? aiResultModel.ColorTypeId
					};
					await unitOfWork.AiTestResultRepository.CreateAsync(aiTestResult);
				}
				else
				{
					// Update existing AI result
					existingAiResult.Date = DateTime.Now;
					existingAiResult.Note += " | AI Review (Fallback)";
					existingAiResult.SuggestedColor = string.Join(",", aiResultModel.SuggestedColorHexCodes);
					existingAiResult.AvoidedColor = string.Join(",", aiResultModel.AvoidedColorHexCodes);
					existingAiResult.ColorTypeId = colorType?.Id ?? aiResultModel.ColorTypeId;

					await unitOfWork.AiTestResultRepository.UpdateAsync(existingAiResult);
				}

				// Finalize Review
				testRequest.Status = TestRequestStatus.Completed.ToString();
				await unitOfWork.TestRequestRepository.UpdateAsync(testRequest);

				var notification = new Notification
				{
					Title = "Review Completed",
					Content = "Expert review unavailable. AI has reviewed your test.",
					Receiver = testRequest.UserAccountId,
					TestRequestId = testRequest.Id,
					ReceivedTime = DateTime.Now,
					IsRead = false,
					Type = "ReviewResult"
				};
				await unitOfWork.NotificationRepository.CreateAsync(notification);

				await unitOfWork.SaveChangesWithTransactionAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"AI Review Fallback failed for TestRequest {testRequest.Id}.");
				// Optionally update status to Failed or handle gracefully
			}
		}

		private async Task FinalizeTestAsync(IUnitOfWork unitOfWork, TestRequest testRequest, string message)
		{
			testRequest.Status = TestRequestStatus.Completed.ToString();
			await unitOfWork.TestRequestRepository.UpdateAsync(testRequest);

			var notification = new Notification
			{
				Title = "Your Color Analysis is Ready!",
				Content = message,
				IsRead = false,
				ReceivedTime = DateTime.Now,
				Type = "TestResult",
				Receiver = testRequest.UserAccountId,
				TestRequestId = testRequest.Id
			};
			await unitOfWork.NotificationRepository.CreateAsync(notification);
			await unitOfWork.SaveChangesWithTransactionAsync();
		}
	}
}
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.AI;
using PerHue.Application.Models;
using PerHue.Application.IServices;
using PerHue.Infrastructure.Services; // Ensure this is included for EmailService

namespace PerHue.Infrastructure.SignalR.BroadcastService
{
	public class ExpertTestMonitor : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<ExpertTestMonitor> _logger;
		private const int MaxRetries = 2;
		private const int RequiredResponses = 3;
		private const int DaysToWait = 2; 

		public ExpertTestMonitor(IServiceScopeFactory scopeFactory, ILogger<ExpertTestMonitor> logger)
		{
			_scopeFactory = scopeFactory;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Expert Test Monitor is starting.");

			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation("Expert Test Monitor is running a check.");

				using (var scope = _scopeFactory.CreateScope())
				{
					var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
					var aiAnalysisService = scope.ServiceProvider.GetRequiredService<IAIImageAnalysisService>();
					// Inject EmailService here
					var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

					try
					{
						var pendingTestRequests = await unitOfWork.TestRequestRepository.GetPendingRequestsAsync();

						foreach (var testRequest in pendingTestRequests)
						{
							// Pass emailService to the handler
							await HandleRequestRetriesAndFinalization(unitOfWork, aiAnalysisService, emailService, testRequest);
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error occurred in Expert Test Monitor.");
					}
				}

				// Check every 6 hours (Or adjust frequency as needed)
				//await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
				await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
			}

			_logger.LogInformation("Expert Test Monitor is stopping.");
		}

		private async Task HandleRequestRetriesAndFinalization(
			IUnitOfWork unitOfWork,
			IAIImageAnalysisService aiService,
			EmailService emailService,
			TestRequest testRequest)
		{
			var expertRequests = await unitOfWork.ExpertTestRequestRepository.GetRequestsByTestIdAsync(testRequest.Id);
			var completedResponses = await unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequest.Id);
			var pending = expertRequests.Where(etr => etr.Status == "Pending").ToList();
			var expiredCount = expertRequests.Count(etr => etr.Status == "Expired");

			bool needsNewExpert = false;

			// 1. Check for expired pending requests
			foreach (var req in pending)
			{
				if ((DateTime.UtcNow - req.CreatedDate).TotalDays > DaysToWait)
				{
					_logger.LogInformation($"TestRequest {testRequest.Id} for Expert {req.ExpertId} has expired.");

					// A. Mark as Expired
					req.Status = "Expired";
					await unitOfWork.ExpertTestRequestRepository.UpdateAsync(req);

					// START PENALTY LOGIC 
					try
					{
						// Fetch Expert and User info (ExpertId == UserAccountId)
						var expert = await unitOfWork.ExpertRepository.GetByIdAsync(req.ExpertId);
						var expertUser = await unitOfWork.UserRepository.GetByIdAsync(req.ExpertId);

						if (expert != null && expertUser != null)
						{
							// 1. Deduct Rating
							expert.Rating -= 0.2m;
							if (expert.Rating < 0) expert.Rating = 0;
							await unitOfWork.ExpertRepository.UpdateAsync(expert);

							// 2. Send In-App Notification
							var penaltyNotification = new Notification
							{
								Title = "Deadline Missed",
								Content = $"You missed the response deadline for Test Request #{testRequest.Id}. 0.2 rating points have been deducted.",
								Receiver = expert.Id,
								TestRequestId = testRequest.Id,
								ReceivedTime = DateTime.UtcNow,
								Type = "Penalty",
								IsRead = false
							};
							await unitOfWork.NotificationRepository.CreateAsync(penaltyNotification);

							// 3. Check Rating Threshold & Send Email Warning
							if (expert.Rating < 3.0m)
							{
								var emailSubject = "Action Required: Low Expert Rating Warning";
								var emailBody = $@"
									<h3>Warning: Low Rating Alert</h3>
									<p>Dear {expertUser.Fullname ?? expertUser.Username},</p>
									<p>Your expert rating has dropped to <strong>{expert.Rating:F1}</strong> due to missed deadlines.</p>
									<p>This rating is below the acceptable threshold of 3.0.</p>
									<p>Please ensure you respond to pending requests promptly to restore your standing.</p>
									<p>Best Regards,<br/>PerHue System</p>";

								await emailService.SendEmailAsync(expertUser.Email, emailSubject, emailBody);
							}
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, $"Error processing penalty for Expert {req.ExpertId}");
					}
					// END PENALTY LOGIC 

					expiredCount++;
					needsNewExpert = true;
				}
			}

			// 2. Handle retries if needed (Try to find real experts first)
			if (needsNewExpert && expiredCount <= MaxRetries)
			{
				var assignedExpertIds = expertRequests.Select(etr => etr.ExpertId).ToList();
				// Note: Ensure GetAllAsync works or implement fetching IDs to filter
				var allExperts = await unitOfWork.ExpertRepository.GetAllAsync();

				var newExpert = allExperts.FirstOrDefault(e => !assignedExpertIds.Contains(e.Id) && e.Id != testRequest.UserAccountId);

				if (newExpert != null)
				{
					_logger.LogInformation($"Retrying TestRequest {testRequest.Id}: assigning to new Expert {newExpert.Id}.");
					var newExpertRequest = new ExpertTestRequest
					{
						ExpertId = newExpert.Id,
						TestRequestId = testRequest.Id,
						Status = "Pending",
						CreatedDate = DateTime.Now
					};
					await unitOfWork.ExpertTestRequestRepository.CreateAsync(newExpertRequest);
					await unitOfWork.SaveChangesWithTransactionAsync();
					return; // Wait for this new expert
				}
				else
				{
					_logger.LogWarning($"TestRequest {testRequest.Id}: No new experts available for retry.");
				}
			}

			// 3. Finalization Check
			// Re-fetch pending count in case logic above changed things, though local vars are safer for logic flow
			var currentExpertRequests = await unitOfWork.ExpertTestRequestRepository.GetRequestsByTestIdAsync(testRequest.Id);
			var pendingCount = currentExpertRequests.Count(etr => etr.Status == "Pending");

			// Success: We have 3 responses from experts
			if (completedResponses.Count() >= RequiredResponses)
			{
				await FinalizeTestAsync(unitOfWork, testRequest, "Expert Analysis Completed");
				return;
			}

			// Fallback Trigger:
			// If we are out of retries AND no experts are currently pending (everyone expired or failed)
			if (expiredCount > MaxRetries && pendingCount == 0)
			{
				_logger.LogInformation($"TestRequest {testRequest.Id}: Fallback to AI. Experts responded: {completedResponses.Count()}/{RequiredResponses}");

				// --- REFUND LOGIC ---
				var subscription = await unitOfWork.UserSubscriptionRepository.GetSubscriptionForRefundAsync(testRequest.UserAccountId);
				if (subscription != null)
				{
					subscription.RemainingUses++;
					if (subscription.RemainingUses > 0 && !subscription.Status)
					{
						subscription.Status = true; // Reactivate if it was closed
					}
					await unitOfWork.UserSubscriptionRepository.UpdateAsync(subscription);

					var refundNotification = new Notification
					{
						Title = "Service Credit Refunded",
						Content = "We were unable to match you with experts in time. 1 credit has been refunded to your account.",
						Receiver = testRequest.UserAccountId,
						TestRequestId = testRequest.Id,
						ReceivedTime = DateTime.Now,
						IsRead = false,
						Type = "Refund"
					};
					await unitOfWork.NotificationRepository.CreateAsync(refundNotification);
					_logger.LogInformation($"Refunded 1 use to User {testRequest.UserAccountId}.");
				}

				try
				{
					// Perform AI Analysis
					var aiRequest = new AiTestModel.GeminiAnalysisRequest
					{
						ImageUrls = testRequest.AiPictures.Select(p => p.Source).ToList(),
						HairColor = testRequest.HairColor,
						EyesColor = testRequest.EyesColor,
						LipsColor = testRequest.LipsColor,
						SkinColor = testRequest.SkinColor
					};

					var aiResultModel = await aiService.AnalyzeColorTypeAsync(aiRequest);
					var colorType = await unitOfWork.ColorTypeRepository.GetByNameAsync(aiResultModel.ColorType);

					var aiTestResult = new AiTestResult
					{
						Id = testRequest.Id,
						Date = DateTime.Now,
						Note = "AI Assistant Analysis (Fallback)",
						SuggestedColor = string.Join(",", aiResultModel.SuggestedColor),
						AvoidedColor = string.Join(",", aiResultModel.AvoidedColor),
						ColorTypeId = colorType?.Id ?? aiResultModel.ColorTypeId
					};

					await unitOfWork.AiTestResultRepository.CreateAsync(aiTestResult);
					await FinalizeTestAsync(unitOfWork, testRequest, "Expert & AI Analysis Completed");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"AI Fallback failed for TestRequest {testRequest.Id}.");

					testRequest.Status = "Failed";
					await unitOfWork.TestRequestRepository.UpdateAsync(testRequest);

					var notification = new Notification
					{
						Title = "Test Analysis Failed",
						Content = "We could not complete your analysis at this time.",
						IsRead = false,
						ReceivedTime = DateTime.Now,
						Type = "TestFailed",
						Receiver = testRequest.UserAccountId,
						TestRequestId = testRequest.Id
					};
					await unitOfWork.NotificationRepository.CreateAsync(notification);
					await unitOfWork.SaveChangesWithTransactionAsync();
				}
			}
		}

		private async Task FinalizeTestAsync(IUnitOfWork unitOfWork, TestRequest testRequest, string message)
		{
			testRequest.Status = "Completed";
			await unitOfWork.TestRequestRepository.UpdateAsync(testRequest);

			var notification = new Notification
			{
				Title = "Your Color Analysis is Ready!",
				Content = message,
				IsRead = false,
				ReceivedTime = DateTime.Now,
				Type = "TestResult",
				Receiver = testRequest.UserAccountId,
				TestRequestId = testRequest.Id
			};
			await unitOfWork.NotificationRepository.CreateAsync(notification);
			await unitOfWork.SaveChangesWithTransactionAsync();
		}
	}
}*/

/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Application.Models;


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Application.IServices;

namespace PerHue.Infrastructure.SignalR.BroadcastService
{
	public class ExpertTestMonitor : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<ExpertTestMonitor> _logger;
		private const int MaxRetries = 2;
		private const int RequiredResponses = 3;
		private const int DaysToWait = 2;

		public ExpertTestMonitor(IServiceScopeFactory scopeFactory, ILogger<ExpertTestMonitor> logger)
		{
			_scopeFactory = scopeFactory;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Expert Test Monitor is starting.");

			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation("Expert Test Monitor is running a check.");

				using (var scope = _scopeFactory.CreateScope())
				{
					var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
					var aiAnalysisService = scope.ServiceProvider.GetRequiredService<IAIImageAnalysisService>();

					try
					{
						var pendingTestRequests = await unitOfWork.TestRequestRepository.GetPendingRequestsAsync();

						foreach (var testRequest in pendingTestRequests)
						{
							await HandleRequestRetriesAndFinalization(unitOfWork, aiAnalysisService, testRequest);
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error occurred in Expert Test Monitor.");
					}
				}

				// Check every 6 hours
				await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
			}

			_logger.LogInformation("Expert Test Monitor is stopping.");
		}

		private async Task HandleRequestRetriesAndFinalization(IUnitOfWork unitOfWork, IAIImageAnalysisService aiService, TestRequest testRequest)
		{
			var expertRequests = await unitOfWork.ExpertTestRequestRepository.GetRequestsByTestIdAsync(testRequest.Id);
			var completedResponses = await unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequest.Id);
			var pending = expertRequests.Where(etr => etr.Status == "Pending").ToList();
			var expiredCount = expertRequests.Count(etr => etr.Status == "Expired");

			bool needsNewExpert = false;

			// 1. Check for expired pending requests
			foreach (var req in pending)
			{
				if ((DateTime.Now - req.CreatedDate).TotalDays > DaysToWait)
				{
					_logger.LogInformation($"TestRequest {testRequest.Id} for Expert {req.ExpertId} has expired.");
					req.Status = "Expired";
					await unitOfWork.ExpertTestRequestRepository.UpdateAsync(req);
					expiredCount++;
					needsNewExpert = true;
				}
			}

			// 2. Handle retries if needed (Try to find real experts first)
			if (needsNewExpert && expiredCount <= MaxRetries)
			{
				var assignedExpertIds = expertRequests.Select(etr => etr.ExpertId).ToList();
				// Assuming GetByIdAsync includes UserAccount, but we need GetAll here. 
				// Note: This logic assumes Repository has a GetAll method.
				var allExperts = await unitOfWork.ExpertRepository.GetAllAsync();

				var newExpert = allExperts.FirstOrDefault(e => !assignedExpertIds.Contains(e.Id) && e.Id != testRequest.UserAccountId);

				if (newExpert != null)
				{
					_logger.LogInformation($"Retrying TestRequest {testRequest.Id}: assigning to new Expert {newExpert.Id}.");
					var newExpertRequest = new ExpertTestRequest
					{
						ExpertId = newExpert.Id,
						TestRequestId = testRequest.Id,
						Status = "Pending",
						CreatedDate = DateTime.Now
					};
					await unitOfWork.ExpertTestRequestRepository.CreateAsync(newExpertRequest);
					await unitOfWork.SaveChangesWithTransactionAsync();
					return; // Wait for this new expert
				}
				else
				{
					_logger.LogWarning($"TestRequest {testRequest.Id}: No new experts available for retry.");
				}
			}

			// 3. Finalization Check
			// Re-fetch pending count
			var currentExpertRequests = await unitOfWork.ExpertTestRequestRepository.GetRequestsByTestIdAsync(testRequest.Id);
			var pendingCount = currentExpertRequests.Count(etr => etr.Status == "Pending");

			// Success: We have 3 responses from experts
			if (completedResponses.Count() >= RequiredResponses)
			{
				await FinalizeTestAsync(unitOfWork, testRequest, "Expert Analysis Completed");
				return;
			}

			// Fallback Trigger:
			// If we are out of retries AND no experts are currently pending (everyone expired or failed)
			// AND we don't have enough responses (e.g. we have 2, need 3)
			if (expiredCount > MaxRetries && pendingCount == 0)
			{
				_logger.LogInformation($"TestRequest {testRequest.Id}: Fallback to AI. Experts responded: {completedResponses.Count()}/{RequiredResponses}");

				// --- REFUND LOGIC ---
				// Since experts failed to fulfill the request, we refund the user.
				var subscription = await unitOfWork.UserSubscriptionRepository.GetSubscriptionForRefundAsync(testRequest.UserAccountId);
				if (subscription != null)
				{
					subscription.RemainingUses++;
					if (subscription.RemainingUses > 0 && !subscription.Status)
					{
						subscription.Status = true; // Reactivate if it was closed
					}
					await unitOfWork.UserSubscriptionRepository.UpdateAsync(subscription);
					// --- NOTIFICATION HERE ---
					var refundNotification = new Notification
					{
						Title = "Service Credit Refunded",
						Content = "We were unable to match you with experts in time. 1 credit has been refunded to your account.",
						Receiver = testRequest.UserAccountId,
						TestRequestId = testRequest.Id,
						ReceivedTime = DateTime.Now,
						IsRead = false,
						Type = "Refund"
					};
					await unitOfWork.NotificationRepository.CreateAsync(refundNotification);
					_logger.LogInformation($"Refunded 1 use to User {testRequest.UserAccountId}.");
				}

				try
				{
					// Perform AI Analysis
					var aiRequest = new AiTestModel.GeminiAnalysisRequest
					{
						// Use the AI Pictures associated with the request
						ImageUrls = testRequest.AiPictures.Select(p => p.Source).ToList(),
						HairColor = testRequest.HairColor,
						EyesColor = testRequest.EyesColor,
						LipsColor = testRequest.LipsColor,
						SkinColor = testRequest.SkinColor
					};

					// Call Gemini
					var aiResultModel = await aiService.AnalyzeColorTypeAsync(aiRequest);

					// Convert to Entity and Save
					// Note: AI Result usually creates a 1-to-1 entry in AiTestResult table
					var colorType = await unitOfWork.ColorTypeRepository.GetByNameAsync(aiResultModel.ColorType); // Ensure this Repo method exists or use ID logic

					var aiTestResult = new AiTestResult
					{
						// For 1-to-1 relationship, the ID is often the same as TestRequest ID
						// Or if it's auto-increment, map TestRequestId. 
						// Based on your schema: public virtual TestRequest IdNavigation { get; set; } means PK is FK.
						Id = testRequest.Id,
						Date = DateTime.Now,
						Note = "AI Assistant Analysis (Fallback)",
						SuggestedColor = string.Join(",", aiResultModel.SuggestedColor),
						AvoidedColor = string.Join(",", aiResultModel.AvoidedColor),
						ColorTypeId = colorType?.Id ?? aiResultModel.ColorTypeId // Fallback to ID from model if Name lookup fails
					};

					await unitOfWork.AiTestResultRepository.CreateAsync(aiTestResult);

					// Mark as Completed
					await FinalizeTestAsync(unitOfWork, testRequest, "Expert & AI Analysis Completed");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"AI Fallback failed for TestRequest {testRequest.Id}.");

					// Only mark as failed if we have ZERO responses. 
					// If we have 2 expert responses, we might still want to show them? 
					// For now, sticking to flow: "Failed".
					testRequest.Status = "Failed";
					await unitOfWork.TestRequestRepository.UpdateAsync(testRequest);

					var notification = new Notification
					{
						Title = "Test Analysis Failed",
						Content = "We could not complete your analysis at this time.",
						IsRead = false,
						ReceivedTime = DateTime.Now,
						Type = "TestFailed",
						Receiver = testRequest.UserAccountId,
						TestRequestId = testRequest.Id
					};
					await unitOfWork.NotificationRepository.CreateAsync(notification);
					await unitOfWork.SaveChangesWithTransactionAsync();
				}
			}
		}

		private async Task FinalizeTestAsync(IUnitOfWork unitOfWork, TestRequest testRequest, string message)
		{
			testRequest.Status = "Completed";
			await unitOfWork.TestRequestRepository.UpdateAsync(testRequest);

			var notification = new Notification
			{
				Title = "Your Color Analysis is Ready!",
				Content = message,
				IsRead = false,
				ReceivedTime = DateTime.Now,
				Type = "TestResult",
				Receiver = testRequest.UserAccountId,
				TestRequestId = testRequest.Id
			};
			await unitOfWork.NotificationRepository.CreateAsync(notification);
			await unitOfWork.SaveChangesWithTransactionAsync();
		}
	}
}

*/