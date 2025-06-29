using PerHue.Application.Basic;
using PerHue.Application.Models;
using PerHue.Domain.Entities;

namespace PerHue.Application.IServices
{
	public interface ITestResultService : IGenericService<TestResultModel>
	{
		Task<TestResultModel> CreateNormalTestCapsulePaletteResult(CreateNormalTestResultModel model);
		Task<TestResultModel> CreateNormalTestSimpleColorResult(CreateNormalTestResultModel model);
	}
}
