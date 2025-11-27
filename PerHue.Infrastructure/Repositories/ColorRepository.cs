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

		public async Task<IEnumerable<Color>> GetByColorTypeIdAsync(int colorTypeId)
		{
			// FIX: Query CapsulePalette directly and flatten the Colors collection
			return await _context.Set<CapsulePalette>()
				.Where(cp => cp.ColorTypeId == colorTypeId)
				.SelectMany(cp => cp.Colors) // Selects all colors from the matching palettes
				.Distinct() // Removes duplicates if a color appears in multiple palettes
				.ToListAsync();
		}

		public async Task<IEnumerable<Color>> GetByColorTypeIdAsync(int colorTypeId, int pageIndex, int pageSize, string? searchTerm)
		{
			// FIX: Query CapsulePalette directly
			var query = _context.Set<CapsulePalette>()
				.Where(cp => cp.ColorTypeId == colorTypeId)
				.SelectMany(cp => cp.Colors)
				.Distinct();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				query = query.Where(c => c.Name.Contains(searchTerm) || c.HexCode.Contains(searchTerm));
			}

			return await query
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}

		public async Task<int> CountByColorTypeIdAsync(int colorTypeId, string? searchTerm)
		{
			// FIX: Query CapsulePalette directly
			var query = _context.Set<CapsulePalette>()
				.Where(cp => cp.ColorTypeId == colorTypeId)
				.SelectMany(cp => cp.Colors)
				.Distinct();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				query = query.Where(c => c.Name.Contains(searchTerm) || c.HexCode.Contains(searchTerm));
			}

			return await query.CountAsync();
		}

		public async Task<IEnumerable<Color>> GetAllBySpectrumAsync()
		{
			// 1. Fetch all colors into memory first
			var entities = await _context.Colors.ToListAsync();

			// 2. Perform the complex sorting in memory
			return entities
				.Select(c =>
				{
					var hsl = GetHslFromHex(c.HexCode);
					// Assign a bucket index (0=Red, 1=Orange... 9=Gray)
					int familyIndex = GetColorFamilyIndex(hsl.H, hsl.S, hsl.L);
					return new { Color = c, HSL = hsl, Family = familyIndex };
				})
				// 3. Group by Color Family (Red -> Orange -> ... -> Pink -> Gray)
				.OrderBy(x => x.Family)
				// 4. Sort by Lightness DESC (Lightest Tints -> Darkest Shades)
				.ThenByDescending(x => x.HSL.L)
				.Select(x => x.Color)
				.ToList();
		}
		public async Task<(IEnumerable<Color> Items, int TotalCount)> GetAllBySpectrumPagedAsync(int pageIndex, int pageSize, string? searchTerm)
		{
			// 1. Fetch all colors into memory first
			var entities = await _context.Colors.ToListAsync();

			// 2. Apply Search Filter (In-Memory)
			if (!string.IsNullOrEmpty(searchTerm))
			{
				searchTerm = searchTerm.ToLower();
				entities = entities.Where(c =>
					(c.Name != null && c.Name.ToLower().Contains(searchTerm)) ||
					c.HexCode.ToLower().Contains(searchTerm)
				).ToList();
			}

			// 3. Perform the complex sorting in memory
			var sortedEntities = entities
				.Select(c =>
				{
					var hsl = GetHslFromHex(c.HexCode);
					int familyIndex = GetColorFamilyIndex(hsl.H, hsl.S, hsl.L);
					return new { Color = c, HSL = hsl, Family = familyIndex };
				})
				.OrderBy(x => x.Family)
				.ThenByDescending(x => x.HSL.L)
				.Select(x => x.Color)
				.ToList();

			// 4. Apply Pagination
			var totalCount = sortedEntities.Count;
			var pagedItems = sortedEntities
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			return (pagedItems, totalCount);
		}

		// --- PRIVATE HELPER METHODS (Moved from Service) ---

		private int GetColorFamilyIndex(float h, float s, float l)
		{
			// 1. Filter Neutrals (Grayscale)
			if (s < 0.12f || l > 0.98f || l < 0.05f)
			{
				return 100; // Grays at the end
			}

			// 2. Map Hue (0-360) to Families
			if (h >= 345 || h < 15) return 0;  // Red
			if (h >= 15 && h < 45) return 1;  // Orange
			if (h >= 45 && h < 70) return 2;  // Yellow / Amber
			if (h >= 70 && h < 150) return 3;  // Green / Lime
			if (h >= 150 && h < 200) return 4; // Teal / Cyan
			if (h >= 200 && h < 260) return 5; // Blue
			if (h >= 260 && h < 300) return 6; // Purple / Violet
			return 7;                          // Pink / Rose
		}

		private (float H, float S, float L) GetHslFromHex(string hex)
		{
			try
			{
				if (string.IsNullOrEmpty(hex)) return (0, 0, 0);
				hex = hex.TrimStart('#');
				if (hex.Length == 3) hex = string.Join("", hex.Select(c => new string(c, 2)));
				if (hex.Length != 6) return (0, 0, 0);

				int r = Convert.ToInt32(hex.Substring(0, 2), 16);
				int g = Convert.ToInt32(hex.Substring(2, 2), 16);
				int b = Convert.ToInt32(hex.Substring(4, 2), 16);

				float rf = r / 255f;
				float gf = g / 255f;
				float bf = b / 255f;

				float max = Math.Max(rf, Math.Max(gf, bf));
				float min = Math.Min(rf, Math.Min(gf, bf));
				float delta = max - min;

				float h = 0;
				float s = 0;
				float l = (max + min) / 2;

				if (delta != 0)
				{
					s = l < 0.5 ? (delta / (max + min)) : (delta / (2.0f - max - min));

					if (max == rf) h = (gf - bf) / delta + (gf < bf ? 6 : 0);
					else if (max == gf) h = (bf - rf) / delta + 2;
					else h = (rf - gf) / delta + 4;

					h *= 60;
				}

				if (h < 0) h += 360;

				return (h, s, l);
			}
			catch
			{
				return (0, 0, 0);
			}
		}

		public async Task<List<Color>> GetAllColorsAsync()
		{
			return await _context.Colors
				.ToListAsync();
		}

		public async Task<Color?> GetColorByHexCodeAsync(string hexCode)
		{
			hexCode = NormalizeHexCode(hexCode);
			return await _dbSet
				//.Include(c => c.ColorType)
				.FirstOrDefaultAsync(c => c.HexCode.ToLower() == hexCode.ToLower());
		}

		public async Task<List<Color>> GetColorsByColorTypeIdAsync(int colorTypeId)
		{
			return await _dbSet
				//.Include(c => c.ColorType)
				//.Where(c => c.ColorTypeId == colorTypeId)
				.ToListAsync();
		}

		public async Task<Color?> FindClosestColorByHexAsync(string hexCode)
		{
			hexCode = NormalizeHexCode(hexCode);
			var allColors = await GetAllAsync();

			if (!allColors.Any())
				return null;

			var targetRgb = HexToRgb(hexCode);
			if (targetRgb == null)
				return null;

			Color? closestColor = null;
			double minDistance = double.MaxValue;

			foreach (var color in allColors)
			{
				var colorRgb = HexToRgb(color.HexCode);
				if (colorRgb == null)
					continue;

				var distance = CalculateColorDistance(targetRgb.Value, colorRgb.Value);
				if (distance < minDistance)
				{
					minDistance = distance;
					closestColor = color;
				}
			}

			return closestColor;
		}

		private string NormalizeHexCode(string hexCode)
		{
			hexCode = hexCode.Trim().TrimStart('#');
			if (hexCode.Length == 3)
			{
				hexCode = string.Concat(hexCode.Select(c => $"{c}{c}"));
			}
			return $"#{hexCode}";
		}

		private (int R, int G, int B)? HexToRgb(string hexCode)
		{
			try
			{
				hexCode = hexCode.TrimStart('#');
				if (hexCode.Length != 6)
					return null;

				int r = Convert.ToInt32(hexCode.Substring(0, 2), 16);
				int g = Convert.ToInt32(hexCode.Substring(2, 2), 16);
				int b = Convert.ToInt32(hexCode.Substring(4, 2), 16);

				return (r, g, b);
			}
			catch
			{
				return null;
			}
		}

		private double CalculateColorDistance((int R, int G, int B) color1, (int R, int G, int B) color2)
		{
			double rDiff = color1.R - color2.R;
			double gDiff = color1.G - color2.G;
			double bDiff = color1.B - color2.B;

			return Math.Sqrt((rDiff * rDiff) + (gDiff * gDiff) + (bDiff * bDiff));
		}
	}
}
