using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.Extensions.Configuration;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.ExpertTestResult;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Services
{
	internal class ExpertTestService : IExpertTestService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public ExpertTestService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_configuration = configuration;
		}

		public async Task<IEnumerable<TestRequestModel>> GetPendingRequestsAsync(int expertId)
		{
			var expertRequests = await _unitOfWork.ExpertTestRequestRepository.GetPendingRequestsForExpertAsync(expertId);

			// Map the underlying TestRequest to TestRequestModel
			var testRequests = expertRequests.Select(etr => etr.TestRequest);
			return _mapper.Map<IEnumerable<TestRequestModel>>(testRequests);
		}
		public async Task<IEnumerable<ExpertAssignmentModel>> GetAllRequestsAsync(int expertId)
		{
			// Fetch the ExpertTestRequests (which contain the Status and Link to TestRequest)
			var expertRequests = await _unitOfWork.ExpertTestRequestRepository.GetAllRequestsForExpertAsync(expertId);

			// Map to the new ExpertAssignmentModel
			var result = expertRequests.Select(etr =>
			{
				// 1. Map the base TestRequest details
				var model = _mapper.Map<ExpertAssignmentModel>(etr.TestRequest);

				// 2. Populate the Expert-specific details
				model.ExpertStatus = etr.Status;           // The status from Expert_TestRequest (Completed/Pending/etc)
				model.AssignmentDate = etr.CreatedDate;    // The date assigned to Expert

				return model;
			});

			return result;
		}

		public async Task<TestResponseModel> SubmitResponseAsync(CreateTestResponseModel model, int expertId)
		{
			// 1. Verify the expert has a pending request for this test
			var pendingRequest = await _unitOfWork.ExpertTestRequestRepository.GetPendingRequestAsync(expertId, model.TestRequestId);
			if (pendingRequest == null)
			{
				throw new InvalidOperationException("No pending test request found for this expert and test.");
			}

			// 2. Map DTO to Entity
			var testResponse = _mapper.Map<TestResponse>(model);
			testResponse.ExpertId = expertId;
			testResponse.CreatedDate = DateTime.Now;
			testResponse.Type = ResponseTypeEnum.Normal.ToString();

			// 3. Save the response
			await _unitOfWork.TestResponseRepository.CreateAsync(testResponse);

			// 4. Update the ExpertTestRequest status to "Completed"
			pendingRequest.Status = ExpertTestRequestStatus.Completed.ToString();
			await _unitOfWork.ExpertTestRequestRepository.UpdateAsync(pendingRequest);

			// 5. Save changes
			await _unitOfWork.SaveChangesWithTransactionAsync();

			return _mapper.Map<TestResponseModel>(testResponse);
		}

		public async Task<ExpertTestResultModel> GetExpertResponsesForUserAsync(int testRequestId, int userId)
		{
			var testRequest = await _unitOfWork.TestRequestRepository.GetByIdWithDetailsAsync(testRequestId);

			if (testRequest == null) throw new Exception("Test request not found.");
			if (testRequest.UserAccountId != userId) throw new UnauthorizedAccessException("You are not authorized.");
			//if (testRequest.Status != "Completed") throw new Exception("Test is still being processed.");

			var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequestId);
			var responseModels = _mapper.Map<List<TestResponseModel>>(responses);

			// --- CHECK FOR AI RESULT ---
			// Since GetByIdWithDetailsAsync now Includes AiTestResult, we can use it directly
			if (testRequest.AiTestResult != null)
			{
				var aiResponseModel = new TestResponseModel
				{
					Id = 0,
					TestRequestId = testRequestId,
					ExpertId = 0,
					Note = testRequest.AiTestResult.Note,
					CreatedDate = testRequest.AiTestResult.Date,
					Rating = null,
					BestColor = testRequest.AiTestResult.SuggestedColor,
					WorstColor = testRequest.AiTestResult.AvoidedColor,
					ColorTypeId = testRequest.AiTestResult.ColorTypeId,
					ColorTypeName = testRequest.AiTestResult.ColorType?.Name ?? "Unknown"
				};
				responseModels.Add(aiResponseModel);
			}

			return new ExpertTestResultModel
			{
				TestRequest = _mapper.Map<TestRequestModel>(testRequest),
				Responses = responseModels
			};
		}
		public async Task<IEnumerable<ExpertTestResultModel>> GetAllCompletedExpertTestsAsync()
		{
			var completedTests = await _unitOfWork.TestRequestRepository.GetCompletedExpertTestsAsync();
			var results = new List<ExpertTestResultModel>();

			// This loop can be performance-intensive if you have thousands of tests.
			// For a small to medium number of tests, it is perfectly fine.
			foreach (var test in completedTests)
			{
				var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(test.Id);

				results.Add(new ExpertTestResultModel
				{
					TestRequest = _mapper.Map<TestRequestModel>(test),
					Responses = _mapper.Map<IEnumerable<TestResponseModel>>(responses)
				});
			}

			return results;
		}
		public async Task<IEnumerable<TestRequestModel>> GetAllExpertTestRequestsAsync()
		{
			var requests = await _unitOfWork.TestRequestRepository.GetAllExpertTestsAsync();
			return _mapper.Map<IEnumerable<TestRequestModel>>(requests);
		}

		public async Task<PaginatedResult<ExpertTestResultModel>> GetMyCompletedExpertTestsAsync(int userId, int pageIndex, int pageSize, DateTime? fromDate, DateTime? toDate)
		{
			// 1. Get Paged Test Requests
			var (testRequests, totalCount) = await _unitOfWork.TestRequestRepository.GetCompletedExpertTestsForUserAsync(userId, pageIndex, pageSize, fromDate, toDate);

			var resultItems = new List<ExpertTestResultModel>();

			// 2. Loop through requests
			foreach (var test in testRequests)
			{
				// Get Expert responses
				var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(test.Id);
				var mappedResponses = _mapper.Map<List<TestResponseModel>>(responses);

				// --- NEW LOGIC: Check for AI Result ---
				if (test.AiTestResult != null)
				{
					var aiResponseModel = new TestResponseModel
					{
						Id = 0, // Indicator for AI
						TestRequestId = test.Id,
						ExpertId = 0, // 0 = AI/System
						Note = test.AiTestResult.Note,
						CreatedDate = test.AiTestResult.Date,
						Rating = null,
						BestColor = test.AiTestResult.SuggestedColor,
						WorstColor = test.AiTestResult.AvoidedColor,
						ColorTypeId = test.AiTestResult.ColorTypeId,
						ColorTypeName = test.AiTestResult.ColorType?.Name ?? "Unknown"
					};
					mappedResponses.Add(aiResponseModel);
				}

				resultItems.Add(new ExpertTestResultModel
				{
					TestRequest = _mapper.Map<TestRequestModel>(test),
					Responses = mappedResponses
				});
			}

			// 3. Return Paginated Result
			return new PaginatedResult<ExpertTestResultModel>
			{
				Items = resultItems,
				TotalCount = totalCount,
				PageIndex = pageIndex,
				PageSize = pageSize,
				TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
			};
		}
		/*public async Task RateExpertResponseAsync(RateExpertResponseModel model, int userId)
		{
			// 1. Get the TestResponse and its parent TestRequest
			var testResponse = await _unitOfWork.TestResponseRepository.GetByIdAsync(model.TestResponseId);
			if (testResponse == null)
			{
				throw new Exception("Test response not found.");
			}

			var testRequest = await _unitOfWork.TestRequestRepository.GetByIdAsync(testResponse.TestRequestId);
			if (testRequest == null)
			{
				throw new Exception("Parent test request not found.");
			}

			// 2. Security Check: Ensure the user rating this is the one who created the test
			if (testRequest.UserAccountId != userId)
			{
				throw new UnauthorizedAccessException("You are not authorized to rate this response.");
			}

			// 3. Check if already rated
			if (testResponse.Rating != null)
			{
				throw new InvalidOperationException("This response has already been rated.");
			}

			// 4. Save the rating to the TestResponse
			testResponse.Rating = model.Rating;
			await _unitOfWork.TestResponseRepository.UpdateAsync(testResponse);

			// 5. Recalculate the Expert's average rating
			var expert = await _unitOfWork.ExpertRepository.GetByIdAsync(testResponse.ExpertId);
			if (expert == null)
			{
				// This shouldn't happen, but good to check
				throw new Exception("Expert not found.");
			}

			// Get all *rated* responses for this expert
			var allRatedResponses = await _unitOfWork.TestResponseRepository.GetAllByExpertIdAsync(expert.Id);

			// Calculate new average
			var newAverage = allRatedResponses.Average(r => r.Rating);
			expert.Rating = (decimal)newAverage;

			await _unitOfWork.ExpertRepository.UpdateAsync(expert);

			// 6. Save all changes in one transaction
			await _unitOfWork.SaveChangesWithTransactionAsync();
		}*/
		public async Task RateExpertResponseAsync(RateExpertResponseModel model, int userId)
		{
			// 1. Get the TestResponse and its parent TestRequest
			var testResponse = await _unitOfWork.TestResponseRepository.GetByIdAsync(model.TestResponseId);
			if (testResponse == null) throw new Exception("Test response not found.");

			var testRequest = await _unitOfWork.TestRequestRepository.GetByIdAsync(testResponse.TestRequestId);
			if (testRequest == null) throw new Exception("Parent test request not found.");

			// 2. Security Check
			if (testRequest.UserAccountId != userId) throw new UnauthorizedAccessException("You are not authorized to rate this response.");

			// 3. Check if already rated
			if (testResponse.Rating != null) throw new InvalidOperationException("This response has already been rated.");

			// 4. Save the rating to the TestResponse
			testResponse.Rating = model.Rating;
			await _unitOfWork.TestResponseRepository.UpdateAsync(testResponse);

			// 5. Recalculate the Expert's Overall Rating
			var expert = await _unitOfWork.ExpertRepository.GetByIdAsync(testResponse.ExpertId);
			if (expert == null) throw new Exception("Expert not found.");

			// A. Calculate Average from "Rated" Responses
			var allResponses = await _unitOfWork.TestResponseRepository.GetAllByExpertIdAsync(expert.Id);
			var ratedResponses = allResponses.Where(r => r.Rating != null && r.Rating > 0).ToList();

			decimal averageRating = 0;
			if (ratedResponses.Any())
			{
				averageRating = (decimal)ratedResponses.Average(r => r.Rating!.Value);
			}
			else
			{
				averageRating = 5.0m;
			}

			// B. Calculate Total Penalty from "Expired" Requests
			var expiredRequests = await _unitOfWork.ExpertTestRequestRepository
				.FindAsync(r => r.ExpertId == expert.Id && r.Status == "Expired");

			int expiredCount = expiredRequests.Count();

			// Get penalty amount from configuration (default 0.2)
			decimal ratingDeduction = _configuration.GetValue<decimal>("ExpertTestSettings:RatingDeduction");
			decimal totalPenalty = expiredCount * ratingDeduction;

			// C. Apply Formula: Rating = Average - Penalty
			decimal finalRating = averageRating - totalPenalty;

			// D. Ensure Rating doesn't drop below 0
			if (finalRating < 0) finalRating = 0;

			expert.Rating = finalRating;
			await _unitOfWork.ExpertRepository.UpdateAsync(expert);

			// 6. Save all changes in one transaction
			await _unitOfWork.SaveChangesWithTransactionAsync();
		}
		public async Task<IEnumerable<ReviewTestRequestModel>> GetPendingReviewRequestsAsync(int expertId)
		{
			// 1. Fetch requests with "PendingReview" status
			var expertRequests = await _unitOfWork.ExpertTestRequestRepository.GetPendingReviewRequestsForExpertAsync(expertId);

			var result = new List<ReviewTestRequestModel>();

			foreach (var req in expertRequests)
			{
				// 2. Map the TestRequest
				var requestModel = _mapper.Map<TestRequestModel>(req.TestRequest);

				// 3. Map the Responses (The 3 initial ones)
				// Note: We filter out any potential 'Review' type responses to be safe, showing only the original work.
				var previousResponses = req.TestRequest.TestResponses
					.Where(r => r.Type != ResponseTypeEnum.Review.ToString())
					.ToList();

				var responseModels = _mapper.Map<IEnumerable<TestResponseModel>>(previousResponses);

				result.Add(new ReviewTestRequestModel
				{
					ExpertTestRequestId = req.ExpertId, // Or the composite ID if needed, usually TestRequestId is enough context
					TestRequest = requestModel,
					PreviousResponses = responseModels
				});
			}

			return result;
		}

		// --- IMPLEMENT VOTING ---
		public async Task<TestResponseModel> VoteForResponseAsync(VoteResponseModel model, int expertId)
		{
			// 1. Verify the expert has a pending request for this test
			var pendingRequest = await _unitOfWork.ExpertTestRequestRepository.GetPendingReviewRequestAsync(expertId, model.TestRequestId);

			if (pendingRequest == null)
			{
				throw new InvalidOperationException("No pending review request found for this expert.");
			}

			// 2. Get the response the expert voted for
			var votedResponse = await _unitOfWork.TestResponseRepository.GetByIdAsync(model.VotedResponseId);
			if (votedResponse == null || votedResponse.TestRequestId != model.TestRequestId)
			{
				throw new ArgumentException("Invalid response selected.");
			}

			// 3. Create the Review Response (Copying data)
			var reviewResponse = new TestResponse
			{
				TestRequestId = model.TestRequestId,
				ExpertId = expertId,
				CreatedDate = DateTime.Now,
				Type = ResponseTypeEnum.Review.ToString(), // Set Type to Review

				// Copy core analysis data
				BestColor = votedResponse.BestColor,
				WorstColor = votedResponse.WorstColor,
				ColorTypeId = votedResponse.ColorTypeId,

				// Add note indicating it's a review/vote
				Note = string.IsNullOrWhiteSpace(model.Note)
					? $"Reviewed and agreed with Expert {votedResponse.ExpertId}."
					: model.Note
			};

			await _unitOfWork.TestResponseRepository.CreateAsync(reviewResponse);

			// 4. Mark request as completed
			pendingRequest.Status = ExpertTestRequestStatus.Completed.ToString();
			await _unitOfWork.ExpertTestRequestRepository.UpdateAsync(pendingRequest);

			// --- STATUS CHANGE: Mark main request back to Completed ---
			var mainRequest = await _unitOfWork.TestRequestRepository.GetByIdAsync(model.TestRequestId);
			if (mainRequest != null)
			{
				mainRequest.Status = TestRequestStatus.Completed.ToString();
				await _unitOfWork.TestRequestRepository.UpdateAsync(mainRequest);

				// --- NOTIFICATION: For the User ---
				var notification = new Notification
				{
					Title = "Review Completed",
					Content = "The expert review you requested has been completed.",
					Receiver = mainRequest.UserAccountId,
					TestRequestId = model.TestRequestId,
					ReceivedTime = DateTime.Now,
					IsRead = false,
					Type = "ReviewResult"
				};
				await _unitOfWork.NotificationRepository.CreateAsync(notification);
			}

			await _unitOfWork.SaveChangesWithTransactionAsync();

			// 5. Return mapped model (Fetching color type name handled by mapper if entity loaded, or we rely on lazy loading/repo include)
			// To be safe, let's load the ColorType for mapping
			var colorType = await _unitOfWork.ColorTypeRepository.GetByIdAsync(reviewResponse.ColorTypeId);
			reviewResponse.ColorType = colorType;

			return _mapper.Map<TestResponseModel>(reviewResponse);
		}
	}
}


