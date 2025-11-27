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
				.Include(tr => tr.Pictures)
				.Include(tr => tr.AiTestResult)
					.ThenInclude(atr => atr.ColorType)
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

		public async Task<TestRequest> CreateTestRequestAsync(int userAccountId, string typeOfTest)
		{
			var testRequest = new TestRequest
			{
				UserAccountId = userAccountId,
				TypeOfTest = typeOfTest,
				CreatedDate = DateTime.Now,
				Status = "Pending"
			};

			_context.TestRequests.Add(testRequest);
			await _context.SaveChangesAsync();
			return testRequest;
		}

		public async Task<AiPicture> AddAiPictureAsync(int testRequestId, string imageUrl, string note)
		{
			var aiPicture = new AiPicture
			{
				TestRequestId = testRequestId,
				Source = imageUrl,
				Note = note
			};

			_context.AiPictures.Add(aiPicture);
			await _context.SaveChangesAsync();
			return aiPicture;
		}

		public async Task<AiTestResult> AddAiTestResultAsync(int testRequestId, string suggestedColor, string avoidedColor, int colorTypeId, string note)
		{
			var testRequest = await _context.TestRequests.FindAsync(testRequestId);
			if (testRequest == null)
			{
				throw new ArgumentException($"TestRequest with id {testRequestId} not found.");
			}

			var aiTestResult = new AiTestResult
			{
				//IdNavigation = testRequestId //loi testRequéstId la int khong phai TestRequest
				IdNavigation = testRequest, // FIX: assign the TestRequest entity, not the int
				SuggestedColor = suggestedColor,
				AvoidedColor = avoidedColor,
				ColorTypeId = colorTypeId,
				Note = note,
				Date = DateTime.Now
			};

			_context.AiTestResults.Add(aiTestResult);
			await _context.SaveChangesAsync();
			return aiTestResult;
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
		public async Task<(IEnumerable<TestRequest> Items, int TotalCount)> GetCompletedExpertTestsForUserAsync(int userId, int pageIndex, int pageSize, DateTime? fromDate, DateTime? toDate)
		{
			var query = _context.TestRequests
				.Include(tr => tr.UserAccount)
				.Include(tr => tr.AiPictures)   
				.Include(tr => tr.Pictures)
				.Include(tr => tr.AiTestResult)         
					.ThenInclude(atr => atr.ColorType)
				.Where(tr => tr.UserAccountId == userId &&
							 tr.Status == "Completed" && 
							 tr.TypeOfTest == "Expert");

			// Apply Date Filter if provided
			if (fromDate.HasValue)
			{
				query = query.Where(tr => tr.CreatedDate.HasValue && tr.CreatedDate.Value.Date >= fromDate.Value.Date);
			}

			if (toDate.HasValue)
			{
				query = query.Where(tr => tr.CreatedDate.HasValue && tr.CreatedDate.Value.Date <= toDate.Value.Date);
			}

			// Get Total Count for Pagination
			var totalCount = await query.CountAsync();

			// Get Paged Data
			var items = await query
				.OrderByDescending(tr => tr.CreatedDate)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalCount);
		}


		//==========================================


		public async Task<List<TestRequest>> GetByUserIdAsync(int userId)
		{
			return await _dbSet
				.Where(t => t.UserAccountId == userId)
				.OrderByDescending(t => t.CreatedDate)
				.ToListAsync();
		}

		public async Task<List<TestRequest>> GetByUserIdWithDetailsAsync(int userId)
		{
			return await _dbSet
				.Include(t => t.UserAccount)
				.Include(t => t.AiTestResult)
					.ThenInclude(r => r.ColorType)
				.Include(t => t.AiPictures)
				.Where(t => t.UserAccountId == userId)
				.OrderByDescending(t => t.CreatedDate)
				.ToListAsync();
		}

		public async Task<List<TestRequest>> GetByStatusAsync(string status)
		{
			return await _dbSet
				.Include(t => t.UserAccount)
				.Where(t => t.Status == status)
				.OrderByDescending(t => t.CreatedDate)
				.ToListAsync();
		}

		public async Task<List<TestRequest>> GetByTypeAsync(string typeOfTest)
		{
			return await _dbSet
				.Include(t => t.UserAccount)
				.Where(t => t.TypeOfTest == typeOfTest)
				.OrderByDescending(t => t.CreatedDate)
				.ToListAsync();
		}

		public async Task<int> CountByUserIdAsync(int userId)
		{
			return await _dbSet
				.Where(t => t.UserAccountId == userId)
				.CountAsync();
		}

		public async Task<List<TestRequest>> GetPendingTestsByUserIdAsync(int userId)
		{
			return await _dbSet
				.Include(t => t.UserAccount)
				.Where(t => t.UserAccountId == userId && t.Status == "Pending")
				.OrderByDescending(t => t.CreatedDate)
				.ToListAsync();
		}

		public async Task<IEnumerable<TestRequest>> GetAllAsync()
		{
			return await _dbSet
				.Include(t => t.UserAccount)
				.OrderByDescending(t => t.CreatedDate)
				.ToListAsync();
		}

	}
}