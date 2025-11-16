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

		// --- Aggregation methods are no longer needed and have been removed ---
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
	}
}*/