using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerHue.Application.Models.TestRequest;

namespace PerHue.Application.Models.ExpertTestResult
{
	/// <summary>
	/// A DTO to group a completed expert test request with its individual expert responses.
	/// </summary>
	public class ExpertTestResultModel
	{
		public TestRequestModel TestRequest { get; set; }
		public IEnumerable<TestResponseModel> Responses { get; set; }
	}
}
