using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.PaymentLog
{
	public class CreatePaymentLogModel
	{
		public int PaymentId { get; set; }

		public string? OldStatus { get; set; }

		public string? NewStatus { get; set; }

		public string Mesage { get; set; } = null!;

		public DateTime CreatedAt { get; set; }

		public string Metadata { get; set; }
	}
}
