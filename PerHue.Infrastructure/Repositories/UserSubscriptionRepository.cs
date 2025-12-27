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
		private readonly IDateTimeService _dateTimeService;
		public UserSubscriptionRepository(PerHueDbContext context, IDateTimeService dateTimeService) : base(context)
		{
			_dateTimeService = dateTimeService;
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
				.Include(us => us.ServicePackage)
				.FirstOrDefaultAsync(us => us.UserId == userId && us.Status == true && us.ServicePackage.Type == "AI");
		}

		/// <summary>
		/// Lấy subscription đang active của user (status = true, còn trong thời hạn, còn lượt sử dụng)
		/// </summary>
		public async Task<UserSubscription?> GetActiveSubscriptionAsync(int userId)
		{
			var now = _dateTimeService.GetCurrentTime();
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
		public async Task<bool> HasActiveSubscriptionWithRemainingUsesAsync(int userId, string? type = null)
		{
			var now = _dateTimeService.GetCurrentTime();

			var query = _context.UserSubscriptions
				.Include(us => us.ServicePackage)
				.Where(us => us.UserId == userId
					&& us.Status == true
					&& us.StartDate <= now
					&& us.EndDate >= now
					&& us.RemainingUses > 0);

			// ✅ Apply type filter if provided
			if (!string.IsNullOrWhiteSpace(type))
			{
				query = query.Where(us => us.ServicePackage.Type == type);
			}

			return await query.AnyAsync();
		}

		/// <summary>
		/// Lấy subscription mới nhất đang active theo userId, packageId và type
		/// </summary>
		public async Task<UserSubscription?> GetLatestActiveSubscriptionByPackageAndTypeAsync(int userId, int packageId, string type)
		{
			var now = _dateTimeService.GetCurrentTime();
			return await _context.UserSubscriptions
				.Include(us => us.ServicePackage)
				.Include(us => us.User)
				.Where(us => us.UserId == userId
					&& us.ServicePackageId == packageId
					&& us.ServicePackage.Type == type
					&& us.Status == true
					&& us.StartDate <= now
					&& us.EndDate >= now
					&& us.RemainingUses > 0)
				.OrderByDescending(us => us.CreateAt)
				.FirstOrDefaultAsync();
		}

		/// <summary>
		/// Trừ 1 lượt sử dụng của user
		/// </summary>
		public async Task<bool> DeductRemainingUsesAsync(int userId, int packageId, string type)
		{
			var subscription = await GetLatestActiveSubscriptionByPackageAndTypeAsync(userId, packageId, type);

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
		public async Task<bool> RefundRemainingUsesAsync(int userId, int packageId, string type)
		{
			var now = _dateTimeService.GetCurrentTime();
			var subscription = await GetLatestActiveSubscriptionByPackageAndTypeAsync(userId, packageId, type);

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
				.Where(s => s.UserId == userId && s.EndDate > _dateTimeService.GetCurrentTime())
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
					&& us.EndDate >= _dateTimeService.GetCurrentTime())
				.OrderBy(us => us.EndDate)
				.ToListAsync();
		}

		// Đếm tổng lượt sử dụng còn lại theo PackageId
		public async Task<Dictionary<int, int>> GetTotalRemainingUsesByPackageAndUserAsync(int userId)
		{
			var result = await _context.UserSubscriptions
				.Where(us => us.UserId == userId
					&& us.Status == true
					&& us.EndDate >= _dateTimeService.GetCurrentTime())
				.GroupBy(us => us.ServicePackageId)
				.Select(g => new
				{
					PackageId = g.Key,
					TotalRemaining = g.Sum(us => us.RemainingUses)
				})
				.ToDictionaryAsync(x => x.PackageId, x => x.TotalRemaining);

			return result;
		}

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
				.Where(us => us.Status == true && us.EndDate < _dateTimeService.GetCurrentTime())
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




		public async Task<UserSubscription?> GetActiveSubscriptionByPackageIdAsync(int userId, int servicePackageId)
		{
			return await _context.UserSubscriptions
				.Include(us => us.ServicePackage)
				.Include(us => us.User)
				.FirstOrDefaultAsync(us =>
					us.UserId == userId &&
					us.ServicePackageId == servicePackageId &&
					us.Status == true &&
					us.EndDate >= _dateTimeService.GetCurrentTime());
		}

		//public async Task<List<UserSubscription>> GetAllActiveSubscriptionsByUserIdAsync(int userId)
		//{
		//	return await _context.UserSubscriptions
		//		.Include(us => us.ServicePackage)
		//		.Where(us =>
		//			us.UserId == userId &&
		//			us.Status == true &&
		//			us.EndDate >= _dateTimeService.GetCurrentTime())
		//		.ToListAsync();
		//}

		public async Task<bool> DisableSubscriptionAsync(int subscriptionId)
		{
			var subscription = await GetByIdAsync(subscriptionId);
			if (subscription == null)
				return false;

			subscription.Status = false;
			await UpdateAsync(subscription);
			return true;
		}

		public async Task<int> GetTotalRemainingUsagesByUserIdAsync(int userId)
		{
			return await _context.UserSubscriptions
				.Where(us =>
					us.UserId == userId &&
					us.Status == true &&
					us.EndDate >= _dateTimeService.GetCurrentTime())
				.SumAsync(us => us.RemainingUses);
		}

		public async Task<UserSubscription> FindSameTypeSubscriptionIsActiveOrNot (int userId, string type)
		{
			return await _context.UserSubscriptions
				.Where(us => us.UserId == userId && us.Status == true && us.ServicePackage.Type == type)
				.OrderByDescending(us => us.EndDate)
				.FirstOrDefaultAsync();
		}


		// ========= An làm
		/// <summary>
		/// Lấy tất cả subscriptions đang sử dụng của user tính đến thời điểm hiện tại
		/// (Status = true, còn trong thời hạn, còn lượt sử dụng)
		/// </summary>
		public async Task<List<UserSubscription>> GetCurrentlyActiveSubscriptionsByUserIdAsync(int userId)
		{
			var now = _dateTimeService.GetCurrentTime();
			return await _context.UserSubscriptions
				.Include(us => us.ServicePackage)
				.Where(us => us.UserId == userId
					&& us.Status == true
					&& us.StartDate <= now
					&& us.EndDate >= now
					&& us.RemainingUses > 0)
				.OrderByDescending(us => us.CreateAt)
				.ToListAsync();
		}

		/// <summary>
		/// Lấy tất cả subscriptions đã đăng ký của user (chỉ active)
		/// </summary>
		public async Task<List<UserSubscription>> GetAllActiveSubscriptionsByUserIdAsync(int userId)
		{
			return await _context.UserSubscriptions
				.Include(us => us.ServicePackage)
				.Where(us => us.UserId == userId && us.Status == true)
				.OrderByDescending(us => us.CreateAt)
				.ToListAsync();
		}

		/// <summary>
		/// Lấy tất cả subscriptions đã đăng ký của user (chỉ inactive)
		/// </summary>
		public async Task<List<UserSubscription>> GetAllInactiveSubscriptionsByUserIdAsync(int userId)
		{
			return await _context.UserSubscriptions
				.Include(us => us.ServicePackage)
				.Where(us => us.UserId == userId && us.Status == false)
				.OrderByDescending(us => us.CreateAt)
				.ToListAsync();
		}

		/// <summary>
		/// Lấy tất cả subscriptions đã đăng ký của user (cả active và inactive)
		/// </summary>
		public async Task<List<UserSubscription>> GetAllRegisteredSubscriptionsByUserIdAsync(int userId)
		{
			return await _context.UserSubscriptions
				.Include(us => us.ServicePackage)
				.Where(us => us.UserId == userId)
				.OrderByDescending(us => us.CreateAt)
				.ToListAsync();
		}

		/// <summary>
		/// Lấy tất cả subscriptions với phân trang và filter theo status
		/// </summary>
		public async Task<(List<UserSubscription> subscriptions, int totalCount)> GetUserSubscriptionsWithPaginationAsync(
			int userId,
			int pageIndex,
			int pageSize,
			bool? status = null)
		{
			var query = _context.UserSubscriptions
				.Include(us => us.ServicePackage)
				.Where(us => us.UserId == userId);

			if (status.HasValue)
			{
				query = query.Where(us => us.Status == status.Value);
			}

			var totalCount = await query.CountAsync();

			var subscriptions = await query
				.OrderByDescending(us => us.CreateAt)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (subscriptions, totalCount);
		}

	}
}
