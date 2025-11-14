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
		Task<TestResponseModel> SubmitResponseAsync(CreateTestResponseModel model, int expertId);

		Task<IEnumerable<TestResponseModel>> GetExpertResponsesForUserAsync(int testRequestId, int userId);

		Task<IEnumerable<ExpertTestResultModel>> GetAllCompletedExpertTestsAsync();
		Task<IEnumerable<ExpertTestResultModel>> GetMyCompletedExpertTestsAsync(int userId);
	}
}