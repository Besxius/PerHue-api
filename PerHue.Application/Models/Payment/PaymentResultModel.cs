namespace PerHue.Application.Models.Payment
{
	public class PaymentResultModel
	{
		public int PaymentId { get; set; }
		public int PaymentLogId { get; set; }
		public int UserSubscriptionId { get; set; }
		public bool Success { get; set; }
		public string Message { get; set; }
		public string? PaymentUrl { get; set; }
		public int? PreviousSubscriptionId { get; set; }
		public bool WasUpgraded { get; set; }
	}
}