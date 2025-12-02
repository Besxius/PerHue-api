using PerHue.Application.Basic;
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
	}
}
