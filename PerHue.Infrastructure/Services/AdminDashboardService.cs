using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.Dashboard;
using PerHue.Infrastructure.Persistence;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Services
{
	public class AdminDashboardService : IAdminDashboardService
	{
		private readonly PerHueDbContext _context;

		public AdminDashboardService(PerHueDbContext context)
		{
			_context = context;
		}

		public async Task<DashboardMetricsModel> GetDashboardMetricsAsync(DateTime? startDate = null, DateTime? endDate = null)
		{
			var today = DateTime.Today;
			var start = startDate ?? today.AddDays(-30);
			var end = endDate ?? today.AddDays(1);

			var totalUsers = await _context.UserAccounts.CountAsync();
			var totalExperts = await _context.UserAccounts
				.Where(u => u.RoleId == 3)
				.CountAsync();

			var activeUsers = await _context.UserAccounts
				.Where(u => u.IsActive)
				.CountAsync();

			var newUsersToday = await _context.UserAccounts
				.Where(u => u.CreatedDate.Date == today)
				.CountAsync();

			var totalRevenue = await _context.Payments
				.Where(p => p.Status.ToLower() == nameof(PaymentStatusEnum.Success).ToLower())
				.SumAsync(p => p.Amount);

			var revenueToday = await _context.Payments
				.Where(p => p.Status.ToLower() == nameof(PaymentStatusEnum.Success).ToLower() && p.CreatedAt.Date == today)
				.SumAsync(p => p.Amount);

			var totalTests = await _context.TestResults.CountAsync();

			var testsToday = await _context.TestRequests
				.Where(t => t.CreatedDate.HasValue && t.CreatedDate.Value.Date == today)
				.CountAsync();

			// Assuming penalty table exists
			var totalPenalties = 0;
			var penaltyAmountTotal = 0m;

			return new DashboardMetricsModel
			{
				TotalUsers = totalUsers,
				TotalExperts = totalExperts,
				ActiveUsers = activeUsers,
				NewUsersToday = newUsersToday,
				TotalRevenue = totalRevenue,
				RevenueToday = revenueToday,
				TotalTests = totalTests,
				TestsToday = testsToday,
				TotalPenalties = totalPenalties,
				PenaltyAmountTotal = penaltyAmountTotal,
				ExpertActivityCount = totalExperts,
				LastUpdated = DateTime.Now
			};
		}

		public async Task<AccountCountModel> GetAccountCountsAsync(DateTime? startDate = null, DateTime? endDate = null)
		{
			var today = DateTime.Today;
			var thisMonth = new DateTime(today.Year, today.Month, 1);
			var thisWeek = today.AddDays(-(int)today.DayOfWeek);

			var totalAccounts = await _context.UserAccounts.CountAsync();
			var activeAccounts = await _context.UserAccounts.Where(u => u.IsActive).CountAsync();
			var inactiveAccounts = await _context.UserAccounts.Where(u => !u.IsActive).CountAsync();
			var bannedAccounts = 0; // Add logic for banned users if field exists

			var expertAccounts = await _context.UserAccounts
				.Where(u => u.RoleId == 3)
				.CountAsync();

			var regularAccounts = totalAccounts - expertAccounts;

			var newAccountsThisMonth = await _context.UserAccounts
				.Where(u => u.CreatedDate >= thisMonth)
				.CountAsync();

			var newAccountsThisWeek = await _context.UserAccounts
				.Where(u => u.CreatedDate >= thisWeek)
				.CountAsync();

			var newAccountsToday = await _context.UserAccounts
				.Where(u => u.CreatedDate.Date == today)
				.CountAsync();

			var accountsByRole = await _context.UserAccounts
				.Include(u => u.Role)
				.GroupBy(u => u.Role.Name)
				.Select(g => new { Role = g.Key, Count = g.Count() })
				.ToDictionaryAsync(x => x.Role, x => x.Count);

			var accountsByDay = await _context.UserAccounts
				.Where(u => u.CreatedDate >= today.AddDays(-7))
				.GroupBy(u => u.CreatedDate.Date)
				.Select(g => new { Date = g.Key.ToString("yyyy-MM-dd"), Count = g.Count() })
				.ToDictionaryAsync(x => x.Date, x => x.Count);

			return new AccountCountModel
			{
				TotalAccounts = totalAccounts,
				ActiveAccounts = activeAccounts,
				InactiveAccounts = inactiveAccounts,
				BannedAccounts = bannedAccounts,
				ExpertAccounts = expertAccounts,
				RegularAccounts = regularAccounts,
				NewAccountsThisMonth = newAccountsThisMonth,
				NewAccountsThisWeek = newAccountsThisWeek,
				NewAccountsToday = newAccountsToday,
				AccountsByRole = accountsByRole,
				AccountsByDay = accountsByDay
			};
		}

		public async Task<PaginatedResultV2<AccountListItemModel>> GetAccountListAsync(AccountSearchModel searchModel)
		{
			var query = _context.UserAccounts.AsQueryable();

			// Apply filters
			if (!string.IsNullOrEmpty(searchModel.Email))
			{
				query = query.Where(u => u.Email.Contains(searchModel.Email));
			}

			if (!string.IsNullOrEmpty(searchModel.Username))
			{
				query = query.Where(u => u.Username.Contains(searchModel.Username));
			}

			if (!string.IsNullOrEmpty(searchModel.Fullname))
			{
				query = query.Where(u => u.Fullname != null && u.Fullname.Contains(searchModel.Fullname));
			}

			if (searchModel.IsActive.HasValue)
			{
				query = query.Where(u => u.IsActive == searchModel.IsActive.Value);
			}

			if (searchModel.RoleId.HasValue)
			{
				query = query.Where(u => u.RoleId == searchModel.RoleId.Value);
			}

			if (searchModel.CreatedFrom.HasValue)
			{
				query = query.Where(u => u.CreatedDate >= searchModel.CreatedFrom.Value);
			}

			if (searchModel.CreatedTo.HasValue)
			{
				query = query.Where(u => u.CreatedDate <= searchModel.CreatedTo.Value);
			}

			// Apply sorting
			query = searchModel.SortBy switch
			{
				AccountSortBy.Email => searchModel.SortOrder == SortOrder.Ascending
					? query.OrderBy(u => u.Email)
					: query.OrderByDescending(u => u.Email),
				AccountSortBy.Username => searchModel.SortOrder == SortOrder.Ascending
					? query.OrderBy(u => u.Username)
					: query.OrderByDescending(u => u.Username),
				AccountSortBy.LastLoginDate => searchModel.SortOrder == SortOrder.Ascending
					? query.OrderBy(u => u.CreatedDate)
					: query.OrderByDescending(u => u.CreatedDate),
				_ => searchModel.SortOrder == SortOrder.Ascending
					? query.OrderBy(u => u.CreatedDate)
					: query.OrderByDescending(u => u.CreatedDate)
			};

			var totalCount = await query.CountAsync();

			var items = await query
				.Include(u => u.Role)
				.Include(u => u.TestResults)
				.Include(u => u.Payments)
				.Skip((searchModel.PageIndex - 1) * searchModel.PageSize)
				.Take(searchModel.PageSize)
				.Select(u => new AccountListItemModel
				{
					Id = u.Id,
					Email = u.Email,
					Username = u.Username,
					Fullname = u.Fullname,
					IsActive = u.IsActive,
					IsBanned = false, // Add logic if ban field exists
					RoleName = u.Role.Name,
					CreatedDate = u.CreatedDate,
					LastLoginDate = u.CreatedDate,
					TestCount = u.TestResults.Count(),
					TotalSpent = u.Payments.Where(p => p.Status.ToLower() == nameof(PaymentStatusEnum.Success).ToLower()).Sum(p => p.Amount)
				})
				.ToListAsync();

			return new PaginatedResultV2<AccountListItemModel>
			{
				List = items,
				Total = totalCount,
				Current = searchModel.PageIndex
			};
		}

		public async Task<AccountDetailModel?> GetAccountDetailAsync(int accountId)
		{
			var user = await _context.UserAccounts
				.Include(u => u.Role)
				.Include(u => u.TestResults)
				.Include(u => u.Payments)
				.Include(u => u.UserSubscriptions)
					.ThenInclude(us => us.ServicePackage)
				.FirstOrDefaultAsync(u => u.Id == accountId);

			if (user == null)
				return null;

			var recentActivity = new List<AccountActivityModel>
			{
				new AccountActivityModel { Activity = "Account Created", Date = user.CreatedDate, Details = "User registered" }
			};

			var subscriptions = user.UserSubscriptions.Select(us => new AccountSubscriptionModel
			{
				Id = us.Id,
				PackageName = us.ServicePackage.Name,
				StartDate = us.StartDate ?? DateTime.MinValue,
				EndDate = us.EndDate ?? DateTime.MinValue,
				IsActive = us.Status,
				Amount = us.ServicePackage.Price
			}).ToList();

			return new AccountDetailModel
			{
				Id = user.Id,
				Email = user.Email,
				Username = user.Username,
				Fullname = user.Fullname,
				Phone = user.Phone,
				Gender = user.Gender,
				Dob = user.Dob,
				IsActive = user.IsActive,
				IsBanned = false, // Add logic if ban field exists
				ProfilePicture = user.ProfilePicture,
				RoleId = user.RoleId,
				RoleName = user.Role.Name,
				CreatedDate = user.CreatedDate,
				UpdatedDate = null,
				LastLoginDate = user.CreatedDate,
				TotalTests = user.TestResults.Count,
				CompletedTests = user.TestResults.Count,
				TotalSpent = user.Payments.Where(p => p.Status.ToLower() == nameof(PaymentStatusEnum.Success).ToLower()).Sum(p => p.Amount),
				SubscriptionCount = user.UserSubscriptions.Count,
				RecentActivity = recentActivity,
				Subscriptions = subscriptions
			};
		}

		public async Task<PaginatedResultV2<ExpertActivityModel>> GetExpertActivityAsync(ExpertActivitySearchModel searchModel)
		{
			var query = _context.UserAccounts
				.Include(x => x.Expert)
				.Where(u => u.RoleId == 3)
				.AsQueryable();

			if (!string.IsNullOrEmpty(searchModel.ExpertName))
			{
				query = query.Where(u => u.Fullname != null && u.Fullname.Contains(searchModel.ExpertName));
			}

			if (!string.IsNullOrEmpty(searchModel.ExpertEmail))
			{
				query = query.Where(u => u.Email.Contains(searchModel.ExpertEmail));
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.Skip((searchModel.PageIndex - 1) * searchModel.PageSize)
				.Take(searchModel.PageSize)
				.Select(u => new ExpertActivityModel
				{
					ExpertId = u.Id,
					ExpertName = u.Fullname ?? u.Username,
					ExpertEmail = u.Email,
					ProfilePicture = u.ProfilePicture,
					TestsCompleted = u.TestResults.Count,
					TestsInProgress = 0,
					TotalEarnings = 0, // Add calculation based on your business logic
					AverageRating = u.Expert.Rating ?? 0, // Add calculation based on rating system
					LastActiveDate = u.CreatedDate,
					IsActive = u.IsActive, // Add logic for online status
					RecentActivities = new List<ExpertRecentActivityModel>()
				})
				.ToListAsync();

			return new PaginatedResultV2<ExpertActivityModel>
			{
				List = items,
				Total = totalCount,
				Current = searchModel.PageIndex
			};
		}

		public async Task<RevenueStatisticsModel> GetRevenueStatisticsAsync(DateTime startDate, DateTime endDate, string? groupBy = "day")
		{
			var payments = await _context.Payments
				.Where(p => p.Status.ToLower() == nameof(PaymentStatusEnum.Success).ToLower() && p.CreatedAt >= startDate && p.CreatedAt <= endDate)
				.ToListAsync();

			var totalRevenue = payments.Sum(p => p.Amount);

			var previousPeriod = startDate.AddDays(-(endDate - startDate).Days);
			var previousRevenue = await _context.Payments
				.Where(p => p.Status.ToLower() == nameof(PaymentStatusEnum.Success).ToLower() && p.CreatedAt >= previousPeriod && p.CreatedAt < startDate)
				.SumAsync(p => p.Amount);

			var growthPercentage = previousRevenue > 0 ? ((totalRevenue - previousRevenue) / previousRevenue) * 100 : 0;

			var revenueData = payments
				.GroupBy(p => groupBy?.ToLower() switch
				{
					"week" => p.CreatedAt.Date.AddDays(-(int)p.CreatedAt.DayOfWeek),
					"month" => new DateTime(p.CreatedAt.Year, p.CreatedAt.Month, 1),
					_ => p.CreatedAt.Date
				})
				.Select(g => new RevenueDataPoint
				{
					Period = g.Key.ToString("yyyy-MM-dd"),
					Amount = g.Sum(p => p.Amount),
					TransactionCount = g.Count(),
					Date = g.Key
				})
				.OrderBy(d => d.Date)
				.ToList();

			return new RevenueStatisticsModel
			{
				TotalRevenue = totalRevenue,
				PreviousPeriodRevenue = previousRevenue,
				GrowthPercentage = growthPercentage,
				RevenueData = revenueData,
				StartDate = startDate,
				EndDate = endDate,
				GroupBy = groupBy ?? "day"
			};
		}

		public async Task<TestCountStatisticsModel> GetTestCountStatisticsAsync(DateTime startDate, DateTime endDate, string? groupBy = "day", string? testType = null)
		{
			var query = _context.TestRequests
				.Where(t => t.CreatedDate.HasValue && t.CreatedDate.Value >= startDate && t.CreatedDate.Value <= endDate);

			if (!string.IsNullOrEmpty(testType))
			{
				query = query.Where(t => t.TypeOfTest.ToLower() == testType.ToLower());
			}

			var tests = await query.ToListAsync();
			var totalTests = tests.Count;

			var previousPeriod = startDate.AddDays(-(endDate - startDate).Days);
			var previousTests = await _context.TestRequests
				.Where(t => t.CreatedDate.HasValue && t.CreatedDate.Value >= previousPeriod && t.CreatedDate.Value < startDate)
				.CountAsync();

			var growthPercentage = previousTests > 0 ? ((double)(totalTests - previousTests) / previousTests) * 100 : 0;

			var testData = tests
				.Where(t => t.CreatedDate.HasValue)
				.GroupBy(t => groupBy?.ToLower() switch
				{
					"week" => t.CreatedDate!.Value.Date.AddDays(-(int)t.CreatedDate.Value.DayOfWeek),
					"month" => new DateTime(t.CreatedDate!.Value.Year, t.CreatedDate.Value.Month, 1),
					_ => t.CreatedDate!.Value.Date
				})
				.Select(g => new TestCountDataPoint
				{
					Period = g.Key.ToString("yyyy-MM-dd"),
					Count = g.Count(),
					CompletedCount = g.Count(t => t.Status.ToLower() == (nameof(TestStatus.Completed)).ToLower()),
					InProgressCount = g.Count(t => t.Status.ToLower() == (nameof(TestStatus.Pending)).ToLower() || t.Status.ToLower() == (nameof(TestStatus.Processing)).ToLower()),
					Date = g.Key
				})
				.OrderBy(d => d.Date)
				.ToList();

			// Calculate tests by type statistics
			var testsByType = tests
				.GroupBy(t => t.TypeOfTest)
				.Select(g => new TestByTypeModel
				{
					TestType = g.Key,
					Count = g.Count(),
					Percentage = totalTests > 0 ? (double)g.Count() / totalTests * 100 : 0
				})
				.OrderByDescending(t => t.Count)
				.ToList();

			return new TestCountStatisticsModel
			{
				TotalTests = totalTests,
				PreviousPeriodTests = previousTests,
				GrowthPercentage = growthPercentage,
				TestData = testData,
				StartDate = startDate,
				EndDate = endDate,
				GroupBy = groupBy ?? "day",
				TestsByType = testsByType
			};
		}
	}
}