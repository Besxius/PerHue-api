using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IAiTestResultRepository : IGenericRepository<AiTestResult>
	{
		Task<TestRequest> CreateTestRequestAsync(TestRequest testRequest);
		Task<TestRequest?> GetTestRequestByIdAsync(int id);
		Task<List<TestRequest>> GetTestRequestsByUserIdAsync(int userId);
		Task UpdateTestRequestAsync(TestRequest testRequest);
		Task<AiTestResult> CreateAiTestResultAsync(AiTestResult result);
		Task<List<AiPicture>> CreateAiPicturesAsync(List<AiPicture> pictures);
		Task<(List<TestRequest> tests, int totalCount)> GetFilteredTestRequestsAsync(
			int pageIndex, int pageSize, int? userId, string? status, string? typeOfTest, string? fullname,
			DateTime? startDate, DateTime? endDate, int? sortBy, int? sortOrder);

		Task CreatePicturesAsync(List<Picture> pictures);

		Task<string?> GetPictureUrlByTestRequestIdAsync(int requestId);
	}
}