namespace PerHue.Application.Models
{
	public class CreateNormalTestResultModel
	{
		public int UserId { get; set; }
		public List<string> SelectedColors { get; set; }
		public string? ColorType { get; set; } = string.Empty;
	}
}
