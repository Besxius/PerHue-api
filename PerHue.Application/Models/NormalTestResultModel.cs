namespace PerHue.Application.Models
{
	public class NormalTestResultModel
	{
		public ColorTypeModel ColorType { get; set; }
		public IEnumerable<SimpleColorModel> SimpleColors { get; set; } = null!;
		public IEnumerable<CapsulePaletteModel> CapsulePalettes { get; set; } = null!;
	}
}
