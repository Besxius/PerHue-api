using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.Payment.An
{
	public class PaymentModel
	{
		public int Id { get; set; }
		public int Amount { get; set; }
		public string Description { get; set; } = null!;
		public string Status { get; set; } = null!;
		public string TransactionId { get; set; } = null!;
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}
