using AutoMapper;
using Microsoft.Extensions.Logging;
using PerHue.Application.IServices;
using PerHue.Application.Models.UserSubscription;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Utils;
using System;

namespace PerHue.Infrastructure.Services
{
	internal class UserSubscriptionService : IUserSubscriptionService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ILogger<UserSubscriptionService> _logger;
		public UserSubscriptionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserSubscriptionService> logger)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_logger = logger;
		}
		public async Task<int> CreateAsync(CreateUserSubscriptionModel model)
		{
			var entity = _mapper.Map<UserSubscription>(model);

			var user = await _unitOfWork.UserRepository.GetByIdAsync(model.UserId);
			var servicePackage = await _unitOfWork.ServicePackageRepository.GetByIdAsync(model.ServicePackageId);

			var findUserSubscription = await _unitOfWork.UserSubscriptionRepository
				.FindSameTypeSubscriptionIsActiveOrNot(user.Id, servicePackage.Id);
			if (findUserSubscription != null)
			{
				findUserSubscription.Status = false;
			}

			entity.StartDate = DateTime.Now;
			entity.EndDate = DateTime.Now.AddMonths(servicePackage.Duration);
			entity.CreateAt = DateTime.Now;
			entity.Status = model.Status;
			entity.User = user;
			entity.ServicePackage = servicePackage;

			await _unitOfWork.UserSubscriptionRepository.CreateAsync(entity);

			if (model.Status == true)
			{
				user.IsAitested = true;
				await _unitOfWork.SaveChangesWithTransactionAsync();
			}

			return entity.Id;
		}
		public async Task<IEnumerable<UserSubscriptionModel>> GetAllAsync()
		{
			var entities = await _unitOfWork.UserSubscriptionRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<UserSubscriptionModel>>(entities);
		}
		public async Task<UserSubscriptionModel> GetByIdAsync(int id)
		{
			var entity = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(id);
			return _mapper.Map<UserSubscriptionModel>(entity);

		}

		//public async Task UpdateStatusUserSubscriptionAsync(int id, bool status)
		//{
		//	var entity = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(id);

		//	entity.Status = status;
		//	await _unitOfWork.UserSubscriptionRepository.UpdateAsync(entity);
		//}

		public async Task<UserSubscriptionModel> GetCurrentUserSubscriptionByUserIdAsync(int userId)
		{
			var entity = await _unitOfWork.UserSubscriptionRepository.GetCurrentUserSubscriptionByUserIdAsync(userId);
			return _mapper.Map<UserSubscriptionModel>(entity);
		}

		public Task<IEnumerable<UserSubscriptionModel>> GetHistoryUserSubscriptionsByUserIdAsync(int userId)
		{
			var entities = _unitOfWork.UserSubscriptionRepository.GetHistoryUserSubscriptionsByUserIdAsync(userId);
			return _mapper.Map<Task<IEnumerable<UserSubscriptionModel>>>(entities);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(id);
			return await _unitOfWork.UserSubscriptionRepository.RemoveAsync(entity);
		}

		public Task UpdateStatusUserSubscriptionAsync(int id, string status)
		{
			throw new NotImplementedException();
		}

		//====================================================================================

		/// <summary>
		/// Kiểm tra xem user có còn lượt sử dụng không
		/// </summary>
		public async Task<bool> HasRemainingUsageAsync(int userId)
		{
			try
			{
				var hasActive = await _unitOfWork.UserSubscriptionRepository
					.HasActiveSubscriptionWithRemainingUsesAsync(userId);

				_logger.LogInformation($"User {userId} has remaining usage: {hasActive}");
				return hasActive;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error checking remaining usage for user {userId}");
				return false;
			}
		}

		/// <summary>
		/// Lấy số lượt sử dụng còn lại của user
		/// </summary>
		public async Task<int> GetRemainingUsageAsync(int userId)
		{
			try
			{
				var subscription = await _unitOfWork.UserSubscriptionRepository
					.GetActiveSubscriptionAsync(userId);

				var remaining = subscription?.RemainingUses ?? 0;
				_logger.LogInformation($"User {userId} has {remaining} remaining uses");

				return remaining;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting remaining usage for user {userId}");
				return 0;
			}
		}

		/// <summary>
		/// Trừ 1 lượt sử dụng của user
		/// </summary>
		/// <param name="userId">ID của user</param>
		/// <param name="isFromExpertTest">True nếu là request từ Expert Test (không trừ lượt)</param>
		/// <returns>True nếu trừ thành công, False nếu không còn lượt hoặc lỗi</returns>
		public async Task<bool> DeductUsageAsync(int userId)
		{
			try
			{
				var deducted = await _unitOfWork.UserSubscriptionRepository.DeductRemainingUsesAsync(userId);

				if (!deducted)
				{
					_logger.LogWarning($"Failed to deduct usage for user {userId} - No active subscription or no remaining uses");
					return false;
				}

				await _unitOfWork.SaveChangesWithTransactionAsync();

				var remaining = await GetRemainingUsageAsync(userId);
				_logger.LogInformation($"Successfully deducted 1 usage for user {userId}. Remaining: {remaining}");

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error deducting usage for user {userId}");
				return false;
			}
		}

		/// <summary>
		/// Hoàn trả 1 lượt sử dụng (trong trường hợp lỗi xử lý)
		/// </summary>
		public async Task<bool> RefundUsageAsync(int userId)
		{
			try
			{
				var refunded = await _unitOfWork.UserSubscriptionRepository.RefundRemainingUsesAsync(userId);

				if (!refunded)
				{
					_logger.LogWarning($"Failed to refund usage for user {userId}");
					return false;
				}

				await _unitOfWork.SaveChangesWithTransactionAsync();

				var remaining = await GetRemainingUsageAsync(userId);
				_logger.LogInformation($"Successfully refunded 1 usage for user {userId}. Remaining: {remaining}");

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error refunding usage for user {userId}");
				return false;
			}
		}

		/// <summary>
		/// Lấy thông tin subscription đang active của user
		/// </summary>
		public async Task<UserSubscriptionModel?> GetActiveSubscriptionAsync(int userId)
		{
			try
			{
				var subscription = await _unitOfWork.UserSubscriptionRepository.GetActiveSubscriptionAsync(userId);

				if (subscription == null)
				{
					_logger.LogInformation($"No active subscription found for user {userId}");
					return null;
				}

				return _mapper.Map<UserSubscriptionModel>(subscription);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error getting active subscription for user {userId}");
				return null;
			}
		}


		// ========================= PAYMENT ============================

		// Kiểm tra có lượt sử dụng còn lại không
		public async Task<bool> BooleanForHasRemainingUsageAsync(int userId)
		{
			return await _unitOfWork.UserSubscriptionRepository.HasRemainingUsageAsync(userId);
		}

		// Lấy TỔNG lượt sử dụng còn lại (tất cả packages)
		public async Task<int> GetAllActiveRemainingUsageByUserIdAsync(int userId)
		{
			var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetaAllActiveSubscriptionsByUserIdAsync(userId);
			return subscriptions.Sum(s => s.RemainingUses);
		}

		// Lấy lượt sử dụng còn lại THEO TỪNG PACKAGE
		public async Task<Dictionary<int, PackageUsageInfo>> GetRemainingUsageByPackageAsync(int userId)
		{
			var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetaAllActiveSubscriptionsByUserIdAsync(userId);

			var packageUsage = subscriptions
				.GroupBy(s => s.ServicePackageId)
				.ToDictionary(
					g => g.Key,
					g => new PackageUsageInfo
					{
						PackageId = g.Key,
						PackageName = g.First().ServicePackage?.Name ?? "Unknown",
						TotalRemaining = g.Sum(s => s.RemainingUses),
						SubscriptionCount = g.Count()
					}
				);

			return packageUsage;
		}

		// Trừ 1 lượt sử dụng
		/*public async Task<bool> DeductUsageAsync(int userId)
		{
			try
			{
				var deducted = await _repository.DeductRemainingUsesAsync(userId);

				if (deducted)
				{
					var remaining = await GetRemainingUsageAsync(userId);
					_logger.LogInformation($"Deducted 1 usage for user {userId}. Total remaining: {remaining}");
				}
				else
				{
					_logger.LogWarning($"Failed to deduct usage for user {userId} - No active subscriptions");
				}

				return deducted;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error deducting usage for user {userId}");
				return false;
			}
		}*/

		// Hoàn trả 1 lượt sử dụng
		/*public async Task<bool> RefundUsageAsync(int userId)
		{
			try
			{
				var refunded = await _repository.RefundRemainingUsesAsync(userId);

				if (refunded)
				{
					_logger.LogInformation($"Refunded 1 usage for user {userId}");
				}

				return refunded;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error refunding usage for user {userId}");
				return false;
			}
		}*/

		// Lấy thông tin tổng hợp sử dụng theo package
		public async Task<List<PackageUsageSummary>> GetUsageSummaryAsync(int userId)
		{
			var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetAllSubscriptionsWithPackageByUserIdAsync(userId);

			var summary = subscriptions
				.Where(s => s.Status == true && s.EndDate >= DateTime.UtcNow)
				.GroupBy(s => new { s.ServicePackageId, s.ServicePackage.Name })
				.Select(g => new PackageUsageSummary
				{
					PackageId = g.Key.ServicePackageId,
					PackageName = g.Key.Name,
					TotalPurchased = g.Sum(s => s.ServicePackage.Uses),
					TotalUsed = g.Sum(s => s.ServicePackage.Uses - s.RemainingUses),
					TotalRemaining = g.Sum(s => s.RemainingUses),
					ActiveSubscriptionCount = g.Count(),
					EarliestExpiry = g.Min(s => s.EndDate),
					LatestExpiry = g.Max(s => s.EndDate)
				})
				.ToList();

			return summary;
		}
	}
}
