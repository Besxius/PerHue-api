namespace PerHue.Application.Models
{
	public class CreateNormalTestResultModel
	{
		public int UserId { get; set; }
		public List<string> SelectedColors { get; set; }
		public int ColorTypeId { get; set; }
		public IEnumerable<CapsulePaletteModel> CapsulePalettes { get; set; } = null!;
	}
}
