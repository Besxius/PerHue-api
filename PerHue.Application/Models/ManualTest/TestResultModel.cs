using PerHue.Application.Models.CapsulePalette;
using PerHue.Application.Models.Color;
using PerHue.Application.Models.ColorType;
using PerHue.Application.Models.User;
using PerHue.Domain.Entities;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace PerHue.Application.Models.ManualTest
{
	public class TestResultModel
	{
		public int Id { get; set; } //

		[JsonIgnore]
		public int UserId { get; set; }

		public string? Picture { get; set; } //

		public int ColorTypeId { get; set; } //

		public string ColorTypeName { get; set; } //

		public DateTime CreatedDate { get; set; } //

		[JsonIgnore]
		public string ChosenColor { get; set; }

		[JsonIgnore]
		public string SuggestedColor { get; set; }

		[JsonIgnore]
		public virtual ColorTypeModel ColorType { get; set; } = null!;

		[JsonIgnore]
		public virtual UserModel User { get; set; } = null!;

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public virtual ICollection<CapsulePaletteModel?> CapsulePalettes { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public virtual ICollection<ColorModel?> Colors { get; set; }
	}
}
