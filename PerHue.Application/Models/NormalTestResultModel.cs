namespace PerHue.Application.Models
{
	public class NormalTestResultModel
	{
		public List<string> SelectedColors { get; set; } = null!;
		public int totalResultCount { get; set; } = 0;
		public IEnumerable<CapsulePaletteModel> CapsulePalettes { get; set; } = null!;
	}
}
