using PerHue.Domain.Basic;
using PerHue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Domain.IRepositories
{
	public interface IUserSubscriptionRepository : IGenericRepository<UserSubscription>
	{
		Task<UserSubscription> GetCurrentUserSubscriptionByUserIdAsync(int userId);
		Task<IEnumerable<UserSubscription>> GetHistoryUserSubscriptionsByUserIdAsync(int userId);
		Task<UserSubscription?> GetActiveByUserIdAsync(int userId);
		Task<UserSubscription?> GetSubscriptionForRefundAsync(int userId);

		Task<UserSubscription?> GetActiveSubscriptionAsync(int userId);
		Task<UserSubscription?> GetActiveSubscriptionByTypeAsync(int userId, string type);
		Task<bool> HasActiveSubscriptionWithRemainingUsesAsync(int userId, string? type = null);
		Task<UserSubscription?> GetLatestActiveSubscriptionByPackageAndTypeAsync(int userId, int packageId, string type);
		Task<bool> DeductRemainingUsesAsync(int userId, int packageId, string type);
		Task<bool> RefundRemainingUsesAsync(int userId, int packageId, string type);

		Task<List<UserSubscription>> GetaAllActiveSubscriptionsByUserIdAsync(int userId);

		// Đếm tổng lượt sử dụng còn lại theo PackageId
		Task<Dictionary<int, int>> GetTotalRemainingUsesByPackageAndUserAsync(int userId);

		Task<List<UserSubscription>> GetAllSubscriptionsWithPackageByUserIdAsync(int userId);

		Task<int> AutoExpireSubscriptionsAsync();

		Task<UserSubscription?> GetActiveSubscriptionByPackageIdAsync(int userId, int servicePackageId);
		Task<bool> DisableSubscriptionAsync(int subscriptionId);
		Task<int> GetTotalRemainingUsagesByUserIdAsync(int userId);

		Task<UserSubscription> FindSameTypeSubscriptionIsActiveOrNot(int userId, string type);


		// lấy gói theo user id
		/// <summary>
		/// Lấy tất cả subscriptions đang sử dụng của user tính đến thời điểm hiện tại
		/// </summary>
		Task<List<UserSubscription>> GetCurrentlyActiveSubscriptionsByUserIdAsync(int userId);

		/// <summary>
		/// Lấy tất cả subscriptions đã đăng ký của user (chỉ active)
		/// </summary>
		Task<List<UserSubscription>> GetAllActiveSubscriptionsByUserIdAsync(int userId);

		/// <summary>
		/// Lấy tất cả subscriptions đã đăng ký của user (chỉ inactive)
		/// </summary>
		Task<List<UserSubscription>> GetAllInactiveSubscriptionsByUserIdAsync(int userId);

		/// <summary>
		/// Lấy tất cả subscriptions đã đăng ký của user (cả active và inactive)
		/// </summary>
		Task<List<UserSubscription>> GetAllRegisteredSubscriptionsByUserIdAsync(int userId);

		/// <summary>
		/// Lấy tất cả subscriptions với phân trang và filter theo status
		/// </summary>
		Task<(List<UserSubscription> subscriptions, int totalCount)> GetUserSubscriptionsWithPaginationAsync(
			int userId,
			int pageIndex,
			int pageSize,
			bool? status = null);

	}
}
