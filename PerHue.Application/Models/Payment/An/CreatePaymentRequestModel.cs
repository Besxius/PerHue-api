using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.Payment.An
{
	public class CreatePaymentRequestModel
	{
		public int ServicePackageId { get; set; }
		public string ReturnUrl { get; set; } = null!;
		public string CancelUrl { get; set; } = null!;
	}
}
