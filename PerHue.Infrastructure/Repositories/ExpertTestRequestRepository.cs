using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class ExpertTestRequestRepository : GenericRepository<ExpertTestRequest>, IExpertTestRequestRepository
	{
		public ExpertTestRequestRepository(PerHueDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<ExpertTestRequest>> GetPendingRequestsForExpertAsync(int expertId)
		{
			return await _context.ExpertTestRequests
				.Include(etr => etr.TestRequest)
					.ThenInclude(tr => tr.AiPictures)
				.Where(etr => etr.ExpertId == expertId && etr.Status == "Pending")
				.ToListAsync();
		}

		public async Task<IEnumerable<ExpertTestRequest>> GetRequestsByTestIdAsync(int testRequestId)
		{
			return await _context.ExpertTestRequests
				.Where(etr => etr.TestRequestId == testRequestId)
				.ToListAsync();
		}

		public async Task<ExpertTestRequest> GetPendingRequestAsync(int expertId, int testRequestId)
		{
			return await _context.ExpertTestRequests
				.FirstOrDefaultAsync(etr => etr.ExpertId == expertId
										&& etr.TestRequestId == testRequestId
										&& etr.Status == "Pending");
		}
	}
}