/*public async Task<ExpertTestResultModel> GetExpertResponsesForUserAsync(int testRequestId, int userId)
		{
			// 1. Get the main test request, now with all details
			var testRequest = await _unitOfWork.TestRequestRepository.GetByIdWithDetailsAsync(testRequestId);

			if (testRequest == null)
			{
				throw new Exception("Test request not found.");
			}

			// 2. Security Check: Ensure the user asking for the test is the one who created it
			if (testRequest.UserAccountId != userId)
			{
				throw new UnauthorizedAccessException("You are not authorized to view this test result.");
			}

			// 3. Check if the test is finished
			if (testRequest.Status != "Completed")
			{
				throw new Exception("Your test is still being processed by our experts.");
			}

			// 4. Get the individual responses
			var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequestId);

			// 5. Build the complete response DTO
			var result = new ExpertTestResultModel
			{
				TestRequest = _mapper.Map<TestRequestModel>(testRequest),
				Responses = _mapper.Map<IEnumerable<TestResponseModel>>(responses)
			};

			return result;
		}*/
/*public async Task<ExpertTestResultModel> GetExpertResponsesForUserAsync(int testRequestId, int userId)
{
	// 1. Get the main test request
	var testRequest = await _unitOfWork.TestRequestRepository.GetByIdWithDetailsAsync(testRequestId);

	if (testRequest == null)
	{
		throw new Exception("Test request not found.");
	}

	// 2. Security Check
	if (testRequest.UserAccountId != userId)
	{
		throw new UnauthorizedAccessException("You are not authorized to view this test result.");
	}

	// 3. Check status
	if (testRequest.Status != "Completed")
	{
		throw new Exception("Your test is still being processed.");
	}

	// 4. Get EXPERT responses
	var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequestId);
	var responseModels = _mapper.Map<List<TestResponseModel>>(responses);

	// 5. CHECK FOR AI RESULT (The Fallback)
	// Assuming specific repository method exists or using FindAsync
	// Since AiTestResult ID is Shared PK with TestRequest:
	var aiResult = await _unitOfWork.AiTestResultRepository.GetByIdAsync(testRequestId);

	if (aiResult != null)
	{
		// Map AI Result to a "Fake" TestResponseModel so it fits in the list
		var aiResponseModel = new TestResponseModel
		{
			Id = 0, // Or some indicator that it's AI
			TestRequestId = testRequestId,
			ExpertId = 0, // 0 indicates System/AI
						  // You might want to fetch "AI" expert Name in frontend or hardcode logic here
						  // Note: ExpertName is not in TestResponseModel currently, but if you added it:
						  // ExpertName = "PerHue AI Assistant", 

			Note = aiResult.Note,
			CreatedDate = aiResult.Date,
			Rating = null, // User can't rate AI? Or maybe they can.
			BestColor = aiResult.SuggestedColor,
			WorstColor = aiResult.AvoidedColor,
			ColorTypeId = aiResult.ColorTypeId,
			ColorTypeName = aiResult.ColorType?.Name ?? "Unknown" // Ensure ColorType is included in repo query
		};

		// If ColorType was null in aiResult, fetch name manually
		if (aiResult.ColorType == null && aiResult.ColorTypeId > 0)
		{
			var ct = await _unitOfWork.ColorTypeRepository.GetByIdAsync(aiResult.ColorTypeId);
			aiResponseModel.ColorTypeName = ct?.Name ?? "Unknown";
		}

		responseModels.Add(aiResponseModel);
	}

	// 6. Build Final Result
	var result = new ExpertTestResultModel
	{
		TestRequest = _mapper.Map<TestRequestModel>(testRequest),
		Responses = responseModels
	};

	return result;
}*/