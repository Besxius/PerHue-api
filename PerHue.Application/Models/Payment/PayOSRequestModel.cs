namespace PerHue.Application.Models.Payment
{
	public class PayOSRequestModel
	{
		public int Amount { get; set; }
		public string Description { get; set; } = string.Empty;
		public string ReturnUrl { get; set; } = string.Empty;
		public string CancelUrl { get; set; } = string.Empty;

	}
}
