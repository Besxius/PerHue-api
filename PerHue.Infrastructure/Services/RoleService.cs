using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Domain.UnitOfWork;

namespace PerHue.Infrastructure.Services
{
	internal class RoleService : IRoleService
	{
		private readonly IUnitOfWork _unitOfWork;

		private readonly IMapper _mapper;
		public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _unitOfWork.RoleRepository.GetByIdAsync(id);
			return await _unitOfWork.RoleRepository.RemoveAsync(entity);
		}

		public async Task<IEnumerable<RoleModel>> GetAllAsync()
		{
			var entities = await _unitOfWork.RoleRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<RoleModel>>(entities);
		}

		public async Task<RoleModel> GetByIdAsync(int id)
		{
			var entity = await _unitOfWork.RoleRepository.GetByIdAsync(id);
			return _mapper.Map<RoleModel>(entity);
		}
	}
}
