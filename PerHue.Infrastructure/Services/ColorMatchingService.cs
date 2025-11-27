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

		public async Task<List<ColorMatchResult>> MatchColorsFromHexCodesAsync(List<string> hexCodes)
		{
			var results = new List<ColorMatchResult>();

			foreach (var hexCode in hexCodes)
			{
				var result = await FindClosestColorAsync(hexCode);
				results.Add(result);
			}

			return results;
		}

		public async Task<ColorMatchResult> FindClosestColorAsync(string hexCode)
		{
			try
			{
				hexCode = NormalizeHexCode(hexCode);

				// Th? tìm exact match tr??c
				var exactMatch = await _colorRepository.GetColorByHexCodeAsync(hexCode);
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

				// N?u không có exact match, tìm màu g?n nh?t
				var closestColor = await _colorRepository.FindClosestColorByHexAsync(hexCode);
				if (closestColor != null)
				{
					var similarity = CalculateSimilarity(hexCode, closestColor.HexCode);
					return new ColorMatchResult
					{
						HexCode = hexCode,
						MatchedColor = closestColor,
						IsExactMatch = false,
						SimilarityScore = similarity
					};
				}

				// Không tìm th?y màu nào
				return new ColorMatchResult
				{
					HexCode = hexCode,
					MatchedColor = null,
					IsExactMatch = false,
					SimilarityScore = 0
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error matching color: {HexCode}", hexCode);
				return new ColorMatchResult
				{
					HexCode = hexCode,
					MatchedColor = null,
					IsExactMatch = false,
					SimilarityScore = 0
				};
			}
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

			// Tính kho?ng cách Euclidean
			double distance = Math.Sqrt(
				Math.Pow(rgb1.Value.R - rgb2.Value.R, 2) +
				Math.Pow(rgb1.Value.G - rgb2.Value.G, 2) +
				Math.Pow(rgb1.Value.B - rgb2.Value.B, 2)
			);

			// Chuy?n ??i kho?ng cách thành ?i?m t??ng ??ng (0-1)
			// Kho?ng cách max trong RGB là sqrt(255^2 * 3) ? 441.67
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
	}
}