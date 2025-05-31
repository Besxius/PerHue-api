using PerHue.Application.Models;

namespace PerHue.Application.IServices
{
	public interface IUserSubscriptionService
	{
		Task<int> CreateUserSubscriptionAsync(CreateUserSubscriptionModel model);
		Task<UserSubscriptionModel> GetCurrentUserSubscriptionByUserIdAsync(int userId);
		Task<IEnumerable<UserSubscriptionModel>> GetHistoryUserSubscriptionsByUserIdAsync(int userId);
		Task<UserSubscriptionModel> GetUserSubscriptionByIdAsync(int id);
		Task<IEnumerable<UserSubscriptionModel>> GetUserSubscriptionModels();
		Task UpdateUserSubscriptionAsync(int id, string status);
	}
}
