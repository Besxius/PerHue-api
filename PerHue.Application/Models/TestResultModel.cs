using PerHue.Domain.Entities;
using System.Runtime.CompilerServices;

namespace PerHue.Application.Models
{
	public class TestResultModel
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public string Picture { get; set; } = null!;

		public string? Type { get; set; }

		public int ColorTypeId { get; set; }

		public virtual ColorTypeModel ColorType { get; set; } = null!;

		public virtual UserModel User { get; set; } = null!;

		public virtual ICollection<CapsulePaletteModel> CapsulePalettes { get; set; } = new List<CapsulePaletteModel>();

		public virtual ICollection<ColorModel> Colors { get; set; } = new List<ColorModel>();

		public virtual ICollection<SimpleColorModel> SimpleColors { get; set; } = new List<SimpleColorModel>();

	}
}
