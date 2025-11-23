namespace PerHue.Application.Models.Color
{
	public class AdminColorUpdateModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public string HexCode { get; set; } = null!;
	}
}