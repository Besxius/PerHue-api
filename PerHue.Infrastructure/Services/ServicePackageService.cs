using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;

namespace PerHue.Infrastructure.Services
{
	internal class ServicePackageService : IServicePackageService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public ServicePackageService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task CreateAsync(ServicePackageModel servicePackageModel)
		{
			var entity = _mapper.Map<ServicePackage>(servicePackageModel);
			await _unitOfWork.ServicePackageRepository.CreateAsync(entity);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _unitOfWork.ServicePackageRepository.GetByIdAsync(id);
			return await _unitOfWork.ServicePackageRepository.RemoveAsync(entity);
		}

		public async Task<IEnumerable<ServicePackageModel>> GetAllAsync()
		{
			var entities = await _unitOfWork.ServicePackageRepository.GetAllAsync();
			
			var models = _mapper.Map<IEnumerable<ServicePackageModel>>(entities);
			return models;
		}

		public async Task<ServicePackageModel> GetByIdAsync(int id)
		{ 
			var entity = await _unitOfWork.ServicePackageRepository.GetByIdAsync(id);
			return _mapper.Map<ServicePackageModel>(entity);
		}

		public async Task<bool> UpdateAsync(int id, ServicePackageModel servicePackageModel)
		{
			var entity = _mapper.Map<ServicePackage>(servicePackageModel);
			entity.Id = id;
			return await _unitOfWork.ServicePackageRepository.UpdateAsync(entity) > 0;
		}
		public async Task<ServicePackageModel> GetByAmountAsync(int amount)
		{
			var entity = await _unitOfWork.ServicePackageRepository.GetByAmountAsync(amount);
			return _mapper.Map<ServicePackageModel>(entity);
		}
	}
}
