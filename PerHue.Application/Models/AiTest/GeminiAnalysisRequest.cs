using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerHue.Domain.Entities;

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
		public IFormFile UserImage { get; set; } = null!;
		public List<string> SuggestedColorHexCodes { get; set; } = new();
		public List<string> Environments { get; set; } = new() { "indoor", "outdoor_sunny", "outdoor_cloudy", "evening" };
		public List<string> ClothingTypes { get; set; } = new() { "shirt", "dress", "sweater" };
	}

	public class VirtualTryOnResponse
	{
		public List<GeneratedImage> GeneratedImages { get; set; } = new();
	}

	public class GeneratedImage
	{
		public string ImageUrl { get; set; } = string.Empty;
		public string Environment { get; set; } = string.Empty;
		public string ClothingType { get; set; } = string.Empty;
		public string ColorHex { get; set; } = string.Empty;
		public string Prompt { get; set; } = string.Empty;
	}

	public class AiTestCompleteRequest
	{
		public List<IFormFile> FaceImages { get; set; } = new();
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

		public string? Note { get; set; }

		public DateTime? Date { get; set; }

		public string SuggestedColor { get; set; } = null!;

		public string AvoidedColor { get; set; } = null!;

		public int ColorTypeId { get; set; }
	}
}
