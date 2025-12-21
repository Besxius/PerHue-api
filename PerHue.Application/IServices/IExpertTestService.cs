using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PerHue.Application.Models;
using PerHue.Application.Models.ExpertTestResult;

namespace PerHue.Application.IServices
{
	public interface IExpertTestService
	{
		Task<IEnumerable<TestRequestModel>> GetPendingRequestsAsync(int expertId);
		Task<IEnumerable<ExpertAssignmentModel>> GetAllRequestsAsync(int expertId);
		Task<TestResponseModel> SubmitResponseAsync(CreateTestResponseModel model, int expertId);

		//Task<IEnumerable<TestResponseModel>> GetExpertResponsesForUserAsync(int testRequestId, int userId);
		Task<ExpertTestResultModel> GetExpertResponsesForUserAsync(int testRequestId, int userId);

		Task<IEnumerable<ExpertTestResultModel>> GetAllCompletedExpertTestsAsync();

		Task<IEnumerable<TestRequestModel>> GetAllExpertTestRequestsAsync();

		Task<IEnumerable<TestRequestModel>> GetExpertTestRequestsByUserIdAsync(int userId);

		Task<PaginatedResult<ExpertTestResultModel>> GetMyCompletedExpertTestsAsync(int userId, int pageIndex, int pageSize, DateTime? fromDate, DateTime? toDate);
		Task RateExpertResponseAsync(RateExpertResponseModel model, int userId);
		Task<TestResponseModel> VoteForResponseAsync(VoteResponseModel model, int expertId);
		Task<IEnumerable<ReviewTestRequestModel>> GetPendingReviewRequestsAsync(int expertId);
		Task<IEnumerable<ReviewTestRequestModel>> GetCompletedReviewRequestsAsync(int expertId);

		Task<ExpertTestResultModel> GetExpertResponsesForExpertAsync(int testRequestId, int userId);
		Task<TestResponseModel> UpdateResponseAsync(int testRequestId, UpdateTestResponseModel model, int expertId);
		Task<ReviewTestRequestModel> GetPendingReviewRequestsByIdAsync(int expertId, int testRequestId);
		Task<IEnumerable<ExpertAssignmentModel>> GetReviewHistoryAsync(int expertId);
		Task<IEnumerable<ExpertAssignmentModel>> GetCompletedRequestsAsync(int expertId);
		Task<IEnumerable<ExpertAssignmentModel>> GetExpiredRequestsAsync(int expertId);
		Task<IEnumerable<ReviewTestRequestModel>> GetExpiredReviewRequestsAsync(int expertId);

	}
}
