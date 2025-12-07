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
	internal class TestResponseRepository : GenericRepository<TestResponse>, ITestResponseRepository
	{
		public TestResponseRepository(PerHueDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<TestResponse>> GetResponsesForRequestAsync(int testRequestId)
		{
			return await _context.TestResponses
				.Include(r => r.ColorType) // Include ColorType to map the name
				.Where(resp => resp.TestRequestId == testRequestId)
				.ToListAsync();
		}
		public async Task<IEnumerable<TestResponse>> GetAllByExpertIdAsync(int expertId)
		{
			return await _context.TestResponses
				.Where(r => r.ExpertId == expertId && r.Rating != null) // Only get rated responses
				.ToListAsync();
		}
		public async Task<IEnumerable<TestResponse>> GetRatedResponsesByExpertIdAsync(int expertId, DateTime? startDate, DateTime? endDate)
		{
			var query = _context.TestResponses
				.Where(r => r.ExpertId == expertId && r.Rating != null);

			if (startDate.HasValue)
			{
				query = query.Where(r => r.CreatedDate >= startDate.Value);
			}

			if (endDate.HasValue)
			{
				query = query.Where(r => r.CreatedDate <= endDate.Value);
			}

			return await query.OrderByDescending(r => r.CreatedDate).ToListAsync();
		}
		public async Task<TestResponse?> GetByRequestAndExpertAsync(int testRequestId, int expertId)
		{
			return await _context.TestResponses
				.FirstOrDefaultAsync(r => r.TestRequestId == testRequestId && r.ExpertId == expertId);
		}

	}
}