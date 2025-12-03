using PerHue.Application.Basic;
using PerHue.Application.Models;
using PerHue.Application.Models.Payment;
using PerHue.Application.Models.UserSubscription;

namespace PerHue.Application.IServices
{
	public interface IUserSubscriptionService : IGenericService<UserSubscriptionModel>
	{
		Task<int> CreateAsync(CreateUserSubscriptionModel model);
		Task<UserSubscriptionModel> GetCurrentUserSubscriptionByUserIdAsync(int userId);
		Task<IEnumerable<UserSubscriptionModel>> GetHistoryUserSubscriptionsByUserIdAsync(int userId);
		Task UpdateStatusUserSubscriptionAsync(int id, string status);

		Task<bool> HasRemainingUsageAsync(int userId);
		Task<int> GetRemainingUsageAsync(int userId);
		Task<bool> DeductUsageAsync(int userId, int packageId, string type);
		Task<bool> RefundUsageAsync(int userId, int packageId, string type);
		Task<UserSubscriptionModel?> GetActiveSubscriptionAsync(int userId);

		Task<int> GetAllActiveRemainingUsageByUserIdAsync(int userId);
		Task<Dictionary<int, PackageUsageInfo>> GetRemainingUsageByPackageAsync(int userId);
		Task<List<PackageUsageSummary>> GetUsageSummaryAsync(int userId);

		// lấy danh sách gói theo user id
		/// <summary>
		/// Lấy tất cả subscriptions đang sử dụng của user tính đến thời điểm hiện tại
		/// </summary>
		Task<List<UserSubscriptionModel>> GetCurrentlyActiveSubscriptionsByUserIdAsync(int userId);

		/// <summary>
		/// Lấy tất cả subscriptions active của user
		/// </summary>
		Task<List<UserSubscriptionModel>> GetAllActiveSubscriptionsForUserAsync(int userId);

		/// <summary>
		/// Lấy tất cả subscriptions inactive của user
		/// </summary>
		Task<List<UserSubscriptionModel>> GetAllInactiveSubscriptionsForUserAsync(int userId);

		/// <summary>
		/// Lấy tất cả subscriptions đã đăng ký của user (cả active và inactive)
		/// </summary>
		Task<List<UserSubscriptionModel>> GetAllRegisteredSubscriptionsForUserAsync(int userId);

		/// <summary>
		/// Lấy subscriptions với phân trang và filter
		/// </summary>
		Task<PaginatedResultV2<UserSubscriptionModel>> GetUserSubscriptionsWithFilterAsync(
			int userId,
			int pageIndex,
			int pageSize,
			bool? status = null);
	}
}
