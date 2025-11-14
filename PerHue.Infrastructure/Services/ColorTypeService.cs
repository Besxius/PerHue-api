using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.ColorType;
using PerHue.Domain.UnitOfWork;

namespace PerHue.Infrastructure.Services
{
	internal class ColorTypeService : IColorTypeService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public ColorTypeService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		public Task<bool> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<ColorTypeModel>> GetAllAsync()
		{
			var entity = await _unitOfWork.ColorTypeRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<ColorTypeModel>>(entity);
		}

		public async Task<ColorTypeModel> GetByIdAsync(int id)
		{
			var entity = await _unitOfWork.ColorTypeRepository.GetByIdAsync(id);
			return _mapper.Map<ColorTypeModel>(entity);
		}

		public async Task<ColorTypeModel> GetByNameAsync(string name)
		{
			var entity = await _unitOfWork.ColorTypeRepository.GetByNameAsync(name);
			return _mapper.Map<ColorTypeModel>(entity);
		}
	}
}
