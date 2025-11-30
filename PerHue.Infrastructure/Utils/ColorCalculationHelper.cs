using System;

namespace PerHue.Infrastructure.Utils
{
	public static class ColorCalculationHelper
	{
		/// <summary>
		/// Chuyển đổi hex code sang RGB
		/// </summary>
		public static (int R, int G, int B)? HexToRgb(string hexCode)
		{
			try
			{
				hexCode = NormalizeHexCode(hexCode).TrimStart('#');
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

		/// <summary>
		/// Chuẩn hóa hex code (#RRGGBB)
		/// </summary>
		public static string NormalizeHexCode(string hexCode)
		{
			hexCode = hexCode.Trim().TrimStart('#');
			if (hexCode.Length == 3)
			{
				hexCode = string.Concat(hexCode.Select(c => $"{c}{c}"));
			}
			return $"#{hexCode.ToUpper()}";
		}

		/// <summary>
		/// Tính khoảng cách Euclidean giữa 2 màu RGB (0-441.67)
		/// </summary>
		public static double CalculateEuclideanDistance((int R, int G, int B) color1, (int R, int G, int B) color2)
		{
			double rDiff = color1.R - color2.R;
			double gDiff = color1.G - color2.G;
			double bDiff = color1.B - color2.B;

			return Math.Sqrt((rDiff * rDiff) + (gDiff * gDiff) + (bDiff * bDiff));
		}

		/// <summary>
		/// Chuyển đổi RGB sang LAB color space (chính xác hơn cho perception)
		/// </summary>
		public static (double L, double A, double B) RgbToLab((int R, int G, int B) rgb)
		{
			// RGB to XYZ
			double r = rgb.R / 255.0;
			double g = rgb.G / 255.0;
			double b = rgb.B / 255.0;

			r = r > 0.04045 ? Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92;
			g = g > 0.04045 ? Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92;
			b = b > 0.04045 ? Math.Pow((b + 0.055) / 1.055, 2.4) : b / 12.92;

			double x = (r * 0.4124564 + g * 0.3575761 + b * 0.1804375) * 100;
			double y = (r * 0.2126729 + g * 0.7151522 + b * 0.0721750) * 100;
			double z = (r * 0.0193339 + g * 0.1191920 + b * 0.9503041) * 100;

			// XYZ to LAB (D65 illuminant)
			x /= 95.047;
			y /= 100.000;
			z /= 108.883;

			x = x > 0.008856 ? Math.Pow(x, 1.0 / 3.0) : (7.787 * x) + (16.0 / 116.0);
			y = y > 0.008856 ? Math.Pow(y, 1.0 / 3.0) : (7.787 * y) + (16.0 / 116.0);
			z = z > 0.008856 ? Math.Pow(z, 1.0 / 3.0) : (7.787 * z) + (16.0 / 116.0);

			double L = (116.0 * y) - 16.0;
			double A = 500.0 * (x - y);
			double B = 200.0 * (y - z);

			return (L, A, B);
		}

		/// <summary>
		/// Tính khoảng cách Delta E (CIE76) - chuẩn công nghiệp cho sự khác biệt màu
		/// Giá trị < 2.3: không thể phân biệt bằng mắt thường
		/// Giá trị 2.3-10: khác biệt nhỏ
		/// Giá trị > 10: khác biệt rõ rệt
		/// </summary>
		public static double CalculateDeltaE((int R, int G, int B) color1, (int R, int G, int B) color2)
		{
			var lab1 = RgbToLab(color1);
			var lab2 = RgbToLab(color2);

			double deltaL = lab1.L - lab2.L;
			double deltaA = lab1.A - lab2.A;
			double deltaB = lab1.B - lab2.B;

			return Math.Sqrt((deltaL * deltaL) + (deltaA * deltaA) + (deltaB * deltaB));
		}

		/// <summary>
		/// Tính độ tương đồng màu (0-100%, 100% = giống nhất)
		/// Sử dụng Delta E với ngưỡng tối đa = 100
		/// </summary>
		public static double CalculateColorSimilarity((int R, int G, int B) color1, (int R, int G, int B) color2)
		{
			double deltaE = CalculateDeltaE(color1, color2);
			// Normalize về scale 0-100, với Delta E max = 100
			double similarity = Math.Max(0, 100 - deltaE);
			return similarity;
		}

		/// <summary>
		/// Xác định xem 2 màu có "tương tự" không dựa trên ngưỡng Delta E
		/// </summary>
		public static bool AreColorsSimilar((int R, int G, int B) color1, (int R, int G, int B) color2, double threshold = 30.0)
		{
			double deltaE = CalculateDeltaE(color1, color2);
			return deltaE <= threshold;
		}

		/// <summary>
		/// Tính điểm relevance (độ liên quan) giữa màu input và màu trong database
		/// Trả về giá trị 0-100
		/// </summary>
		public static double CalculateRelevanceScore((int R, int G, int B) inputColor, (int R, int G, int B) dbColor, double weight = 1.0)
		{
			double similarity = CalculateColorSimilarity(inputColor, dbColor);
			return similarity * weight;
		}
	}
}