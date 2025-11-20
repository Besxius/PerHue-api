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
	}
}