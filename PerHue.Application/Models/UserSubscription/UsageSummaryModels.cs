namespace PerHue.Application.Models.UserSubscription
{
	/// <summary>
	/// Tổng kết lượt sử dụng theo từng package
	/// Đếm tất cả subscription có Status = true
	/// </summary>
	public class PackageUsageSummary
	{
		public int PackageId { get; set; }
		public string PackageName { get; set; } = string.Empty;
		public int TotalPurchased { get; set; }
		public int TotalUsed { get; set; }
		public int TotalRemaining { get; set; }
		public int ActiveSubscriptionCount { get; set; }
		public DateTime? EarliestExpiry { get; set; }
		public DateTime? LatestExpiry { get; set; }
	}

	public class PackageUsageInfo
	{
		public int PackageId { get; set; }
		public string PackageName { get; set; } = string.Empty;
		public int TotalRemaining { get; set; }
		public int SubscriptionCount { get; set; }
	}

	/// <summary>
	/// Chi tiết từng subscription
	/// </summary>
	public class SubscriptionDetail
	{
		public int SubscriptionId { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public short RemainingUses { get; set; }
		public bool IsExpired { get; set; }
	}

	/// <summary>
	/// Response cho API usage summary
	/// </summary>
	public class UsageSummaryResponse
	{
		public int UserId { get; set; }
		public int TotalRemainingUses { get; set; }
		public bool HasActiveSubscription { get; set; }
		public List<PackageUsageSummary> PackageBreakdown { get; set; } = new();
	}
}