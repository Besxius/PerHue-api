using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models;
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
	}
}