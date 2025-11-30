using PerHue.Application.Models.CapsulePalette;
using PerHue.Application.Models.Color;
using PerHue.Application.Models.ColorType;
using PerHue.Application.Models.User;
using PerHue.Domain.Entities;
using System.Runtime.CompilerServices;

namespace PerHue.Application.Models.ManualTest
{
	public class TestResultModel
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public string? Picture { get; set; } = null!;

		public string? Type { get; set; }

		public int ColorTypeId { get; set; }

		public DateTime CreatedDate { get; set; }

		public string ChosenColor { get; set; } = null!;

		public string SuggestedColor { get; set; } = null!;

		public virtual ColorTypeModel ColorType { get; set; } = null!;

		public virtual UserModel User { get; set; } = null!;

		public virtual ICollection<CapsulePaletteModel> CapsulePalettes { get; set; } = new List<CapsulePaletteModel>();

		public virtual ICollection<ColorModel> Colors { get; set; } = new List<ColorModel>();
	}
}
