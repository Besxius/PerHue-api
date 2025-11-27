using PerHue.Application.Basic;
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
		Task<bool> DeductUsageAsync(int userId, bool isFromExpertTest = false);
		Task<bool> RefundUsageAsync(int userId);
		Task<UserSubscriptionModel?> GetActiveSubscriptionAsync(int userId);
	}
}
