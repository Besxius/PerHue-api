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

		public async Task<IEnumerable<TestRequest>> GetCompletedExpertTestsAsync()
		{
			return await _context.TestRequests
				.Include(tr => tr.UserAccount) // For mapping to TestRequestModel
				.Include(tr => tr.AiPictures)   // For mapping to TestRequestModel
				.Where(tr => tr.Status == "Completed" && tr.TypeOfTest == "Expert")
				.OrderByDescending(tr => tr.CreatedDate)
				.ToListAsync();
		}
		public async Task<IEnumerable<TestRequest>> GetCompletedExpertTestsForUserAsync(int userId)
		{
			return await _context.TestRequests
				.Include(tr => tr.UserAccount)
				.Include(tr => tr.AiPictures)
				// We check for both "Completed" and "Failed" as they are both part of the user's history
				.Where(tr => tr.UserAccountId == userId &&
							 (tr.Status == "Completed" || tr.Status == "Failed") &&
							 tr.TypeOfTest == "Expert")
				.OrderByDescending(tr => tr.CreatedDate)
				.ToListAsync();
		}
	}
}