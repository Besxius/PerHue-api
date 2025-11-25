using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.CapsulePalette;
using PerHue.Domain.UnitOfWork;

namespace PerHue.Infrastructure.Services
{
	internal class CapsulePaletteService : ICapsulePaletteService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public CapsulePaletteService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _unitOfWork.CapsulePaletteRepository.GetByIdAsync(id);
			return await _unitOfWork.CapsulePaletteRepository.RemoveAsync(entity);
		}

		public async Task<PaginatedResult<CapsulePaletteModel>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm)
		{
			var entities = await _unitOfWork.CapsulePaletteRepository.GetAllAsync(pageIndex, pageSize, searchTerm);
			var totalCount = entities.Count();
			if (searchTerm.Length == 0)
			{
				totalCount = _unitOfWork.CapsulePaletteRepository.GetAllAsync().Result.Count();
			}
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
			var paginatedResult = new PaginatedResult<CapsulePaletteModel>
			{
				Items = _mapper.Map<IEnumerable<CapsulePaletteModel>>(entities),
				PageSize = pageSize,
				PageIndex = pageIndex,
				TotalCount = totalCount,
				TotalPages = totalPages
			};
			return paginatedResult;
		}

		public async Task<IEnumerable<CapsulePaletteModel>> GetAllAsync()
		{
			var entities = await _unitOfWork.CapsulePaletteRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<CapsulePaletteModel>>(entities);
		}

		public async Task<CapsulePaletteModel> GetByIdAsync(int id)
		{
			var entity = await _unitOfWork.CapsulePaletteRepository.GetByIdAsync(id);
			return _mapper.Map<CapsulePaletteModel>(entity);
		}

		public async Task<IEnumerable<CapsulePaletteModel>> GetRelativeCapsulePalettes(List<string> selectedColors)
		{
			var entities = await _unitOfWork.CapsulePaletteRepository.GetRelativeCapsulePalettes(selectedColors);
			return _mapper.Map<IEnumerable<CapsulePaletteModel>>(entities);
		}

		public async Task<IEnumerable<CapsulePaletteModel>> GetByColorTypeIdAsync(int colorTypeId)
		{
			var entities = await _unitOfWork.CapsulePaletteRepository.GetByColorTypeIdAsync(colorTypeId);
			return _mapper.Map<IEnumerable<CapsulePaletteModel>>(entities);
		}

		public async Task<PaginatedResult<CapsulePaletteModel>> GetByColorTypeIdPagedAsync(int colorTypeId, int pageIndex, int pageSize, string? searchTerm)
		{
			var (entities, totalCount) = await _unitOfWork.CapsulePaletteRepository.GetByColorTypeIdPagedAsync(colorTypeId, pageIndex, pageSize, searchTerm);

			var items = _mapper.Map<IEnumerable<CapsulePaletteModel>>(entities);

			return new PaginatedResult<CapsulePaletteModel>
			{
				Items = items,
				PageIndex = pageIndex,
				PageSize = pageSize,
				TotalCount = totalCount,
				TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
			};
		}
	}
}
