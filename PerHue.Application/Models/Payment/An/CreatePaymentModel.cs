using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.Payment.An
{
	public class CreatePaymentModel
	{
		public int UserId { get; set; }
		public int ServicePackageId { get; set; }
		public int Amount { get; set; }
		public string Description { get; set; } = null!;
		public string OrderCode { get; set; } = null!;
	}
}
