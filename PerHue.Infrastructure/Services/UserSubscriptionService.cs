using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Utils;
using System.Security.Claims;

namespace PerHue.Infrastructure.Services
{
	internal class UserSubscriptionService : IUserSubscriptionService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public UserSubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		public async Task<int> CreateAsync(CreateUserSubscriptionModel model)
		{
			var entity = _mapper.Map<UserSubscription>(model);

			var user = await _unitOfWork.UserRepository.GetByIdAsync(model.UserId);
			var servicePackage = await _unitOfWork.ServicePackageRepository.GetByIdAsync(model.ServicePackageId);

			entity.StartDate = DateTime.Now;
			entity.EndDate = DateTime.Now.AddDays(servicePackage.Duration.Value);
			entity.CreateAt = DateTime.Now;
			entity.Status = UserSubscriptionStatusEnum.Pending.ToString();
			entity.User = user;
			entity.ServicePackage = servicePackage;

			await _unitOfWork.UserSubscriptionRepository.CreateAsync(entity);

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

		public async Task UpdateStatusUserSubscriptionAsync(int id, string status)
		{
			var entity = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(id);

			entity.Status = status;
			entity.UpdateAt = DateTime.Now;
			await _unitOfWork.UserSubscriptionRepository.UpdateAsync(entity);
		}

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
	}
}
