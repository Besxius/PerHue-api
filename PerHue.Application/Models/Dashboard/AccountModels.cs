using PerHue.Application.Models;

namespace PerHue.Application.Models.Dashboard
{
    /// <summary>
    /// Account search and filter model
    /// </summary>
    public class AccountSearchModel : BaseSearchModel
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Fullname { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsBanned { get; set; }
        public int? RoleId { get; set; }
        public bool? IsAiTested { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public AccountSortBy SortBy { get; set; } = AccountSortBy.CreatedDate;
    }

    /// <summary>
    /// Account list item model for listing
    /// </summary>
    public class AccountListItemModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Fullname { get; set; }
        public bool IsActive { get; set; }
        public bool IsBanned { get; set; }
        public string RoleName { get; set; } = null!;
        public bool IsAiTested { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int TestCount { get; set; }
        public decimal TotalSpent { get; set; }
    }

    /// <summary>
    /// Account detail model
    /// </summary>
    public class AccountDetailModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public bool Gender { get; set; }
        public DateOnly? Dob { get; set; }
        public bool IsActive { get; set; }
        public bool IsBanned { get; set; }
        public string? BanReason { get; set; }
        public DateTime? BannedDate { get; set; }
        public string? ProfilePicture { get; set; }
        public bool IsAiTested { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // Statistics
        public int TotalTests { get; set; }
        public int CompletedTests { get; set; }
        public decimal TotalSpent { get; set; }
        public int SubscriptionCount { get; set; }
        public List<AccountActivityModel> RecentActivity { get; set; } = new();
        public List<AccountSubscriptionModel> Subscriptions { get; set; } = new();
    }

    /// <summary>
    /// Account activity model
    /// </summary>
    public class AccountActivityModel
    {
        public string Activity { get; set; } = null!;
        public DateTime Date { get; set; }
        public string? Details { get; set; }
    }

    /// <summary>
    /// Account subscription model
    /// </summary>
    public class AccountSubscriptionModel
    {
        public int Id { get; set; }
        public string PackageName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// Account sort options
    /// </summary>
    public enum AccountSortBy
    {
        CreatedDate,
        LastLoginDate,
        Email,
        Username,
        TotalSpent,
        TestCount
    }
}