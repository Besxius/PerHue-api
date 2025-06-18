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
	}
}
