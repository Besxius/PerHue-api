using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Repositories
{
	public class UserSubscriptionRepository : GenericRepository<UserSubscription>, IUserSubscriptionRepository
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

		/// <summary>
		/// Lấy subscription đang active của user (status = true, còn trong thời hạn, còn lượt sử dụng)
		/// </summary>
		public async Task<UserSubscription?> GetActiveSubscriptionAsync(int userId)
		{
			var now = DateTime.UtcNow;
			return await _context.UserSubscriptions
				.Include(us => us.ServicePackage)
				.Include(us => us.User)
				.Where(us => us.UserId == userId
					&& us.Status == true
					&& us.StartDate <= now
					&& us.EndDate >= now
					&& us.RemainingUses > 0)
				.OrderByDescending(us => us.CreateAt)
				.FirstOrDefaultAsync();
		}

		/// <summary>
		/// Kiểm tra xem user có subscription active với lượt sử dụng còn lại không
		/// </summary>
		public async Task<bool> HasActiveSubscriptionWithRemainingUsesAsync(int userId)
		{
			var now = DateTime.UtcNow;
			return await _context.UserSubscriptions
				.AnyAsync(us => us.UserId == userId
					&& us.Status == true
					&& us.StartDate <= now
					&& us.EndDate >= now
					&& us.RemainingUses > 0);
		}

		/// <summary>
		/// Trừ 1 lượt sử dụng của user
		/// </summary>
		public async Task<bool> DeductRemainingUsesAsync(int userId)
		{
			var subscription = await GetActiveSubscriptionAsync(userId);

			if (subscription == null || subscription.RemainingUses <= 0)
			{
				return false;
			}

			subscription.RemainingUses -= 1;

			// Nếu hết lượt thì tự động set status = false
			if (subscription.RemainingUses == 0)
			{
				subscription.Status = false;
			}

			await _context.SaveChangesAsync();
			return true;
		}

		/// <summary>
		/// Hoàn trả 1 lượt sử dụng (trong trường hợp lỗi)
		/// </summary>
		public async Task<bool> RefundRemainingUsesAsync(int userId)
		{
			var now = DateTime.UtcNow;
			var subscription = await _context.UserSubscriptions
				.Include(us => us.ServicePackage)
				.Where(us => us.UserId == userId
					&& us.StartDate <= now
					&& us.EndDate >= now)
				.OrderByDescending(us => us.CreateAt)
				.FirstOrDefaultAsync();

			if (subscription == null)
			{
				return false;
			}

			// Lấy số lượt tối đa từ ServicePackage
			var maxUses = subscription.ServicePackage?.Uses ?? 0;

			// Chỉ hoàn trả nếu chưa vượt quá giới hạn
			if (subscription.RemainingUses < maxUses)
			{
				subscription.RemainingUses += 1;

				// Nếu trước đó status = false do hết lượt, giờ có lượt lại thì set = true
				if (!subscription.Status && subscription.RemainingUses > 0)
				{
					subscription.Status = true;
				}

				await _context.SaveChangesAsync();
				return true;
			}

			return false;
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


		//============= PAYMENT ========================================

		public async Task<List<UserSubscription>> GetaAllActiveSubscriptionsByUserIdAsync(int userId)
		{
			return await _context.UserSubscriptions
				.Include(us => us.ServicePackage)
				.Where(us => us.UserId == userId
					&& us.Status == true
					&& us.EndDate >= DateTime.UtcNow)
				.OrderBy(us => us.EndDate)
				.ToListAsync();
		}

		// Đếm tổng lượt sử dụng còn lại theo PackageId
		public async Task<Dictionary<int, int>> GetTotalRemainingUsesByPackageAndUserAsync(int userId)
		{
			var result = await _context.UserSubscriptions
				.Where(us => us.UserId == userId
					&& us.Status == true
					&& us.EndDate >= DateTime.UtcNow)
				.GroupBy(us => us.ServicePackageId)
				.Select(g => new
				{
					PackageId = g.Key,
					TotalRemaining = g.Sum(us => us.RemainingUses)
				})
				.ToDictionaryAsync(x => x.PackageId, x => x.TotalRemaining);

			return result;
		}

		// Kiểm tra có lượt sử dụng còn lại không
		public async Task<bool> HasRemainingUsageAsync(int userId)
		{
			return await _context.UserSubscriptions
				.AnyAsync(us => us.UserId == userId
					&& us.Status == true
					&& us.RemainingUses > 0
					&& us.EndDate >= DateTime.UtcNow);
		}

		// Trừ lượt sử dụng - Ưu tiên trừ từ subscription CŨ NHẤT
		/*
		public async Task<bool> DeductRemainingUsesAsync(int userId)
		{
			var subscription = await _context.UserSubscriptions
				.Where(us => us.UserId == userId
					&& us.Status == true
					&& us.RemainingUse > 0
					&& us.EndDate >= DateTime.UtcNow)
				.OrderBy(us => us.EndDate) // Trừ từ gói sắp hết hạn trước
				.FirstOrDefaultAsync();

			if (subscription == null)
				return false;

			subscription.RemainingUse -= 1;

			// KHÔNG tự động set Status = false khi hết lượt
			// Chỉ auto-expire khi hết hạn thời gian

			await _context.SaveChangesAsync();
			return true;
		}
		*/

		// Hoàn trả lượt sử dụng - Ưu tiên hoàn vào subscription MỚI NHẤT
		/*
		public async Task<bool> RefundRemainingUsesAsync(int userId)
		{
			var subscription = await _context.UserSubscriptions
				.Where(us => us.UserId == userId
					&& us.Status == true
					&& us.EndDate >= DateTime.UtcNow)
				.OrderByDescending(us => us.StartDate) // Hoàn vào gói mới nhất
				.FirstOrDefaultAsync();

			if (subscription == null)
				return false;

			subscription.RemainingUse += 1;
			await _context.SaveChangesAsync();
			return true;
		}
		*/


		public async Task<List<UserSubscription>> GetAllSubscriptionsWithPackageByUserIdAsync(int userId)
		{
			return await _context.UserSubscriptions
				.Include(us => us.ServicePackage)
				.Where(us => us.UserId == userId)
				.OrderByDescending(us => us.StartDate)
				.ToListAsync();
		}

		public async Task<int> AutoExpireSubscriptionsAsync()
		{
			var expiredSubscriptions = await _context.UserSubscriptions
				.Where(us => us.Status == true && us.EndDate < DateTime.UtcNow)
				.ToListAsync();

			foreach (var subscription in expiredSubscriptions)
			{
				subscription.Status = false;
			}

			return await _context.SaveChangesAsync();
		}


		public async Task<UserSubscription?> GetActiveSubscriptionByTypeAsync(int userId, string type)
		{
			return await _context.UserSubscriptions
				.Include(s => s.ServicePackage)
				.Where(s => s.UserId == userId
							&& s.Status == true
							&& s.RemainingUses > 0
							&& s.ServicePackage.Type == type)
				.OrderByDescending(s => s.EndDate)
				.FirstOrDefaultAsync();
		}
	}
}
