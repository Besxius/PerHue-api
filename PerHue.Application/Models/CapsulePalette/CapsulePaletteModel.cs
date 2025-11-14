using PerHue.Application.Models.Color;
using PerHue.Application.Models.ColorType;

namespace PerHue.Application.Models.CapsulePalette
{
	public class CapsulePaletteModel
	{
		public int Id { get; set; }

		public int ColorTypeId { get; set; }

		public virtual ColorTypeModel ColorType { get; set; } = null!;

		public virtual ICollection<ColorModel> Colors { get; set; } = new List<ColorModel>();
	}
}
