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
		public async Task<(IEnumerable<TestRequest> Items, int TotalCount)> GetCompletedExpertTestsForUserAsync(int userId, int pageIndex, int pageSize, DateTime? date)
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
			if (date.HasValue)
			{
				// Compare only the Date part (ignoring time)
				query = query.Where(tr => tr.CreatedDate.HasValue && tr.CreatedDate.Value.Date == date.Value.Date);
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
	}
}