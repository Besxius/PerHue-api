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
		public async Task<IEnumerable<TestResponseModel>> GetExpertResponsesForUserAsync(int testRequestId, int userId)
		{
			// 1. Get the main test request
			var testRequest = await _unitOfWork.TestRequestRepository.GetByIdAsync(testRequestId);

			if (testRequest == null)
			{
				throw new Exception("Test request not found.");
			}

			// 2. Security Check: Ensure the user asking for the test is the one who created it
			if (testRequest.UserAccountId != userId)
			{
				throw new UnauthorizedAccessException("You are not authorized to view this test result.");
			}

			// 3. Check if the test is actually finished
			if (testRequest.Status != "Completed")
			{
				throw new Exception("Your test is still being processed by our experts.");
			}

			// 4. Get the individual responses
			var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(testRequestId);

			return _mapper.Map<IEnumerable<TestResponseModel>>(responses);
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
		public async Task<IEnumerable<ExpertTestResultModel>> GetMyCompletedExpertTestsAsync(int userId)
		{
			var completedTests = await _unitOfWork.TestRequestRepository.GetCompletedExpertTestsForUserAsync(userId);
			var results = new List<ExpertTestResultModel>();

			foreach (var test in completedTests)
			{
				// We only fetch responses for tests that actually completed (not failed)
				IEnumerable<TestResponseModel> mappedResponses = new List<TestResponseModel>();
				if (test.Status == "Completed")
				{
					var responses = await _unitOfWork.TestResponseRepository.GetResponsesForRequestAsync(test.Id);
					mappedResponses = _mapper.Map<IEnumerable<TestResponseModel>>(responses);
				}

				results.Add(new ExpertTestResultModel
				{
					TestRequest = _mapper.Map<TestRequestModel>(test),
					Responses = mappedResponses
				});
			}

			return results;
		}
	}
}