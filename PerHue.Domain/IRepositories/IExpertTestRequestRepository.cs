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
	}
}