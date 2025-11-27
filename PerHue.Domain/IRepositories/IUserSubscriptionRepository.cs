using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IUserSubscriptionRepository : IGenericRepository<UserSubscription>
	{
		Task<UserSubscription> GetCurrentUserSubscriptionByUserIdAsync(int userId);
		Task<IEnumerable<UserSubscription>> GetHistoryUserSubscriptionsByUserIdAsync(int userId);
		Task<UserSubscription?> GetActiveByUserIdAsync(int userId);
		Task<UserSubscription?> GetSubscriptionForRefundAsync(int userId);

		Task<UserSubscription?> GetActiveSubscriptionAsync(int userId);
		Task<bool> HasActiveSubscriptionWithRemainingUsesAsync(int userId);
		Task<bool> DeductRemainingUsesAsync(int userId);
		Task<bool> RefundRemainingUsesAsync(int userId);
	}
}
