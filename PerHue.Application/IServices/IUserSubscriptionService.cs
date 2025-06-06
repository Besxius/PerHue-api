using PerHue.Application.Basic;
using PerHue.Application.Models;

namespace PerHue.Application.IServices
{
	public interface IUserSubscriptionService : IGenericService<UserSubscriptionModel>
	{
		Task<int> CreateAsync(CreateUserSubscriptionModel model);
		Task<UserSubscriptionModel> GetCurrentUserSubscriptionByUserIdAsync(int userId);
		Task<IEnumerable<UserSubscriptionModel>> GetHistoryUserSubscriptionsByUserIdAsync(int userId);
		Task UpdateStatusUserSubscriptionAsync(int id, string status);
	}
}
