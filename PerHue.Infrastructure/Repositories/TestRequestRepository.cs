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
	internal class TestRequestRepository : GenericRepository<TestRequest>, ITestRequestRepository
	{
		public TestRequestRepository(PerHueDbContext context) : base(context)
		{
		}

		public async Task<TestRequest> GetByIdWithDetailsAsync(int id)
		{
			return await _context.TestRequests
				.Include(tr => tr.UserAccount)
				.Include(tr => tr.AiPictures)
				.FirstOrDefaultAsync(tr => tr.Id == id);
		}

		public async Task<IEnumerable<TestRequest>> GetPendingRequestsAsync()
		{
			// "Pending" means waiting for expert responses
			return await _context.TestRequests
				.Include(tr => tr.UserAccount)
				.Include(tr => tr.ExpertTestRequests)
				.Where(tr => tr.Status == "Pending")
				.ToListAsync();
		}
	}
}