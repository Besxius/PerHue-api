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

		public static double CalculateListSimilarity(List<string> listA, List<string> listB)
		{
			if (listA == null || listB == null || listA.Count == 0 || listB.Count == 0)
			{
				return 0;
			}

			// 1. Chuyển đổi Hex sang RGB để sử dụng ColorCalculationHelper
			var rgbA = listA.Select(ColorCalculationHelper.HexToRgb).Where(r => r.HasValue).Select(r => r.Value).ToList();
			var rgbB = listB.Select(ColorCalculationHelper.HexToRgb).Where(r => r.HasValue).Select(r => r.Value).ToList();

			if (rgbA.Count == 0 || rgbB.Count == 0)
			{
				return 0;
			}

			// Đặt danh sách ngắn hơn làm nguồn (source) và danh sách dài hơn làm đích (target)
			var sourceList = rgbA.Count <= rgbB.Count ? rgbA : rgbB;
			var targetList = rgbA.Count <= rgbB.Count ? rgbB : rgbA;

			var unmatchedTargetIndices = Enumerable.Range(0, targetList.Count).ToList();
			double totalSimilarity = 0;

			// 2. Thực hiện Greedy Matching: Ghép mỗi màu trong Source với màu chưa được ghép tốt nhất trong Target
			foreach (var sourceColor in sourceList)
			{
				double maxSimilarity = 0;
				int bestMatchIndex = -1;

				// Tìm màu gần nhất trong danh sách Target chưa được ghép
				foreach (int index in unmatchedTargetIndices)
				{
					var targetColor = targetList[index];
					// Tận dụng hàm đã có, điểm similarity là 0-100
					double similarity = ColorCalculationHelper.CalculateColorSimilarity(sourceColor, targetColor);

					if (similarity > maxSimilarity)
					{
						maxSimilarity = similarity;
						bestMatchIndex = index;
					}
				}

				// Nếu tìm thấy cặp ghép tốt nhất, thêm điểm và đánh dấu là đã ghép
				if (bestMatchIndex != -1)
				{
					totalSimilarity += maxSimilarity;
					unmatchedTargetIndices.Remove(bestMatchIndex);
				}
				else
				{
					// Trường hợp không tìm thấy cặp ghép (chủ yếu khi danh sách rỗng, đã được check ở đầu)
				}
			}

			// 3. Chuẩn hóa về tỷ lệ 0-100%
			// Chia cho kích thước của danh sách LỚN HƠN để đảm bảo kết quả phản ánh sự thiếu hụt nếu danh sách ngắn hơn nhiều
			// và để điểm số không bao giờ vượt quá 100%.
			return totalSimilarity / targetList.Count;
		}

		public static double CalculatePairSimilarity(
			PersonalColorResult resultA,
			PersonalColorResult resultB,
			double seasonWeight = 0.4, // 40%
			double recommendedWeight = 0.4, // 40%
			double avoidedWeight = 0.2 // 20%
		)
		{
			// Kiểm tra và chuẩn hóa trọng số (tổng phải là 1.0)
			double totalWeight = seasonWeight + recommendedWeight + avoidedWeight;
			if (Math.Abs(totalWeight - 1.0) > 0.001)
			{
				// Có thể log warning hoặc chuẩn hóa lại
			}

			// A. So sánh Màu Mùa (Season Score - S_Season)
			double seasonScore = (resultA.Season.Equals(resultB.Season, StringComparison.OrdinalIgnoreCase)) ? 1.0 : 0.0;

			// B. So sánh DS Màu Nên Dùng (Recommended Score - S_Rec)
			// Chia điểm similarity (0-100) cho 100 để đưa về [0, 1]
			double recommendedScore = CalculateListSimilarity(resultA.RecommendedColors, resultB.RecommendedColors) / 100.0;

			// C. So sánh DS Màu Nên Tránh (Avoided Score - S_Avoid)
			// Chia điểm similarity (0-100) cho 100 để đưa về [0, 1]
			double avoidedScore = CalculateListSimilarity(resultA.AvoidedColors, resultB.AvoidedColors) / 100.0;

			// D. Tính điểm tổng thể (0-1.0)
			double overallSimilarity =
				(seasonScore * seasonWeight) +
				(recommendedScore * recommendedWeight) +
				(avoidedScore * avoidedWeight);

			// Trả về kết quả dưới dạng phần trăm (0-100)
			return overallSimilarity * 100.0;
		}
		public static double CalculateThreeResultSimilarity(
			PersonalColorResult r1,
			PersonalColorResult r2,
			PersonalColorResult r3,
			double seasonWeight = 0.4,
			double recommendedWeight = 0.4,
			double avoidedWeight = 0.2
		)
		{
			// 1. Tính điểm giống nhau cho từng cặp
			double p12 = CalculatePairSimilarity(r1, r2, seasonWeight, recommendedWeight, avoidedWeight);
			double p13 = CalculatePairSimilarity(r1, r3, seasonWeight, recommendedWeight, avoidedWeight);
			double p23 = CalculatePairSimilarity(r2, r3, seasonWeight, recommendedWeight, avoidedWeight);

			// 2. Tính điểm trung bình (trên 3 cặp)
			double totalSimilarity = (p12 + p13 + p23) / 3.0;

			return totalSimilarity; // Trả về giá trị %
		}
	}
	public class PersonalColorResult
	{
		public string Season { get; set; } // Ví dụ: "Light Spring"
		public List<string> RecommendedColors { get; set; } // List of Hex codes (e.g., "#FF0000")
		public List<string> AvoidedColors { get; set; }   // List of Hex codes
	}
}