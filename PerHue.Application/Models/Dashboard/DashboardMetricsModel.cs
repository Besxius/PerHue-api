namespace PerHue.Application.Models.Dashboard
{
    /// <summary>
    /// High-level dashboard metrics model
    /// </summary>
    public class DashboardMetricsModel
    {
        public int TotalUsers { get; set; }
        public int TotalExperts { get; set; }
        public int ActiveUsers { get; set; }
        public int NewUsersToday { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueToday { get; set; }
        public int TotalTests { get; set; }
        public int TestsToday { get; set; }
        public int ExpertActivityCount { get; set; }
    }
}