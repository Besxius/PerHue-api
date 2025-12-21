using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.ExpertTestResult
{
	public class ReviewTestRequestModel
	{
		public int ExpertTestRequestId { get; set; } // The ID of the task assigned to the expert
		public TestRequestModel TestRequest { get; set; }
		public IEnumerable<TestResponseModel> PreviousResponses { get; set; }
		public bool CanEdit { get; set; }
	}
}
