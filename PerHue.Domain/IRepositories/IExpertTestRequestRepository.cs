using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IExpertTestRequestRepository : IGenericRepository<ExpertTestRequest>
	{
		Task<IEnumerable<ExpertTestRequest>> GetPendingRequestsForExpertAsync(int expertId);
		Task<IEnumerable<ExpertTestRequest>> GetRequestsByTestIdAsync(int testRequestId);
		Task<ExpertTestRequest> GetPendingRequestAsync(int expertId, int testRequestId);

		// Fetches requests specifically waiting for "Review"
		Task<IEnumerable<ExpertTestRequest>> GetPendingReviewRequestsForExpertAsync(int expertId);
		Task<ExpertTestRequest> GetPendingReviewRequestAsync(int expertId, int testRequestId);
		Task<IEnumerable<ExpertTestRequest>> GetAllRequestsForExpertAsync(int expertId);

	}
}