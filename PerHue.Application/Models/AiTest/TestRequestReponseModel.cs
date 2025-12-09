using PerHue.Application.Models.CapsulePalette;
using PerHue.Application.Models.Color;
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
		public int Id { get; set; }

		public string? HairColor { get; set; }

		public string? EyesColor { get; set; }

		public string? LipsColor { get; set; }

		public string? SkinColor { get; set; }

		public string? Status { get; set; }

		public DateTime? CreatedDate { get; set; }

		public string TypeOfTest { get; set; } = null!;

		public string? Fullname { get; set; }

		public string? ImageUrl { get; set; }

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

		public CapsulePaletteModel SuggestedCapsulePalleteBySystem { get; set; } = new CapsulePaletteModel();
	}
}
