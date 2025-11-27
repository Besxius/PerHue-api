using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PerHue.Application.Basic;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.CapsulePalette;
using PerHue.Application.Models.Color;
using PerHue.Application.Models.ColorType;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;

namespace PerHue.Infrastructure.Services
{
	internal class AdminCapsulePaletteService : IAdminCapsulePaletteService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public AdminCapsulePaletteService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<IEnumerable<AdminCapsulePaletteModel>> GetAllAsync()
		{
			var palettes = await _unitOfWork.CapsulePaletteRepository.GetQueryable()
				.Include(cp => cp.ColorType)
				.Include(cp => cp.Colors)
				.ToListAsync();

			return palettes.Select(cp => new AdminCapsulePaletteModel
			{
				Id = cp.Id,
				ColorTypeId = cp.ColorTypeId,
				ColorTypeName = cp.ColorType.Name,
				Colors = cp.Colors.Select(c => new AdminColorModel
				{
					Id = c.Id,
					Name = c.Name,
					HexCode = c.HexCode,
				}).ToList(),
			});
		}

		public async Task<PaginatedResultV2<AdminCapsulePaletteModel>> GetAllAsync(AdminCapsulePaletteSearchModel searchModel)
		{
			var baseQuery = _unitOfWork.CapsulePaletteRepository.GetQueryable()
				.Include(cp => cp.ColorType)
				.Include(cp => cp.Colors);

			IQueryable<CapsulePalette> query = baseQuery;

			// Apply search filters
			if (!string.IsNullOrEmpty(searchModel.SearchTerm))
			{
				switch (searchModel.SearchBy?.ToLower())
				{
					case "id":
						if (int.TryParse(searchModel.SearchTerm, out int id))
						{
							query = query.Where(cp => cp.Id == id);
						}
						break;
					case "colortype":
						query = query.Where(cp => cp.ColorType.Name.Contains(searchModel.SearchTerm));
						break;
					default:
						query = query.Where(cp => cp.ColorType.Name.Contains(searchModel.SearchTerm));
						break;
				}
			}

			// Apply ColorTypeId filter
			if (searchModel.ColorTypeId.HasValue)
			{
				query = query.Where(cp => cp.ColorTypeId == searchModel.ColorTypeId.Value);
			}

			// Apply sorting
			IOrderedQueryable<CapsulePalette> orderedQuery;
			switch (searchModel.SortBy?.ToLower())
			{
				case "id":
					orderedQuery = searchModel.SortOrder?.ToLower() == "asc"
						? query.OrderBy(cp => cp.Id)
						: query.OrderByDescending(cp => cp.Id);
					break;
				case "colortype":
					orderedQuery = searchModel.SortOrder?.ToLower() == "asc"
						? query.OrderBy(cp => cp.ColorType.Name)
						: query.OrderByDescending(cp => cp.ColorType.Name);
					break;
				case "createddate":
				default:
					orderedQuery = searchModel.SortOrder?.ToLower() == "asc"
						? query.OrderBy(cp => cp.Id) // Using Id as proxy for creation date
						: query.OrderByDescending(cp => cp.Id);
					break;
			}

			var totalCount = await query.CountAsync();

			var palettes = await orderedQuery
				.Skip((searchModel.PageIndex - 1) * searchModel.PageSize)
				.Take(searchModel.PageSize)
				.Select(cp => new AdminCapsulePaletteModel
				{
					Id = cp.Id,
					ColorTypeId = cp.ColorTypeId,
					ColorTypeName = cp.ColorType.Name,
					Colors = cp.Colors.Select(c => new AdminColorModel
					{
						Id = c.Id,
						Name = c.Name,
						HexCode = c.HexCode,
					}).ToList(),
				})
				.ToListAsync();

			return new PaginatedResultV2<AdminCapsulePaletteModel>
			{
				List = palettes,
				Total = totalCount,
				Current = searchModel.PageIndex,
			};
		}

