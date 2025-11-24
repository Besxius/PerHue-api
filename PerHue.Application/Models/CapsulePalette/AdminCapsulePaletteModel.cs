using PerHue.Application.Models.Color;
using PerHue.Application.Models.ColorType;

namespace PerHue.Application.Models.CapsulePalette
{
	public class AdminCapsulePaletteModel
	{
		public int Id { get; set; }
		public int ColorTypeId { get; set; }
		public string? ColorTypeName { get; set; }

		public virtual ColorTypeModel? ColorType { get; set; }
		public virtual ICollection<AdminColorModel> Colors { get; set; } = new List<AdminColorModel>();
	}
}