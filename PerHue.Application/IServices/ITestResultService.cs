using PerHue.Application.Basic;
using PerHue.Application.Models.ManualTest;
using PerHue.Application.Models.TestRequest;
using PerHue.Domain.Entities;

namespace PerHue.Application.IServices
{
	public interface ITestResultService : IGenericService<TestResultModel>
	{
		Task<TestResultModel> CreateNormalTestCapsulePaletteResult(TestResultModel model);
		Task<TestResultModel> CreateNormalTestSimpleColorResult(TestResultModel model);
		Task<string> GetAiTestUploadImageResult(AiTestUploadImageModel model);
		Task<TestResultModel> GetNormalTestCapsulePaletteResult(CreateManualTestResultModel model);
		Task<TestResultModel> GetNormalTestSimpleColorResult(CreateManualTestResultModel model);
	}
}
