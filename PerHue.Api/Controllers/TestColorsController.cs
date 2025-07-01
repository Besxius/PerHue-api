using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestColorsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		public TestColorsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		[HttpPost]
		[Route("normal-test/simple-color")]
		public async Task<TestResultModel> NormalTestSimpleColor(NormalTestSimpleColorModel model)
		{
			var testResult = new CreateNormalTestResultModel
			{
				UserId = int.Parse(User.FindFirst("UserId")!.Value),
				SelectedColors = model.SelectedColors,
			};
			var result = await _servicesProvider.TestResultService.GetNormalTestSimpleColorResult(testResult);

			return result;
		}

		//[HttpPost]
		//[Route("normal-test/simple-color/save")]
		//public async Task<TestResultModel> SaveNormalTestSimpleColor(TestResultModel model)
		//{
		//	var result = await _servicesProvider.TestResultService.CreateNormalTestSimpleColorResult(model);
		//	return result;
		//}

		[HttpPost] 
		[Route("normal-test/capsule-palette")]
		public async Task<TestResultModel> NormalTestColorPalette(NormalTestColorPaletteModel model)
		{
			var testResult = new CreateNormalTestResultModel
			{
				UserId = int.Parse(User.FindFirst("UserId")!.Value),
				SelectedColors = model.SelectedColors,
				ColorType = model.ColorType
			};
			var result = await _servicesProvider.TestResultService.GetNormalTestCapsulePaletteResult(testResult);
			return result;
		}

		[HttpPost]
		[Route("normal-test/capsule-palette/save")]
		public async Task<TestResultModel> SaveNormalTestColorPalette(TestResultModel model)
		{
			var result = await _servicesProvider.TestResultService.CreateNormalTestCapsulePaletteResult(model);
			return result;
		}

		[HttpPost]
		[Route("ai-test/upload-image")]
		public async Task<NormalTestResultModel> AiTestKUploadImage(string colorsListJson)
		{
			throw new NotSupportedException();
		}
	}
}
