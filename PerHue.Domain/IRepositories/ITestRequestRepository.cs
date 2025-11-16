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

		Task<TestRequest> CreateTestRequestAsync(int userAccountId, string typeOfTest);
		Task<AiPicture> AddAiPictureAsync(int testRequestId, string imageUrl, string note);
		Task<AiTestResult> AddAiTestResultAsync(int testRequestId, string suggestedColor, string avoidedColor, int colorTypeId, string note);

	}
}