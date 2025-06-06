using PerHue.Domain.Entities;

namespace PerHue.Application.Models
{
	public class PaymentLogModel
	{
		public int Id { get; set; }

		public int PaymentId { get; set; }

		public string EventType { get; set; } = null!;

		public string? OldStatus { get; set; }

		public string? NewStatus { get; set; }

		public string Mesage { get; set; } = null!;

		public DateTime CreatedAt { get; set; }

		public string? Metadata { get; set; }

		public virtual PaymentModel Payment { get; set; } = null!;
	}
}
