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
			return await _context.TestResults.Include(p => p.User).ToListAsync();
		}

		public async Task<TestResult> GetByTestResultIdAsync(int id)
		{
			return await _context.TestResults
				.Include(p => p.User)
				.Include(tr => tr.ColorType)
				.ThenInclude(ct => ct.CapsulePalettes)
				.ThenInclude(cp => cp.Colors)
				.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<TestResult> GetByIdAsync(int id)
		{
			return await _context.TestResults.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<IEnumerable<TestResult>> GetAllByUserIdAsync(int userId)
		{
			return await _context.TestResults
				.Include(tr => tr.User)
				.Include(tr => tr.ColorType)
				.ThenInclude(ct => ct.CapsulePalettes)
				.ThenInclude(cp => cp.Colors)
				.Where(tr => tr.UserId == userId)
				.ToListAsync();
		}

		public async Task<IEnumerable<TestResult>> GetTestResultListAllWithUserAndColorTypeAsync()
		{
			return await _context.TestResults
				.Include(tr => tr.User)
				.Include(tr => tr.ColorType)
				.ToListAsync();
		}

		public async Task<TestResult> GetTestResultDetailByIdWithUserAndColorTypeAsync(int id)
		{
			return await _context.TestResults
				.Include(tr => tr.User)
				.Include(tr => tr.ColorType)
				.FirstOrDefaultAsync(tr => tr.Id == id);
		}
	}
}
