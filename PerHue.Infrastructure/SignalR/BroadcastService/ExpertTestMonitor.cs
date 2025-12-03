using System;
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
using System.Text.Json;
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
				if ((DateTime.UtcNow - req.CreatedDate).TotalDays > DaysToWait)
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

/*namespace PerHue.Infrastructure.SignalR.BroadcastService
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

					try
					{
						var pendingTestRequests = await unitOfWork.TestRequestRepository.GetPendingRequestsAsync();

						foreach (var testRequest in pendingTestRequests)
						{
							// We combine both checks into one "Finalize" check
							await HandleRequestRetriesAndFinalization(unitOfWork, testRequest);
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error occurred in Expert Test Monitor.");
					}
				}

				await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
			}

			_logger.LogInformation("Expert Test Monitor is stopping.");
		}

		private async Task HandleRequestRetriesAndFinalization(IUnitOfWork unitOfWork, TestRequest testRequest)
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
					req.Status = "Expired";
					await unitOfWork.ExpertTestRequestRepository.UpdateAsync(req);
					expiredCount++;
					needsNewExpert = true;
				}
			}

			// 2. Handle retries if needed
			if (needsNewExpert && expiredCount <= MaxRetries)
			{
				var assignedExpertIds = expertRequests.Select(etr => etr.ExpertId).ToList();
				var newExpert = (await unitOfWork.ExpertRepository.GetAllAsync())
								  .FirstOrDefault(e => !assignedExpertIds.Contains(e.Id));

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

					// A new expert was added, so we save and wait for the next cycle
					await unitOfWork.SaveChangesWithTransactionAsync();
					return;
				}
				else
				{
					_logger.LogWarning($"TestRequest {testRequest.Id}: No new experts available for retry.");
				}
			}

			// 3. Check for finalization (Success or Failure)
			// Re-fetch pending count in case a new expert was just added
			var pendingCount = (await unitOfWork.ExpertTestRequestRepository.GetRequestsByTestIdAsync(testRequest.Id))
								.Count(etr => etr.Status == "Pending");

			bool shouldFinalize = false;
			bool didFail = false;

			if (completedResponses.Count() >= RequiredResponses)
			{
				shouldFinalize = true;
				didFail = false; // Success
			}
			else if (expiredCount > MaxRetries && pendingCount == 0)
			{
				shouldFinalize = true;
				didFail = true; // Failure
			}

			if (shouldFinalize)
			{
				Notification notification;

				if (didFail)
				{
					// Failure: Max retries hit, not enough responses
					_logger.LogWarning($"TestRequest {testRequest.Id} failed to get {RequiredResponses} expert responses.");
					testRequest.Status = "Failed";

					notification = new Notification
					{
						Title = "Your In-Depth Color Test Failed",
						Content = "We're sorry, we couldn't get a result for your test at this time. Please try submitting again later.",
						IsRead = false,
						ReceivedTime = DateTime.Now,
						Type = "TestFailed",
						Receiver = testRequest.UserAccountId,
						TestRequestId = testRequest.Id
					};
				}
				else
				{
					// Success: We have 3+ responses. Mark as completed.
					_logger.LogInformation($"TestRequest {testRequest.Id} is complete with {completedResponses.Count()} responses.");
					testRequest.Status = "Completed";

					notification = new Notification
					{
						Title = "Your In-Depth Color Test is Ready!",
						Content = "Your expert color analysis is complete. You can now view your results.",
						IsRead = false,
						ReceivedTime = DateTime.Now,
						Type = "TestResult",
						Receiver = testRequest.UserAccountId,
						TestRequestId = testRequest.Id
					};
				}

				await unitOfWork.TestRequestRepository.UpdateAsync(testRequest);
				await unitOfWork.NotificationRepository.CreateAsync(notification);
				await unitOfWork.SaveChangesWithTransactionAsync();
			}
		}
	}
}*/
