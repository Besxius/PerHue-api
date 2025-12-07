using PerHue.Application.Basic;
using PerHue.Application.Models;
using PerHue.Application.Models.ExpertTestResult;
using PerHue.Application.Models.ManualTest;
using PerHue.Application.Models.TestRequest;
using PerHue.Domain.Entities;

namespace PerHue.Application.IServices
{
	public interface ITestResultService : IGenericService<TestResultModel>
	{
		Task<TestRequestModel> CreateExpertTestRequestAsync(ExpertTestCreationParameters parameters);
		Task RequestReviewAsync(int testRequestId, int userId);

		Task<TestResultModel> CreateNormalTestCapsulePaletteResult(TestResultModel model);
		Task<TestResultModel> CreateNormalTestSimpleColorResult(TestResultModel model);
		Task<string> GetAiTestUploadImageResult(AiTestUploadImageModel model);
		Task<TestResultModel> GetNormalTestCapsulePaletteResult(CreateManualTestResultModel model);
		Task<TestResultModel> GetNormalTestSimpleColorResult(CreateManualTestResultModel model);
		Task<IEnumerable<TestResultModel>> GetAllAsyncByUserId(int userId);
		Task<TestResultModel> GetByTestResultIdAsync(int id);
	}
}
