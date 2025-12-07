using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.Payment.An
{
	public class CreatePaymentResult
	{
		public int PaymentId { get; set; }
		public string PaymentUrl { get; set; } = null!;
		public string TransactionId { get; set; } = null!;
	}
}
