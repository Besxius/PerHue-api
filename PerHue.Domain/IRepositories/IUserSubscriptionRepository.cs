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
		Task<bool> HasActiveSubscriptionWithRemainingUsesAsync(int userId);
		Task<bool> DeductRemainingUsesAsync(int userId);
		Task<bool> RefundRemainingUsesAsync(int userId);

		Task<List<UserSubscription>> GetaAllActiveSubscriptionsByUserIdAsync(int userId);

		// Đếm tổng lượt sử dụng còn lại theo PackageId
		Task<Dictionary<int, int>> GetTotalRemainingUsesByPackageAndUserAsync(int userId);

		Task<bool> HasRemainingUsageAsync(int userId);

		Task<List<UserSubscription>> GetAllSubscriptionsWithPackageByUserIdAsync(int userId);

		Task<int> AutoExpireSubscriptionsAsync();

		Task<UserSubscription?> GetActiveSubscriptionByPackageIdAsync(int userId, int servicePackageId);
		Task<List<UserSubscription>> GetAllActiveSubscriptionsByUserIdAsync(int userId);
		Task<bool> DisableSubscriptionAsync(int subscriptionId);
		Task<int> GetTotalRemainingUsagesByUserIdAsync(int userId);

		Task<UserSubscription> FindSameTypeSubscriptionIsActiveOrNot(int userId, int packageId);

	}
}
