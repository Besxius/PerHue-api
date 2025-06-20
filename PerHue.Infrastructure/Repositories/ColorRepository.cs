using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class ColorRepository : GenericRepository<Color>, IColorRepository
	{
		public ColorRepository(PerHueDbContext context) : base(context)
		{
		}
		public async Task<IEnumerable<Color>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm)
		{
			var query = _context.Colors.AsQueryable();
			if (!string.IsNullOrEmpty(searchTerm) && !searchTerm.StartsWith("#"))
			{
				query = query.Where(c => c.Name.Contains(searchTerm));
			}
			if (!string.IsNullOrEmpty(searchTerm) && searchTerm.StartsWith("#") && searchTerm.Length == 7)
			{
				// Chuyển đổi searchTerm thành giá trị RGB
				var searchR = Convert.ToInt32(searchTerm.Substring(1, 2), 16);
				var searchG = Convert.ToInt32(searchTerm.Substring(3, 2), 16);
				var searchB = Convert.ToInt32(searchTerm.Substring(5, 2), 16);

				query = query.Where(c => c.HexCode.StartsWith("#") && c.HexCode.Length == 7);
				// Lọc các mã hex hợp lệ trong SQL
				var filteredColors = await query.ToListAsync();

				// Tính toán khoảng cách Euclidean trong bộ nhớ
				filteredColors = filteredColors
					.Where(c =>
					{
						var colorR = Convert.ToInt32(c.HexCode.Substring(1, 2), 16);
						var colorG = Convert.ToInt32(c.HexCode.Substring(3, 2), 16);
						var colorB = Convert.ToInt32(c.HexCode.Substring(5, 2), 16);

						var distance = Math.Sqrt(Math.Pow(colorR - searchR, 2) +
												 Math.Pow(colorG - searchG, 2) +
												 Math.Pow(colorB - searchB, 2));

						// Giới hạn khoảng cách (ví dụ: 50)
						return distance <= 50;
					})
					.ToList();

				return filteredColors.Skip((pageIndex - 1) * pageSize).Take(pageSize);
			}
			query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
			return await query.ToListAsync();
		}

		public async Task<IEnumerable<Color>> GetRelativeColors(List<string> selectedColors)
		{
			var colorsList = new List<Color>();
			foreach (var color in selectedColors)
			{
				var query = _context.Colors.AsQueryable();
				if (!string.IsNullOrEmpty(color) && color.StartsWith("#") && color.Length == 7)
				{
					// Chuyển đổi searchTerm thành giá trị RGB
					var searchR = Convert.ToInt32(color.Substring(1, 2), 16);
					var searchG = Convert.ToInt32(color.Substring(3, 2), 16);
					var searchB = Convert.ToInt32(color.Substring(5, 2), 16);

					query = query.Where(c => c.HexCode.StartsWith("#") && c.HexCode.Length == 7);
					// Lọc các mã hex hợp lệ trong SQL
					var filteredColors = await query.ToListAsync();

					// Tính toán khoảng cách Euclidean trong bộ nhớ
					filteredColors = filteredColors
						.Where(c =>
						{
							var colorR = Convert.ToInt32(c.HexCode.Substring(1, 2), 16);
							var colorG = Convert.ToInt32(c.HexCode.Substring(3, 2), 16);
							var colorB = Convert.ToInt32(c.HexCode.Substring(5, 2), 16);

							var distance = Math.Sqrt(Math.Pow(colorR - searchR, 2) +
													 Math.Pow(colorG - searchG, 2) +
													 Math.Pow(colorB - searchB, 2));

							// Giới hạn khoảng cách (ví dụ: 50)
							return distance <= 50;
						})
						.ToList();
					colorsList.AddRange(filteredColors);
				}
			}
			return colorsList;
		}
	}
}
