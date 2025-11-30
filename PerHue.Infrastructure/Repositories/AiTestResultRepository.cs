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
		public AiTestResultRepository(PerHueDbContext context) : base(context)
		{
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
				.Where(t => t.UserAccountId == userId && t.TypeOfTest == "AI")
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

		public async Task<(List<TestRequest> tests, int totalCount)> GetFilteredTestRequestsAsync(
			int pageIndex, int pageSize, int? userId, string? status, string? typeOfTest, string? fullname,
			DateTime? startDate, DateTime? endDate, int? sortBy, int? sortOrder)
		{
			var query = _context.TestRequests
				.Include(t => t.AiTestResult)
					.ThenInclude(r => r.ColorType)
				.Include(t => t.AiPictures)
				.Include(t => t.UserAccount)
				.AsQueryable();

			// Apply filters
			if (userId.HasValue)
			{
				query = query.Where(t => t.UserAccountId == userId.Value);
			}

			if (!string.IsNullOrEmpty(status))
			{
				query = query.Where(t => t.Status.Contains(status));
			}

			if (!string.IsNullOrEmpty(typeOfTest))
			{
				query = query.Where(t => t.TypeOfTest.Contains(typeOfTest));
			}

			if (!string.IsNullOrEmpty(fullname))
			{
				query = query.Where(t => t.UserAccount.Fullname.Contains(fullname));
			}

			if (startDate.HasValue)
			{
				query = query.Where(t => t.CreatedDate >= startDate.Value);
			}

			if (endDate.HasValue)
			{
				query = query.Where(t => t.CreatedDate <= endDate.Value);
			}

			// Get total count before pagination
			var totalCount = await query.CountAsync();

			// Apply sorting
			if (sortBy.HasValue && sortOrder.HasValue)
			{
				switch (sortBy.Value)
				{
					case 2: // Status
						query = sortOrder.Value == 1 // Ascending
							? query.OrderBy(t => t.Status)
							: query.OrderByDescending(t => t.Status);
						break;
					case 3: // UserId
						query = sortOrder.Value == 1
							? query.OrderBy(t => t.UserAccountId)
							: query.OrderByDescending(t => t.UserAccountId);
						break;
					case 4: // TestRequestId
						query = sortOrder.Value == 1
							? query.OrderBy(t => t.Id)
							: query.OrderByDescending(t => t.Id);
						break;
					case 1: // CreatedDate
					default:
						query = sortOrder.Value == 1
							? query.OrderBy(t => t.CreatedDate)
							: query.OrderByDescending(t => t.CreatedDate);
						break;
				}
			}
			else
			{
				// Default sorting by CreatedDate descending
				query = query.OrderByDescending(t => t.CreatedDate);
			}

			// Apply pagination
			var tests = await query
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (tests, totalCount);
		}

		public async Task CreatePicturesAsync(List<Picture> pictures)
		{
			await _context.Pictures.AddRangeAsync(pictures);
			await _context.SaveChangesAsync();
		}
	}
}