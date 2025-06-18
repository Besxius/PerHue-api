namespace PerHue.Application.Models
{
	public class CapsulePaletteModel
	{
		public int Id { get; set; }

		public int ColorTypeId { get; set; }

		public virtual ColorTypeModel ColorType { get; set; } = null!;

		public virtual ICollection<ColorModel> Colors { get; set; } = new List<ColorModel>();
	}
}
