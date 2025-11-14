using PerHue.Application.Models.CapsulePalette;
using PerHue.Application.Models.ColorType;

namespace PerHue.Application.Models.ManualTest
{
	public class ManualTestResultModel
	{
		public ColorTypeModel ColorType { get; set; }
		public IEnumerable<CapsulePaletteModel> CapsulePalettes { get; set; } = null!;
	}
}
