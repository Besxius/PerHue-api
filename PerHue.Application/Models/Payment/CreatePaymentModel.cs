namespace PerHue.Application.Models.Payment
{
	public class CreatePaymentModel
	{
		public int UserId { get; set; }
		public int ServicePackageId { get; set; }
		public decimal Amount { get; set; }
		public string PaymentMethod { get; set; } // VNPay, Momo, etc.
		public string? Description { get; set; }
	}
}