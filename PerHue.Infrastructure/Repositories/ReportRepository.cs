using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class ReportRepository : GenericRepository<Report>, IReportRepository
	{
		public ReportRepository(PerHueDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Report>> GetReportsByUserIdAsync(int userId)
		{
			return await _context.Reports
				.Include(r => r.UserAccount)
				.Where(r => r.UserAccountId == userId)
				.OrderByDescending(r => r.Id)
				.ToListAsync();
		}

		public async Task<IEnumerable<Report>> GetReportsByStatusAsync(string status)
		{
			return await _context.Reports
				.Include(r => r.UserAccount)
				.Where(r => r.Status == status)
				.OrderByDescending(r => r.Id)
				.ToListAsync();
		}

		public async Task<IEnumerable<Report>> GetReportsByTypeAsync(string type)
		{
			return await _context.Reports
				.Include(r => r.UserAccount)
				.Where(r => r.Type == type)
				.OrderByDescending(r => r.Id)
				.ToListAsync();
		}

		public async Task<bool> UpdateReportStatusAsync(int reportId, string status, string? notice = null)
		{
			var report = await _context.Reports.FindAsync(reportId);
			if (report == null)
				return false;

			report.Status = status;
			if (notice != null)
				report.Notice = notice;

			await _context.SaveChangesAsync();
			return true;
		}
	}
}
