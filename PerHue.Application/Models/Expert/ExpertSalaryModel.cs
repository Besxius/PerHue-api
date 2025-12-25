using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.Expert
{
	public class ExpertSalaryModel
	{
		public int ExpertId { get; set; }
		public decimal TotalSalary { get; set; }
		public int TotalRequests { get; set; }
		public double AverageRating { get; set; }
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }
		public List<ExpertSalaryDetail> Details { get; set; } = new List<ExpertSalaryDetail>();
	}

	public class ExpertSalaryDetail
	{
		public int TestRequestId { get; set; } 
		public DateTime CompletedDate { get; set; }
		public int? Rating { get; set; } //nullable int since user rating is optional
		public decimal Amount { get; set; }
	}
}