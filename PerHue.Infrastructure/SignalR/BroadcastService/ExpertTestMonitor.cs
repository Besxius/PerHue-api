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

					try
					{
						var pendingTestRequests = await unitOfWork.TestRequestRepository.GetPendingRequestsAsync();

						foreach (var testRequest in pendingTestRequests)
						{
							await HandleRequestRetries(unitOfWork, testRequest);
							await HandleRequestAggregation(scope, unitOfWork, testRequest);
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error occurred in Expert Test Monitor.");
					}
				}

				// Wait for 6 hours before checking again (adjust as needed)
				await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
			}

			_logger.LogInformation("Expert Test Monitor is stopping.");
		}

		private async Task HandleRequestRetries(IUnitOfWork unitOfWork, TestRequest testRequest)
		{
			var expertRequests = await unitOfWork.ExpertTestRequestRepository.GetRequestsByTestIdAsync(testRequest.Id);
			var pending = expertRequests.Where(etr => etr.Status == "Pending").ToList();
			var expiredCount = expertRequests.Count(etr => etr.Status == "Expired");

			bool needsNewExpert = false;

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

			// If a request expired and we are under the retry limit, find a new expert
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
				}
				else
				{
					_logger.LogWarning($"TestRequest {testRequest.Id}: No new experts available for retry.");
				}
			}

			await unitOfWork.SaveChangesWithTransactionAsync();
		}

		/*private async Task HandleRequestAggregation(IServiceScope scope, IUnitOfWork unitOfWork, TestRequest testRequest)
		{
			var expertRequests = await unitOfWork.ExpertTestRequestRepository.GetRequestsByTestIdAsync(testRequest.Id);
			var completedResponses = await unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequest.Id);
			var expiredCount = expertRequests.Count(etr => etr.Status == "Expired");
			var pendingCount = expertRequests.Count(etr => etr.Status == "Pending");

			bool shouldAggregate = false;
			bool useAiFallback = false;

			// Condition 1: We have enough responses
			if (completedResponses.Count() >= RequiredResponses)
			{
				shouldAggregate = true;
			}
			// Condition 2: We have maxed out retries and have no more pending responses
			else if (expiredCount > MaxRetries && pendingCount == 0)
			{
				shouldAggregate = true;
				if (completedResponses.Count() < RequiredResponses)
				{
					useAiFallback = true;
				}
			}

			if (shouldAggregate)
			{
				_logger.LogInformation($"Aggregating TestRequest {testRequest.Id}. AI Fallback: {useAiFallback}");

				AiTestResult finalResult;

				if (useAiFallback)
				{
					// Fallback to AI (Diagram node 41)
					finalResult = await RunAIFallbackAsync(scope, testRequest);
				}
				else
				{
					// Aggregate expert responses (Diagram node 27)
					finalResult = AggregateExpertResponses(completedResponses, testRequest);
				}

				await unitOfWork.AiTestResultRepository.CreateAsync(finalResult);

				// Finalize test request
				testRequest.Status = "Completed";
				testRequest.AiTestResult = finalResult;
				await unitOfWork.TestRequestRepository.UpdateAsync(testRequest);

				// Create notification for user
				var notification = new Notification
				{
					Title = "Your In-Depth Color Test is Ready!",
					Content = $"Your color analysis is complete. Your personal color type is {finalResult.ColorType.Name}.",
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

		private AiTestResult AggregateExpertResponses(IEnumerable<TestResponse> responses, TestRequest request)
		{
			// Simple aggregation: find the most-voted ColorType
			var mostVotedColorType = responses
				.GroupBy(r => r.ColorTypeId)
				.OrderByDescending(g => g.Count())
				.Select(g => g.Key)
				.First();

			// Collect all suggested/avoided colors
			var suggested = responses.SelectMany(r => r.BestColor.Split(',')).Distinct();
			var avoided = responses.SelectMany(r => r.WorstColor.Split(',')).Distinct();

			return new AiTestResult
			{
				Id = request.Id,
				Note = "Result aggregated from 3 expert analyses.",
				Date = DateTime.Now,
				SuggestedColor = string.Join(",", suggested),
				AvoidedColor = string.Join(",", avoided),
				ColorTypeId = mostVotedColorType
			};
		}*/
		private async Task HandleRequestAggregation(IServiceScope scope, IUnitOfWork unitOfWork, TestRequest testRequest)
		{
			var expertRequests = await unitOfWork.ExpertTestRequestRepository.GetRequestsByTestIdAsync(testRequest.Id);
			var completedResponses = await unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequest.Id);
			var expiredCount = expertRequests.Count(etr => etr.Status == "Expired");
			var pendingCount = expertRequests.Count(etr => etr.Status == "Pending");

			bool shouldFinalize = false;
			bool didFail = false; // Flag to indicate failure

			// Condition 1: We have enough responses (Success)
			if (completedResponses.Count() >= RequiredResponses)
			{
				shouldFinalize = true;
				didFail = false;
			}
			// Condition 2: We have maxed out retries and have no more pending responses (Failure)
			else if (expiredCount > MaxRetries && pendingCount == 0)
			{
				shouldFinalize = true;
				didFail = true;
			}

			if (shouldFinalize)
			{
				Notification notification;

				if (didFail)
				{
					// AI Fallback is removed. Mark as Failed and notify user.
					_logger.LogWarning($"TestRequest {testRequest.Id} failed to get {RequiredResponses} expert responses. Max retries reached.");
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
					// Success! Aggregate expert responses (Diagram node 27)
					_logger.LogInformation($"Aggregating TestRequest {testRequest.Id}.");

					// This is the line that was causing the error
					AiTestResult finalResult = AggregateExpertResponses(completedResponses, testRequest);

					// This was the other line causing an error
					await unitOfWork.AiTestResultRepository.CreateAsync(finalResult);

					testRequest.Status = "Completed";
					testRequest.AiTestResult = finalResult;

					var colorType = await unitOfWork.ColorTypeRepository.GetByIdAsync(finalResult.ColorTypeId);

					notification = new Notification
					{
						Title = "Your In-Depth Color Test is Ready!",
						Content = $"Your color analysis is complete. Your personal color type is {colorType?.Name ?? "Unknown"}.",
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

		private AiTestResult AggregateExpertResponses(IEnumerable<TestResponse> responses, TestRequest request)
		{
			// Simple aggregation: find the most-voted ColorType
			var mostVotedColorType = responses
				.GroupBy(r => r.ColorTypeId)
				.OrderByDescending(g => g.Count())
				.Select(g => g.Key)
				.First();

			// Collect all suggested/avoided colors, split by comma, trim whitespace, and get unique
			var suggested = responses.SelectMany(r => r.BestColor.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
									 .Select(s => s.Trim())
									 .Distinct();
			var avoided = responses.SelectMany(r => r.WorstColor.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
									 .Select(s => s.Trim())
									 .Distinct();

			return new AiTestResult
			{
				Id = request.Id, // This links it to the TestRequest
				Note = "Result aggregated from 3 expert analyses.",
				Date = DateTime.Now,
				SuggestedColor = string.Join(",", suggested),
				AvoidedColor = string.Join(",", avoided),
				ColorTypeId = mostVotedColorType
			};
		}

		/*private async Task<AiTestResult> RunAIFallbackAsync(IServiceScope scope, TestRequest request)
		{
			var geminiService = scope.ServiceProvider.GetRequiredService<GeminiService>();
			var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

			// Find the primary picture for the test
			var primaryPicture = (await unitOfWork.AiPictureRepository.GetAllAsync())
									.FirstOrDefault(p => p.TestRequestId == request.Id);

			if (primaryPicture == null)
			{
				_logger.LogError($"AI Fallback for TestRequest {request.Id} failed: No picture found.");
				return new AiTestResult { Id = request.Id, Note = "AI Fallback Failed - No Image", ColorTypeId = 1, SuggestedColor = "", AvoidedColor = "" }; // Default or error state
			}

			string jsonResponse = await geminiService.GeneratePromptWithImageFromUrl(primaryPicture.Source);

			// Deserialize AI response
			var aiModel = JsonSerializer.Deserialize<AiTestResultModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			var colorType = await unitOfWork.ColorTypeRepository.GetByNameAsync(aiModel.ColorType);

			return new AiTestResult
			{
				Id = request.Id,
				Note = "Result generated by AI analysis.",
				Date = DateTime.Now,
				SuggestedColor = string.Join(",", aiModel.SuggestedColor),
				AvoidedColor = string.Join(",", aiModel.AvoidedColor),
				ColorTypeId = colorType?.Id ?? 1 // Default to 1 if name not found
			};
		}*/
	}
}