		async Task<AdminCapsulePaletteModel> IGenericService<AdminCapsulePaletteModel>.GetByIdAsync(int id)
		{
			var palette = await _unitOfWork.CapsulePaletteRepository.GetQueryable()
				.Include(cp => cp.ColorType)
				.Include(cp => cp.Colors)
				.FirstOrDefaultAsync(cp => cp.Id == id);

			if (palette == null) throw new InvalidOperationException($"CapsulePalette with id {id} not found");

			return new AdminCapsulePaletteModel
			{
				Id = palette.Id,
				ColorTypeId = palette.ColorTypeId,
				ColorTypeName = palette.ColorType.Name,
				Colors = palette.Colors.Select(c => new AdminColorModel
				{
					Id = c.Id,
					Name = c.Name,
					HexCode = c.HexCode,
				}).ToList(),
			};
		}

		public async Task<AdminCapsulePaletteModel?> GetByIdAsync(int id)
		{
			var palette = await _unitOfWork.CapsulePaletteRepository.GetQueryable()
				.Include(cp => cp.ColorType)
				.Include(cp => cp.Colors)
				.FirstOrDefaultAsync(cp => cp.Id == id);

			if (palette == null) return null;

			return new AdminCapsulePaletteModel
			{
				Id = palette.Id,
				ColorTypeId = palette.ColorTypeId,
				ColorTypeName = palette.ColorType.Name,
				Colors = palette.Colors.Select(c => new AdminColorModel
				{
					Id = c.Id,
					Name = c.Name,
					HexCode = c.HexCode,
				}).ToList(),
			};
		}

		public async Task<AdminCapsulePaletteModel> CreateAsync(AdminCapsulePaletteCreateModel model)
		{
			var capsulePalette = new CapsulePalette
			{
				ColorTypeId = model.ColorTypeId
			};

			// validate at least 4 colors?
			if (model.ColorIds != null && model.ColorIds.Count < 4)
			{
				throw new InvalidOperationException("A capsule palette must contain at least 4 colors.");
			}

			await _unitOfWork.CapsulePaletteRepository.CreateAsync(capsulePalette);
			await _unitOfWork.SaveChangesWithTransactionAsync();

			// Add colors to the palette if specified
			if (model.ColorIds != null && model.ColorIds.Any())
			{
				var colors = await _unitOfWork.ColorRepository.GetQueryable()
					.Where(c => model.ColorIds.Contains(c.Id))
					.ToListAsync();

				// Since CapsulePalette has a many-to-many relationship with Color,
				// we need to add colors to the Colors collection
				foreach (var color in colors)
				{
					capsulePalette.Colors.Add(color);
				}

				await _unitOfWork.SaveChangesWithTransactionAsync();
			}

			// Reload the entity with related data
			var createdPalette = await GetByIdAsync(capsulePalette.Id);
			if (createdPalette == null)
				throw new InvalidOperationException("Failed to create capsule palette");

			return createdPalette;
		}

		public async Task<AdminCapsulePaletteModel> UpdateAsync(AdminCapsulePaletteUpdateModel model)
		{
			// Validate at least 4 colors?
			if (model.ColorIds != null && model.ColorIds.Count < 4)
			{
				throw new InvalidOperationException("A capsule palette must contain at least 4 colors.");
			}

			var capsulePalette = await _unitOfWork.CapsulePaletteRepository.GetQueryable()
				.Include(cp => cp.Colors)
				.FirstOrDefaultAsync(cp => cp.Id == model.Id) ?? throw new InvalidOperationException($"CapsulePalette with id {model.Id} not found");

			// Update ColorTypeId
			capsulePalette.ColorTypeId = model.ColorTypeId;

			// Update colors in the palette
			if (model.ColorIds != null)
			{
				// Clear existing colors
				capsulePalette.Colors.Clear();

				// Add new colors
				if (model.ColorIds.Any())
				{
					var colors = await _unitOfWork.ColorRepository.GetQueryable()
						.Where(c => model.ColorIds.Contains(c.Id))
						.ToListAsync();

					foreach (var color in colors)
					{
						capsulePalette.Colors.Add(color);
					}
				}
			}

			await _unitOfWork.SaveChangesWithTransactionAsync();

			// Reload the entity with related data
			var updatedPalette = await GetByIdAsync(capsulePalette.Id);
			if (updatedPalette == null)
				throw new InvalidOperationException("Failed to update capsule palette");

			return updatedPalette;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var capsulePalette = await _unitOfWork.CapsulePaletteRepository.GetByIdAsync(id);
			if (capsulePalette == null) return false;

			return await _unitOfWork.CapsulePaletteRepository.RemoveAsync(capsulePalette);
		}
	}
}