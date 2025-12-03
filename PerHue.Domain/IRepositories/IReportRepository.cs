using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IReportRepository : IGenericRepository<Report>
	{
		Task<IEnumerable<Report>> GetReportsByUserIdAsync(int userId);
		Task<IEnumerable<Report>> GetReportsByStatusAsync(string status);
		Task<IEnumerable<Report>> GetReportsByTypeAsync(string type);
		Task<bool> UpdateReportStatusAsync(int reportId, string status, string? notice = null);
	}
}
