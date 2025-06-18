namespace PerHue.Application.Models
{
	public class NormalTestResultModel
	{
		public string SelectedColor { get; set; } = null!;
		public PaginatedResult<CapsulePaletteModel> CapsulePalettes { get; set; } = null!;
	}
}
