using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Repositories
{
	internal class UserSubscriptionRepository : GenericRepository<UserSubscription>, IUserSubscriptionRepository
	{
		public UserSubscriptionRepository(PerHueDbContext context) : base(context)
		{
		}
		public async Task<IEnumerable<UserSubscription>> GetHistoryUserSubscriptionsByUserIdAsync(int userId)
		{
			return await _context.UserSubscriptions
				.Where(us => us.UserId == userId)
				.ToListAsync();
		}
		public async Task<UserSubscription> GetCurrentUserSubscriptionByUserIdAsync(int userId)
		{
			return await _context.UserSubscriptions
				.FirstOrDefaultAsync(us => us.UserId == userId && us.Status == true);
		}
		public async Task<UserSubscription?> GetActiveByUserIdAsync(int userId)
		{
			return await _context.UserSubscriptions
				.FirstOrDefaultAsync(s => s.UserId == userId && s.Status == true && s.RemainingUses > 0);
		}
		public async Task<UserSubscription?> GetSubscriptionForRefundAsync(int userId)
		{
			// Find the most recent subscription that hasn't expired by date
			return await _context.UserSubscriptions
				.Where(s => s.UserId == userId && s.EndDate > DateTime.Now)
				.OrderByDescending(s => s.EndDate)
				.FirstOrDefaultAsync();
		}
	}
}
