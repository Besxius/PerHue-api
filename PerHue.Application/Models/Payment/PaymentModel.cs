using PerHue.Application.Models.User;
using PerHue.Domain.Entities;

namespace PerHue.Application.Models.Payment
{
	public class PaymentModel
	{
		public int Amount { get; set; }

		public string? Type { get; set; }

		public string? Message { get; set; }

		public DateTime Time { get; set; }

		public string Status { get; set; } = null!;

		public int UserId { get; set; }

		public virtual UserModel User { get; set; } = null!;
	}
}
