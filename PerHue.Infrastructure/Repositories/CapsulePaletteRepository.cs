using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class CapsulePaletteRepository : GenericRepository<CapsulePalette>, ICapsulePaletteRepository
	{
		public CapsulePaletteRepository(PerHueDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<CapsulePalette>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm)
		{
			var query = _context.CapsulePalettes.Include(p => p.ColorType).Include(p => p.Colors).AsQueryable();
			if (!string.IsNullOrEmpty(searchTerm) && !searchTerm.StartsWith("#"))
			{
				query = query.Where(p => p.ColorType.Name.Contains(searchTerm)
									|| p.Colors.Any(c => c.Name.Contains(searchTerm)));
			}
			if (!string.IsNullOrEmpty(searchTerm) && searchTerm.StartsWith("#") && searchTerm.Length == 7)
			{
				// Chuyển đổi searchTerm thành giá trị RGB
				var searchR = Convert.ToInt32(searchTerm.Substring(1, 2), 16);
				var searchG = Convert.ToInt32(searchTerm.Substring(3, 2), 16);
				var searchB = Convert.ToInt32(searchTerm.Substring(5, 2), 16);

				query = query.Where(p => p.Colors.Any(cl => cl.HexCode.StartsWith("#") && cl.HexCode.Length == 7));
				// Lọc các mã hex hợp lệ trong SQL
				var filteredColors = await query.ToListAsync();

				// Tính toán khoảng cách Euclidean trong bộ nhớ
				filteredColors = filteredColors
					.Where(c => c.Colors.Any(cl =>
					{
						var colorR = Convert.ToInt32(cl.HexCode.Substring(1, 2), 16);
						var colorG = Convert.ToInt32(cl.HexCode.Substring(3, 2), 16);
						var colorB = Convert.ToInt32(cl.HexCode.Substring(5, 2), 16);

						var distance = Math.Sqrt(Math.Pow(colorR - searchR, 2) +
												 Math.Pow(colorG - searchG, 2) +
												 Math.Pow(colorB - searchB, 2));

						// Giới hạn khoảng cách (ví dụ: 50)
						return distance <= 50;
					})
					).GroupBy(p => p.ColorTypeId)
					.SelectMany(g => g)
					.ToList();

				return filteredColors.Skip((pageIndex - 1) * pageSize).Take(pageSize);
			}
			query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
			return await query.ToListAsync();
		}

		public async Task<CapsulePalette> GetByIdAsync(int id)
		{
			return await _context.CapsulePalettes
				.Include(p => p.ColorType)
				.Include(p => p.Colors)
				.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<IEnumerable<CapsulePalette>> GetRelativeCapsulePalettes(List<string> selectedColors)
		{
			var colorsList = new List<CapsulePalette>();

			foreach (var color in selectedColors)
			{
				var query = _context.CapsulePalettes.Include(p => p.ColorType).Include(p => p.Colors).AsQueryable();
				if (!string.IsNullOrEmpty(color) && color.StartsWith("#") && color.Length == 7)
				{
					// Chuyển đổi searchTerm thành giá trị RGB
					var searchR = Convert.ToInt32(color.Substring(1, 2), 16);
					var searchG = Convert.ToInt32(color.Substring(3, 2), 16);
					var searchB = Convert.ToInt32(color.Substring(5, 2), 16);

					query = query.Where(p => p.Colors.Any(cl => cl.HexCode.StartsWith("#") && cl.HexCode.Length == 7));
					// Lọc các mã hex hợp lệ trong SQL
					var filteredColors = await query.ToListAsync();

					// Tính toán khoảng cách Euclidean trong bộ nhớ
					filteredColors = filteredColors
						.Where(c => c.Colors.Any(cl =>
						{
							var colorR = Convert.ToInt32(cl.HexCode.Substring(1, 2), 16);
							var colorG = Convert.ToInt32(cl.HexCode.Substring(3, 2), 16);
							var colorB = Convert.ToInt32(cl.HexCode.Substring(5, 2), 16);

							var distance = Math.Sqrt(Math.Pow(colorR - searchR, 2) +
													 Math.Pow(colorG - searchG, 2) +
													 Math.Pow(colorB - searchB, 2));

							// Giới hạn khoảng cách (ví dụ: 50)
							return distance <= 50;
						})).ToList();

					colorsList.AddRange(filteredColors);
				}
			}
			var result = colorsList.DistinctBy(p => p.Id).GroupBy(p => p.ColorTypeId).SelectMany(g => g);

			result = result.Where(p => p.ColorTypeId == result.First().ColorTypeId)
				.ToList();
			return result;
		}
		public async Task<IEnumerable<CapsulePalette>> GetRelativeCapsulePalettes(List<string> selectedColors, string colorType)
		{
			var colorsList = new List<CapsulePalette>();

			foreach (var color in selectedColors)
			{
				var query = _context.CapsulePalettes.Include(p => p.ColorType).Include(p => p.Colors).AsQueryable();
				if (!string.IsNullOrEmpty(color) && color.StartsWith("#") && color.Length == 7)
				{
					// Chuyển đổi searchTerm thành giá trị RGB
					var searchR = Convert.ToInt32(color.Substring(1, 2), 16);
					var searchG = Convert.ToInt32(color.Substring(3, 2), 16);
					var searchB = Convert.ToInt32(color.Substring(5, 2), 16);

					query = query.Where(p => p.Colors.Any(cl => cl.HexCode.StartsWith("#") && cl.HexCode.Length == 7));
					// Lọc các mã hex hợp lệ trong SQL
					var filteredColors = await query.ToListAsync();

					// Tính toán khoảng cách Euclidean trong bộ nhớ
					filteredColors = filteredColors
						.Where(c => c.Colors.Any(cl =>
						{
							var colorR = Convert.ToInt32(cl.HexCode.Substring(1, 2), 16);
							var colorG = Convert.ToInt32(cl.HexCode.Substring(3, 2), 16);
							var colorB = Convert.ToInt32(cl.HexCode.Substring(5, 2), 16);

							var distance = Math.Sqrt(Math.Pow(colorR - searchR, 2) +
													 Math.Pow(colorG - searchG, 2) +
													 Math.Pow(colorB - searchB, 2));

							// Giới hạn khoảng cách (ví dụ: 50)
							return distance <= 50;
						})).ToList();

					colorsList.AddRange(filteredColors);
				}
			}
			var result = colorsList.DistinctBy(p => p.Id).GroupBy(p => p.ColorTypeId).SelectMany(g => g);

			var colorTypeId = _context.ColorTypes.FirstOrDefault(ct => ct.Name.ToLower().Equals(colorType.ToLower())).Id;

			result = result.Where(p => p.ColorTypeId == colorTypeId)
				.ToList();
			return result;
		}
		public async Task<IEnumerable<CapsulePalette>> GetByColorTypeIdAsync(int colorTypeId)
		{
			return await _context.CapsulePalettes
				.Include(cp => cp.Colors)     // Include colors in the palette
				.Include(cp => cp.ColorType)  // Include the type details
				.Where(cp => cp.ColorTypeId == colorTypeId)
				.ToListAsync();
		}

		public async Task<(IEnumerable<CapsulePalette> Items, int TotalCount)> GetByColorTypeIdPagedAsync(int colorTypeId, int pageIndex, int pageSize, string? searchTerm)
		{
			var query = _context.CapsulePalettes
				.Include(cp => cp.Colors)
				.Include(cp => cp.ColorType)
				.Where(cp => cp.ColorTypeId == colorTypeId);

			if (!string.IsNullOrEmpty(searchTerm))
			{
				// Search Logic:
				// 1. Match ColorType Name (e.g., "Winter")
				// 2. Match ANY Color Name inside the palette (e.g., "Red")
				// 3. Match ANY Color HexCode inside the palette (e.g., "#FF0000")
				query = query.Where(cp =>
					cp.ColorType.Name.Contains(searchTerm) ||
					cp.Colors.Any(c => c.Name.Contains(searchTerm) || c.HexCode.Contains(searchTerm))
				);
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		public async Task<IEnumerable<CapsulePalette>> GetListByColorsIdAsync(List<int> colorIds)
		{
			return await _context.CapsulePalettes
				.Include(cp => cp.ColorType)
				.Include(cp => cp.Colors)
				.Where(cp => cp.Colors.Any(c => colorIds.Contains(c.Id)))
				.ToListAsync();
		}
	}
}
