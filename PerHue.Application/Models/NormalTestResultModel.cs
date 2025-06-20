namespace PerHue.Application.Models
{
	public class NormalTestResultModel
	{
		public List<string> SelectedColors { get; set; } = null!;
		public IEnumerable<CapsulePaletteModel> CapsulePalettes { get; set; } = null!;
	}
}
