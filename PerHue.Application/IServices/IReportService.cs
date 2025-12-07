using PerHue.Application.Basic;
using PerHue.Application.Models;
using PerHue.Application.Models.Report;

namespace PerHue.Application.IServices
{
	public interface IReportService : IGenericService<ReportModel>
	{
		Task<ReportModel> CreateReportAsync(int userId, CreateReportModel model);
		Task<bool> UpdateReportAsync(int reportId, UpdateReportModel model);
		Task<IEnumerable<ReportModel>> GetReportsByUserIdAsync(int userId);
		Task<PaginatedResultV2<ReportModel>> GetAllReportsAsync(ReportSearchModel searchModel);
		Task<bool> UpdateReportStatusAsync(int reportId, string status, string? notice = null);
	}
}
