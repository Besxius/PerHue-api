namespace PerHue.Application.Models.ManualTest
{
	public class CreateManualTestResultModel
	{
		public int UserId { get; set; }
		public List<string> SelectedColors { get; set; }
		public string? ColorType { get; set; } = string.Empty;
	}
}
