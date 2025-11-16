using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Repositories
{
	public class AiTestResultRepository : GenericRepository<AiTestResult>, IAiTestResultRepository
	{
		private readonly PerHueDbContext _dbContext;
		public AiTestResultRepository(PerHueDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<TestRequest> CreateTestRequestAsync(TestRequest testRequest)
		{
			await _context.TestRequests.AddAsync(testRequest);
			await _context.SaveChangesAsync();
			return testRequest;
		}

		public async Task<TestRequest?> GetTestRequestByIdAsync(int id)
		{
			return await _context.TestRequests
				.Include(t => t.AiTestResult)
					.ThenInclude(r => r.ColorType)
				.Include(t => t.AiPictures)
				.Include(t => t.UserAccount)
				.FirstOrDefaultAsync(t => t.Id == id);
		}

		public async Task<List<TestRequest>> GetTestRequestsByUserIdAsync(int userId)
		{
			return await _context.TestRequests
				.Include(t => t.AiTestResult)
					.ThenInclude(r => r.ColorType)
				.Include(t => t.AiPictures)
				.Where(t => t.UserAccountId == userId && t.TypeOfTest == "AI Test")
				.OrderByDescending(t => t.CreatedDate)
				.ToListAsync();
		}

		public async Task UpdateTestRequestAsync(TestRequest testRequest)
		{
			_context.TestRequests.Update(testRequest);
			await _context.SaveChangesAsync();
		}

		public async Task<AiTestResult> CreateAiTestResultAsync(AiTestResult result)
		{
			await _context.AiTestResults.AddAsync(result);
			await _context.SaveChangesAsync();
			return result;
		}

		public async Task<List<AiPicture>> CreateAiPicturesAsync(List<AiPicture> pictures)
		{
			await _context.AiPictures.AddRangeAsync(pictures);
			await _context.SaveChangesAsync();
			return pictures;
		}
	}
}