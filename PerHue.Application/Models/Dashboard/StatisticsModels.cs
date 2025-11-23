namespace PerHue.Application.Models.Dashboard
{
    /// <summary>
    /// Revenue statistics model
    /// </summary>
    public class RevenueStatisticsModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal PreviousPeriodRevenue { get; set; }
        public decimal GrowthPercentage { get; set; }
        public List<RevenueDataPoint> RevenueData { get; set; } = new();
        public List<RevenueBySourceModel> RevenueBySource { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string GroupBy { get; set; } = null!;
    }

    /// <summary>
    /// Revenue data point model
    /// </summary>
    public class RevenueDataPoint
    {
        public string Period { get; set; } = null!;
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
        public DateTime Date { get; set; }
    }

    /// <summary>
    /// Revenue by source model
    /// </summary>
    public class RevenueBySourceModel
    {
        public string Source { get; set; } = null!;
        public decimal Amount { get; set; }
        public double Percentage { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// Test count statistics model
    /// </summary>
    public class TestCountStatisticsModel
    {
        public int TotalTests { get; set; }
        public int PreviousPeriodTests { get; set; }
        public double GrowthPercentage { get; set; }
        public List<TestCountDataPoint> TestData { get; set; } = new();
        public List<TestByTypeModel> TestsByType { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string GroupBy { get; set; } = null!;
    }

    /// <summary>
    /// Test count data point model
    /// </summary>
    public class TestCountDataPoint
    {
        public string Period { get; set; } = null!;
        public int Count { get; set; }
        public int CompletedCount { get; set; }
        public int InProgressCount { get; set; }
        public DateTime Date { get; set; }
    }

    /// <summary>
    /// Test by type model
    /// </summary>
    public class TestByTypeModel
    {
        public string TestType { get; set; } = null!;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    /// <summary>
    /// Penalty statistics model
    /// </summary>
    public class PenaltyStatisticsModel
    {
        public int TotalPenalties { get; set; }
        public decimal TotalPenaltyAmount { get; set; }
        public int PreviousPeriodPenalties { get; set; }
        public decimal PreviousPeriodAmount { get; set; }
        public double CountGrowthPercentage { get; set; }
        public double AmountGrowthPercentage { get; set; }
        public List<PenaltyDataPoint> PenaltyData { get; set; } = new();
        public List<PenaltyByReasonModel> PenaltiesByReason { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string GroupBy { get; set; } = null!;
    }

    /// <summary>
    /// Penalty data point model
    /// </summary>
    public class PenaltyDataPoint
    {
        public string Period { get; set; } = null!;
        public int Count { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

    /// <summary>
    /// Penalty by reason model
    /// </summary>
    public class PenaltyByReasonModel
    {
        public string Reason { get; set; } = null!;
        public int Count { get; set; }
        public decimal Amount { get; set; }
        public double Percentage { get; set; }
    }
}