using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerHue.Application.Models.TestRequest;

namespace PerHue.Application.Models
{
	public class AiTestModel
	{
		public class CreateAiTestRequestModel
		{
			public string? HairColor { get; set; }
			public string? EyesColor { get; set; }
			public string? LipsColor { get; set; }
			public string? SkinColor { get; set; }
			public List<IFormFile> Images { get; set; } = new();
		}

		public class AiTestResponseModel
		{
			public int TestRequestId { get; set; }
			public string Status { get; set; }
			public DateTime CreatedDate { get; set; }
			public int? UserAccountId { get; set; }
			public string? Fullname { get; set; }
			public string TypeOfTest { get; set; } = null!;
			public AiTestResultModel? Result { get; set; }
		}

		public class GeminiAnalysisRequest
		{
			public List<string> ImageUrls { get; set; } = new();
			public string? HairColor { get; set; }
			public string? EyesColor { get; set; }
			public string? LipsColor { get; set; }
			public string? SkinColor { get; set; }
		}
	}
}
