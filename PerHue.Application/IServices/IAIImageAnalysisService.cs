using PerHue.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerHue.Application.Models.TestRequest;

namespace PerHue.Application.IServices
{
	public interface IAIImageAnalysisService
	{
		Task<AiTestResultModel> AnalyzeColorTypeAsync(AiTestModel.GeminiAnalysisRequest request);
	}
}
