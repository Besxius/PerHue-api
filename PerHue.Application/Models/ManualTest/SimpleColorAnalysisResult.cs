using PerHue.Application.Models.CapsulePalette;
using System.Collections.Generic;

namespace PerHue.Application.Models.ManualTest
{
	public class SimpleColorAnalysisResult
	{
		public int ColorTypeId { get; set; }
		public string ColorTypeName { get; set; } = string.Empty;
		public List<ColorMatchDetail> MatchedColors { get; set; } = new List<ColorMatchDetail>();
		public List<CapsulePaletteModel> SuggestedCapsulePalettes { get; set; } = new List<CapsulePaletteModel>();
		public List<SeasonColorSuggestion> SeasonColors { get; set; } = new List<SeasonColorSuggestion>();
		public int TotalColorsAnalyzed { get; set; }
		public double AverageRelevanceScore { get; set; }
	}

	public class ColorMatchDetail
	{
		public string InputHexCode { get; set; } = string.Empty;
		public string MatchedHexCode { get; set; } = string.Empty;
		public string ColorName { get; set; } = string.Empty;
		public double RelevanceScore { get; set; }
		public double DeltaE { get; set; }
		public string SeasonCategory { get; set; } = string.Empty;
	}

	public class SeasonColorSuggestion
	{
		public string Season { get; set; } = string.Empty;
		public int ColorCount { get; set; }
		public double ConfidenceScore { get; set; }
		public List<string> RepresentativeColors { get; set; } = new List<string>();
	}
}