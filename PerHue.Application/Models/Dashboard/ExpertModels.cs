using PerHue.Application.Models;

namespace PerHue.Application.Models.Dashboard
{
    /// <summary>
    /// Expert penalty search model
    /// </summary>
    public class ExpertPenaltySearchModel : BaseSearchModel
    {
        public string? ExpertName { get; set; }
        public string? ExpertEmail { get; set; }
        public string? Reason { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public PenaltySortBy SortBy { get; set; } = PenaltySortBy.CreatedDate;
    }

    /// <summary>
    /// Expert penalty model
    /// </summary>
    public class ExpertPenaltyModel
    {
        public int Id { get; set; }
        public int ExpertId { get; set; }
        public string ExpertName { get; set; } = null!;
        public string ExpertEmail { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Expert activity search model
    /// </summary>
    public class ExpertActivitySearchModel : BaseSearchModel
    {
        public string? ExpertName { get; set; }
        public string? ExpertEmail { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ExpertActivitySortBy SortBy { get; set; } = ExpertActivitySortBy.Date;
    }

    /// <summary>
    /// Expert activity model
    /// </summary>
    public class ExpertActivityModel
    {
        public int ExpertId { get; set; }
        public string ExpertName { get; set; } = null!;
        public string ExpertEmail { get; set; } = null!;
        public string? ProfilePicture { get; set; }
        public int TestsCompleted { get; set; }
        public int TestsInProgress { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal AverageRating { get; set; }
        public DateTime LastActiveDate { get; set; }
        public bool IsActive { get; set; }
        public List<ExpertRecentActivityModel> RecentActivities { get; set; } = new();
    }

    /// <summary>
    /// Expert recent activity model
    /// </summary>
    public class ExpertRecentActivityModel
    {
        public string Activity { get; set; } = null!;
        public DateTime Date { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Penalty sort options
    /// </summary>
    public enum PenaltySortBy
    {
        CreatedDate,
        Amount,
        ExpertName
    }

    /// <summary>
    /// Expert activity sort options
    /// </summary>
    public enum ExpertActivitySortBy
    {
        Date,
        ExpertName,
        TestsCompleted,
        TotalEarnings
    }
}