using Microsoft.Extensions.Logging;
using PerHue.Domain.IRepositories;
using PerHue.Application.IServices;
using PerHue.Application.Models.AiTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Services
{
	public class ColorMatchingService : IColorMatchingService
	{
		private readonly IColorRepository _colorRepository;
		private readonly ILogger<ColorMatchingService> _logger;

		public ColorMatchingService(IColorRepository colorRepository, ILogger<ColorMatchingService> logger)
		{
			_colorRepository = colorRepository;
			_logger = logger;
		}

		public async Task<List<ColorMatchResult>> MatchColorsFromHexCodesAsync(List<string> hexCodes, List<Domain.Entities.Color> allColors)
		{
			var results = new List<ColorMatchResult>();
			var colorLookup = allColors.ToDictionary(c => c.HexCode.ToLower(), StringComparer.OrdinalIgnoreCase);

			foreach (var hexCode in hexCodes)
			{
				try
				{
					var normalizedHex = NormalizeHexCode(hexCode);

					// Try exact match first
					if (colorLookup.TryGetValue(normalizedHex.ToLower(), out var exactMatch))
					{
						results.Add(new ColorMatchResult
						{
							HexCode = hexCode,
							MatchedColor = exactMatch,
							IsExactMatch = true,
							SimilarityScore = 1.0
						});
						continue;
					}

					// Find closest color in memory
					var closestColor = FindClosestColorInMemory(normalizedHex, allColors);
					if (closestColor != null)
					{
						var similarity = CalculateSimilarity(normalizedHex, closestColor.HexCode);
						results.Add(new ColorMatchResult
						{
							HexCode = hexCode,
							MatchedColor = closestColor,
							IsExactMatch = false,
							SimilarityScore = similarity
						});
					}
					else
					{
						results.Add(new ColorMatchResult
						{
							HexCode = hexCode,
							MatchedColor = null,
							IsExactMatch = false,
							SimilarityScore = 0
						});
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error matching color: {HexCode}", hexCode);
					results.Add(new ColorMatchResult
					{
						HexCode = hexCode,
						MatchedColor = null,
						IsExactMatch = false,
						SimilarityScore = 0
					});
				}
			}

			return await Task.FromResult(results);
		}

		// Find closest color in memory (no DB query)
		private Domain.Entities.Color? FindClosestColorInMemory(string hexCode, List<Domain.Entities.Color> allColors)
		{
			var targetRgb = HexToRgb(hexCode);
			if (targetRgb == null)
				return null;

			Domain.Entities.Color? closestColor = null;
			double minDistance = double.MaxValue;

			foreach (var color in allColors)
			{
				var colorRgb = HexToRgb(color.HexCode);
				if (colorRgb == null)
					continue;

				// Tính khoảng cách Euclidean
				double distance = Math.Sqrt(
					Math.Pow(colorRgb.Value.R - targetRgb.Value.R, 2) +
					Math.Pow(colorRgb.Value.G - targetRgb.Value.G, 2) +
					Math.Pow(colorRgb.Value.B - targetRgb.Value.B, 2)
				);

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
			return $"#{hexCode.ToUpper()}";
		}

		private double CalculateSimilarity(string hex1, string hex2)
		{
			var rgb1 = HexToRgb(hex1);
			var rgb2 = HexToRgb(hex2);

			if (rgb1 == null || rgb2 == null)
				return 0;

			// Tính khoảng cách Euclidean
			double distance = Math.Sqrt(
				Math.Pow(rgb1.Value.R - rgb2.Value.R, 2) +
				Math.Pow(rgb1.Value.G - rgb2.Value.G, 2) +
				Math.Pow(rgb1.Value.B - rgb2.Value.B, 2)
			);

			// Chuyển đổi khoảng cách thành điểm tương đồng (0-1)
			double maxDistance = 441.67;
			return Math.Max(0, 1 - (distance / maxDistance));
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

		public async Task<List<Domain.Entities.Color>> GetAllColorsForMatchingAsync()
		{
			return await _colorRepository.GetAllColorsAsync();
		}

		public async Task<ColorMatchResult> FindClosestColorAsync(string hexCode)
		{
			var allColors = await _colorRepository.GetAllColorsAsync();
			var normalizedHex = NormalizeHexCode(hexCode);

			// Try exact match first
			var exactMatch = allColors.FirstOrDefault(c => string.Equals(NormalizeHexCode(c.HexCode), normalizedHex, StringComparison.OrdinalIgnoreCase));
			if (exactMatch != null)
			{
				return new ColorMatchResult
				{
					HexCode = hexCode,
					MatchedColor = exactMatch,
					IsExactMatch = true,
					SimilarityScore = 1.0
				};
			}

			// Find closest color in memory
			var closestColor = FindClosestColorInMemory(normalizedHex, allColors);
			if (closestColor != null)
			{
				var similarity = CalculateSimilarity(normalizedHex, closestColor.HexCode);
				return new ColorMatchResult
				{
					HexCode = hexCode,
					MatchedColor = closestColor,
					IsExactMatch = false,
					SimilarityScore = similarity
				};
			}

			return new ColorMatchResult
			{
				HexCode = hexCode,
				MatchedColor = null,
				IsExactMatch = false,
				SimilarityScore = 0
			};
		}
	}
}