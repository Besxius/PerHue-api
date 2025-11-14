using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.Color;
using PerHue.Domain.UnitOfWork;

namespace PerHue.Infrastructure.Services
{
	internal class ColorService : IColorService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ColorService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _unitOfWork.ColorRepository.GetByIdAsync(id);
			return await _unitOfWork.ColorRepository.RemoveAsync(entity);
		}

		public async Task<IEnumerable<ColorModel>> GetAllAsync()
		{
			var entities = await _unitOfWork.ColorRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<ColorModel>>(entities);
		}

		public async Task<PaginatedResult<ColorModel>> GetAllAsync(int pageIndex , int pageSize, string? searchTerm)
		{
			var entities = await _unitOfWork.ColorRepository.GetAllAsync(pageIndex, pageSize, searchTerm);
			var totalCount = entities.Count();
			
			if (searchTerm.Length == 0)
			{
				totalCount = _unitOfWork.ColorRepository.GetAllAsync().Result.Count();
			}
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
			var paginatedResult = new PaginatedResult<ColorModel>
			{
				Items = _mapper.Map<IEnumerable<ColorModel>>(entities),
				PageSize = pageSize,
				PageIndex = pageIndex,
				TotalCount = totalCount,
				TotalPages = totalPages
			};
			return paginatedResult;
		}

		public async Task<ColorModel> GetByIdAsync(int id)
		{
			var entity = await _unitOfWork.ColorRepository.GetByIdAsync(id);
			return _mapper.Map<ColorModel>(entity);
		}

		public async Task<IEnumerable<ColorModel>> GetRelativeColors(List<string> selectedColors)
		{
			var entities = await _unitOfWork.ColorRepository.GetRelativeColors(selectedColors);
			return _mapper.Map<IEnumerable<ColorModel>>(entities);
		}
	}
}
