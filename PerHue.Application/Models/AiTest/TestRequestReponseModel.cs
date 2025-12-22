using PerHue.Application.Models.CapsulePalette;
using PerHue.Application.Models.Color;
using PerHue.Application.Models.ColorType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PerHue.Application.Models.AiTest
{
	public class NewTestRequestReponseModel
	{
		public int Id { get; set; } //

		[JsonIgnore]
		public string? HairColor { get; set; }

		[JsonIgnore]
		public string? EyesColor { get; set; }

		[JsonIgnore]
		public string? LipsColor { get; set; }

		[JsonIgnore]
		public string? SkinColor { get; set; }

		[JsonIgnore]
		public string? Status { get; set; }

		public DateTime? CreatedDate { get; set; } //

		[JsonIgnore]
		public string TypeOfTest { get; set; } = null!;

		[JsonIgnore]
		public string? Fullname { get; set; }

		public string? ImageUrl { get; set; } //

		public int ColorTypeId { get; set; } //

		public string ColorTypeName { get; set; } //

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public NewAiTestResultResponseModel? newAiTestResultResponseModel { get; set; }
	}

	public class NewAiTestResultResponseModel
	{
		public int Id { get; set; }

		public string? Note { get; set; }

		public string SuggestedColor { get; set; } = null!;

		public string AvoidedColor { get; set; } = null!;

		public int ColorTypeId { get; set; }

		public List<ColorModel> SuggestedColorsBySystem { get; set; } = new List<ColorModel>();

		public List<CapsulePaletteModel> SuggestedCapsulePalletesBySystem { get; set; } = new List<CapsulePaletteModel>();
	}
}
