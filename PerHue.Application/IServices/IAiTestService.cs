using Microsoft.AspNetCore.Http;
using PerHue.Application.Models;
using PerHue.Application.Models.AiTest;
using PerHue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PerHue.Application.Models.AiTestModel;
using AiTestResponseModel = PerHue.Application.Models.AiTest.AiTestResponseModel;
using GeminiAnalysisRequest = PerHue.Application.Models.AiTest.GeminiAnalysisRequest;

namespace PerHue.Application.IServices
{
	public interface IAiTestService
	{
		Task<PaginatedResultV2<AiTestModel.AiTestResponseModel>> GetAiTestsWithFilterAsync(AiTestSearchModel searchModel);
		Task<bool> MarkTestAsCompletedAsync(int testId);

		Task<AiTestModel.AiTestResponseModel> CreateAiTestRequestAsync(int userId, AiTestModel.CreateAiTestRequestModel model);
		Task<AiTestModel.AiTestResponseModel> ProcessAiTestAsync(int testRequestId);
		Task<AiTestModel.AiTestResponseModel?> GetAiTestResultAsync(int testRequestId, int userId);
		Task<List<AiTestModel.AiTestResponseModel>> GetUserAiTestsAsync(int userId);

		Task<AiTestResultResponseModel> ProcessAiTestAsync2(int userId, AiTestCompleteRequest request);
		Task<GeminiColorAnalysisResponse> AnalyzeColorsOnlyAsync(int testRequestId, GeminiAnalysisRequest request);
		Task<VirtualTryOnResponse> GenerateVirtualTryOnAsync(int testRequestId, VirtualTryOnRequest request);
	}
}
