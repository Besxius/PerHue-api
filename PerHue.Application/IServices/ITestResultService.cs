using PerHue.Application.Basic;
using PerHue.Application.Models;
using PerHue.Domain.Entities;

namespace PerHue.Application.IServices
{
	public interface ITestResultService : IGenericService<TestResultModel>
	{
		Task<TestResultModel> CreateNormalTestCapsulePaletteResult(TestResultModel model);
		Task<TestResultModel> CreateNormalTestSimpleColorResult(TestResultModel model);
		Task<string> GetAiTestUploadImageResult(AiTestUploadImageModel model);
		Task<TestResultModel> GetNormalTestCapsulePaletteResult(CreateNormalTestResultModel model);
		Task<TestResultModel> GetNormalTestSimpleColorResult(CreateNormalTestResultModel model);
	}
}
