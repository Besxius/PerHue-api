using PerHue.Application.Models;
using PerHue.Application.Models.AiTest;
using PerHue.Application.Models.TestRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.IServices
{
	public interface IAIImageAnalysisService
	{
		Task<AiTestResultModel> AnalyzeColorTypeAsync(AiTestModel.GeminiAnalysisRequest request);
		Task<GeminiColorAnalysisResponse> AnalyzeColorTypeAsync2(GeminiAnalysisRequest request);
	}
}
