using Microsoft.AspNetCore.Http;
using PerHue.Application.Attributes;
using PerHue.Application.Models.CapsulePalette;
using PerHue.Application.Models.Color;
using PerHue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.AiTest
{
	public class CreateAiTestRequestModel
	{
		public string? HairColor { get; set; }
		public string? EyesColor { get; set; }
		public string? LipsColor { get; set; }
		public string? SkinColor { get; set; }
		public List<IFormFile> Images { get; set; } = new();
		public List<string> Environments { get; set; } = new(); // ["Snow", "Hot", "Sunny", "Cold"]
	}

	public class AiTestResponseModel
	{
		public int TestRequestId { get; set; }
		public string Status { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; }
		public AiTestResultDetailModel? Result { get; set; }
		public List<GeneratedImageModel> GeneratedImages { get; set; } = new();
	}

	public class AiTestResultDetailModel
	{
		public string ColorType { get; set; } = string.Empty;
		public int ColorTypeId { get; set; }
		public List<ColorInfoModel> SuggestedColors { get; set; } = new();
		public List<ColorInfoModel> AvoidedColors { get; set; } = new();
	}

	public class ColorInfoModel
	{
		public int ColorId { get; set; }
		public string ColorName { get; set; } = string.Empty;
		public string HexCode { get; set; } = string.Empty;
	}

	public class GeneratedImageModel
	{
		public int Id { get; set; }
		public string ImageUrl { get; set; } = string.Empty;
		public string Environment { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; }
	}

	public class GeminiAnalysisRequest
	{
		public List<string> ImageUrls { get; set; } = new();
		public string? HairColor { get; set; }
		public string? EyesColor { get; set; }
		public string? LipsColor { get; set; }
		public string? SkinColor { get; set; }
	}

	public class GeminiColorAnalysisResponse
	{
		public string ColorType { get; set; } = string.Empty;
		public int ColorTypeId { get; set; }
		public List<string> SuggestedColorHexCodes { get; set; } = new();
		public List<string> AvoidedColorHexCodes { get; set; } = new();
	}

	public class ColorMatchResult
	{
		public string HexCode { get; set; } = string.Empty;
		public PerHue.Domain.Entities.Color? MatchedColor { get; set; }
		public bool IsExactMatch { get; set; }
		public double SimilarityScore { get; set; }
	}

	public class VirtualTryOnRequest
	{
		[AllowedImageExtensions(".png", ".jpg", ".jpeg", ".webp")]
		public IFormFile UserImage { get; set; } = null!;
		public List<string> SuggestedColorHexCodes { get; set; } = new();
	}

	public class VirtualTryOnResponse
	{
		public List<GeneratedImage> GeneratedImages { get; set; } = new();
	}

	public class GeneratedImage
	{
		[AllowedImageExtensions(".png", ".jpg", ".jpeg", ".webp")]
		public string ImageUrl { get; set; } = string.Empty;
		public string ColorHex { get; set; } = string.Empty;
		public string Prompt { get; set; } = string.Empty;
	}

	public class AiTestCompleteRequest
	{
		[AllowedImageExtensions(".png", ".jpg", ".jpeg", ".webp")]
		public IFormFile? FaceImages { get; set; }
		public string? HairColor { get; set; }
		public string? EyesColor { get; set; }
		public string? LipsColor { get; set; }
		public string? SkinColor { get; set; }
	}

	public class AiTestCompleteResponse
	{
		public int TestRequestId { get; set; }
		public GeminiColorAnalysisResponse ColorAnalysis { get; set; } = new();
		public List<ColorMatchResult> MatchedSuggestedColors { get; set; } = new();
		public List<ColorMatchResult> MatchedAvoidedColors { get; set; } = new();
		public VirtualTryOnResponse? VirtualTryOnResults { get; set; }
		public string Status { get; set; } = string.Empty;
	}

	public class AiTestResultResponseModel
	{
		public int Id { get; set; }

		public List<AiGeneratedImages> GeneratedImagesList { get; set; } = new List<AiGeneratedImages>();

		public string? Note { get; set; }

		public DateTime? Date { get; set; }
		public int ColorTypeId { get; set; }
		public string ColorTypeName { get; set; } = null!;

		public string SuggestedColor { get; set; } = null!;

		public string AvoidedColor { get; set; } = null!;

		public List<ColorModel> SuggestedColorsBySystem { get; set; } = new List<ColorModel>();

		public CapsulePaletteModel SuggestedCapsulePalleteBySystem { get; set; } = new CapsulePaletteModel();
		//dựa vào SuggestedColor, tìm các capsule palette có colorTypeId trùng kết quả AI trả về có độ tương thích cao
		// ví dụ ColorTypeId có Clean Winter thì trả về danh sách các capsule palette tương ứng của SuggestedColor với color type id = kết quả ai trả về

	}

	public class AiGeneratedImages
	{
		public int AiImageId { get; set; }
		public string AiImageLink { get; set; } = string.Empty;
	}
}
