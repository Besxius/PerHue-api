namespace PerHue.Application.Models.CapsulePalette
{
	public class AdminCapsulePaletteCreateModel
	{
		public int ColorTypeId { get; set; }
		public List<int> ColorIds { get; set; } = new List<int>();
	}
}