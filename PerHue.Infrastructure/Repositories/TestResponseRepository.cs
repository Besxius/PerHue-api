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
				.Where(resp => resp.TestRequestId == testRequestId)
				.ToListAsync();
		}
	}
}