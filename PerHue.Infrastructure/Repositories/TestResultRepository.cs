using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class TestResultRepository : GenericRepository<TestResult>, ITestResultRepository
	{
		public TestResultRepository(PerHueDbContext context) : base(context)
		{

		}

		public async Task<IEnumerable<TestResult>> GetAllAsync()
		{
			return await _context.TestResults.Include(p => p.ColorType).Include(p => p.User).ToListAsync();
		}

		public async Task<TestResult> GetByIdAsync(int id)
		{
			return await _context.TestResults.Include(p => p.ColorType).Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
		}
	}
}
