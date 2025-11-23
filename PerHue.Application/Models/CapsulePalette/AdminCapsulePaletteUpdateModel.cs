namespace PerHue.Application.Models.CapsulePalette
{
	public class AdminCapsulePaletteUpdateModel
	{
		public int Id { get; set; }
		public int ColorTypeId { get; set; }
		public List<int> ColorIds { get; set; } = new List<int>();
	}
}