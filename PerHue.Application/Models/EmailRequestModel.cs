using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models
{
	public class EmailServiceRequestModel
	{
		public string ToEmail { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
	}
}
