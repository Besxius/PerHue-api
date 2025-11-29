using PerHue.Application.Models;
using PerHue.Application.Models.Dashboard;

namespace PerHue.Application.IServices
{
    public interface IAdminDashboardService
    {
        // Dashboard Overview
        Task<DashboardMetricsModel> GetDashboardMetricsAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Account Management
        Task<AccountCountModel> GetAccountCountsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<PaginatedResultV2<AccountListItemModel>> GetAccountListAsync(AccountSearchModel searchModel);
        Task<AccountDetailModel?> GetAccountDetailAsync(int accountId);

        // Expert Management
        Task<PaginatedResultV2<ExpertActivityModel>> GetExpertActivityAsync(ExpertActivitySearchModel searchModel);

        // Statistics
        Task<RevenueStatisticsModel> GetRevenueStatisticsAsync(DateTime startDate, DateTime endDate, string? groupBy = "day");
        Task<TestCountStatisticsModel> GetTestCountStatisticsAsync(DateTime startDate, DateTime endDate, string? groupBy = "day", string? testType = null);
    }
}