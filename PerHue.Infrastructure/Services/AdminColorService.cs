using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PerHue.Application.Basic;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.Color;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;

namespace PerHue.Infrastructure.Services
{
	internal class AdminColorService : IAdminColorService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public AdminColorService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<IEnumerable<AdminColorModel>> GetAllAsync()
		{
			var colors = await _unitOfWork.ColorRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<AdminColorModel>>(colors);
		}

		public async Task<PaginatedResultV2<AdminColorModel>> GetAllAsync(AdminColorSearchModel searchModel)
		{
			var baseQuery = _unitOfWork.ColorRepository.GetQueryable();
			IQueryable<Color> query = baseQuery;

			// Apply search filters
			if (!string.IsNullOrEmpty(searchModel.SearchTerm))
			{
				switch (searchModel.SearchBy?.ToLower())
				{
					case "name":
						query = query.Where(c => c.Name.Contains(searchModel.SearchTerm));
						break;
					case "hexcode":
						query = query.Where(c => c.HexCode.Contains(searchModel.SearchTerm));
						break;
					case "id":
						if (int.TryParse(searchModel.SearchTerm, out int id))
						{
							query = query.Where(c => c.Id == id);
						}
						break;
					default:
						query = query.Where(c => c.Name.Contains(searchModel.SearchTerm) || 
											   c.HexCode.Contains(searchModel.SearchTerm));
						break;
				}
			}

			// Apply sorting
			IOrderedQueryable<Color> orderedQuery;
			switch (searchModel.SortBy?.ToLower())
			{
				case "name":
					orderedQuery = searchModel.SortOrder?.ToLower() == "asc"
						? query.OrderBy(c => c.Name)
						: query.OrderByDescending(c => c.Name);
					break;
				case "id":
					orderedQuery = searchModel.SortOrder?.ToLower() == "asc"
						? query.OrderBy(c => c.Id)
						: query.OrderByDescending(c => c.Id);
					break;
				case "hexcode":
					orderedQuery = searchModel.SortOrder?.ToLower() == "asc"
						? query.OrderBy(c => c.HexCode)
						: query.OrderByDescending(c => c.HexCode);
					break;
				default:
					orderedQuery = searchModel.SortOrder?.ToLower() == "asc"
						? query.OrderBy(c => c.Id) // Using Id as proxy for creation date
						: query.OrderByDescending(c => c.Id);
					break;
			}

			var totalCount = await query.CountAsync();

			var colors = await orderedQuery
				.Skip((searchModel.PageIndex - 1) * searchModel.PageSize)
				.Take(searchModel.PageSize)
				.Select(c => new AdminColorModel
				{
					Id = c.Id,
					Name = c.Name,
					HexCode = c.HexCode,
				})
				.ToListAsync();

			return new PaginatedResultV2<AdminColorModel>
			{
				List = colors,
				Total = totalCount,
				Current = searchModel.PageIndex,
			};
		}

		async Task<AdminColorModel> IGenericService<AdminColorModel>.GetByIdAsync(int id)
		{
			var color = await _unitOfWork.ColorRepository.GetByIdAsync(id);
			if (color == null) throw new InvalidOperationException($"Color with id {id} not found");

			return new AdminColorModel
			{
				Id = color.Id,
				Name = color.Name,
				HexCode = color.HexCode,
			};
		}

		public async Task<AdminColorModel?> GetByIdAsync(int id)
		{
			var color = await _unitOfWork.ColorRepository.GetByIdAsync(id);
			if (color == null) return null;

			return new AdminColorModel
			{
				Id = color.Id,
				Name = color.Name,
				HexCode = color.HexCode,
			};
		}

		public async Task<AdminColorModel> CreateAsync(AdminColorCreateModel model)
		{
			var color = new Color
			{
				Name = model.Name,
				HexCode = model.HexCode
			};

			await _unitOfWork.ColorRepository.CreateAsync(color);
			await _unitOfWork.SaveChangesWithTransactionAsync();

			return new AdminColorModel
			{
				Id = color.Id,
				Name = color.Name,
				HexCode = color.HexCode,
			};
		}

		public async Task<AdminColorModel> UpdateAsync(AdminColorUpdateModel model)
		{
			var color = await _unitOfWork.ColorRepository.GetByIdAsync(model.Id);
			if (color == null)
				throw new InvalidOperationException($"Color with id {model.Id} not found");

			color.Name = model.Name;
			color.HexCode = model.HexCode;

			await _unitOfWork.ColorRepository.UpdateAsync(color);
			await _unitOfWork.SaveChangesWithTransactionAsync();

			return new AdminColorModel
			{
				Id = color.Id,
				Name = color.Name,
				HexCode = color.HexCode,
			};
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var color = await _unitOfWork.ColorRepository.GetByIdAsync(id);
			if (color == null) return false;

			return await _unitOfWork.ColorRepository.RemoveAsync(color);
		}
	}
}