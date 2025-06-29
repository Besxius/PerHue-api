using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using System.Security.Claims;

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
		public async Task<TestResultModel> NormalTestSimpleColor(string colorsListJson)
		{
			var selectedColor = System.Text.Json.JsonSerializer.Deserialize<List<string>>(colorsListJson);

			var testResult = new CreateNormalTestResultModel
			{
				UserId = int.Parse(User.FindFirst("UserId")!.Value),
				SelectedColors = selectedColor,
			};
			var result = await _servicesProvider.TestResultService.CreateNormalTestSimpleColorResult(testResult);

			return result;
		}

		[HttpPost] 
		[Route("normal-test/capsule-palette")]
		public async Task<TestResultModel> NormalTestColorPalette(string colorsListJson)
		{
			var selectedColor = System.Text.Json.JsonSerializer.Deserialize<List<string>>(colorsListJson);

			var testResult = new CreateNormalTestResultModel
			{
				UserId = int.Parse(User.FindFirst("UserId")!.Value),
				SelectedColors = selectedColor,
			};
			var result = await _servicesProvider.TestResultService.CreateNormalTestCapsulePaletteResult(testResult);

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
