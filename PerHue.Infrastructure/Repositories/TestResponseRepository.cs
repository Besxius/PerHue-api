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

	}
}