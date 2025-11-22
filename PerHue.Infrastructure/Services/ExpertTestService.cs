using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.ExpertTestResult;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;

namespace PerHue.Infrastructure.Services
{
	internal class ExpertTestService : IExpertTestService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ExpertTestService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<IEnumerable<TestRequestModel>> GetPendingRequestsAsync(int expertId)
		{
			var expertRequests = await _unitOfWork.ExpertTestRequestRepository.GetPendingRequestsForExpertAsync(expertId);

			// Map the underlying TestRequest to TestRequestModel
			var testRequests = expertRequests.Select(etr => etr.TestRequest);
			return _mapper.Map<IEnumerable<TestRequestModel>>(testRequests);
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

			// 3. Save the response
			await _unitOfWork.TestResponseRepository.CreateAsync(testResponse);

			// 4. Update the ExpertTestRequest status to "Completed"
			pendingRequest.Status = "Completed";
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
			if (testRequest.Status != "Completed") throw new Exception("Test is still being processed.");

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
		public async Task<PaginatedResult<ExpertTestResultModel>> GetMyCompletedExpertTestsAsync(int userId, int pageIndex, int pageSize, DateTime? date)
		{
			// 1. Get Paged Test Requests
			var (testRequests, totalCount) = await _unitOfWork.TestRequestRepository.GetCompletedExpertTestsForUserAsync(userId, pageIndex, pageSize, date);

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
		public async Task RateExpertResponseAsync(RateExpertResponseModel model, int userId)
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