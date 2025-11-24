using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface ITestRequestRepository : IGenericRepository<TestRequest>
	{
		Task<TestRequest> GetByIdWithDetailsAsync(int id);
		Task<IEnumerable<TestRequest>> GetPendingRequestsAsync();

		Task<IEnumerable<TestRequest>> GetCompletedExpertTestsAsync();
		Task<(IEnumerable<TestRequest> Items, int TotalCount)> GetCompletedExpertTestsForUserAsync(int userId, int pageIndex, int pageSize, DateTime? date);

		Task<TestRequest> CreateTestRequestAsync(int userAccountId, string typeOfTest);
		Task<AiPicture> AddAiPictureAsync(int testRequestId, string imageUrl, string note);
		Task<AiTestResult> AddAiTestResultAsync(int testRequestId, string suggestedColor, string avoidedColor, int colorTypeId, string note);
	}
}