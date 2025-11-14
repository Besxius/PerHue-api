using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.ManualTest;
using PerHue.Application.Models.TestRequest;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestColorsController : ControllerBase
	{
		private readonly IImageUploadService _imageUploadService;
		private readonly IServicesProvider _servicesProvider;
		public TestColorsController(IServicesProvider servicesProvider, IImageUploadService imageUploadService)
		{
			_servicesProvider = servicesProvider;
			_imageUploadService = imageUploadService;
		}

		[HttpPost]
		[Route("normal-test/simple-color")]
		public async Task<TestResultModel> NormalTestSimpleColor(ManualTestSimpleColorModel model)
		{
			var user = User.Identity;
			var testResult = new CreateManualTestResultModel
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
		public async Task<TestResultModel> NormalTestColorPalette(ManualTestColorPaletteModel model)
		{
			var testResult = new CreateManualTestResultModel
			{
				UserId = int.Parse(User.FindFirst("UserId")!.Value),
				SelectedColors = model.SelectedColors,
				ColorType = model.ColorType
			};
			var result = await _servicesProvider.TestResultService.GetNormalTestCapsulePaletteResult(testResult);
			return result;
		}

		//[HttpPost]
		//[Route("normal-test/capsule-palette/save")]
		//public async Task<TestResultModel> SaveNormalTestColorPalette(TestResultModel model)
		//{
		//	var result = await _servicesProvider.TestResultService.CreateNormalTestCapsulePaletteResult(model);
		//	return result;
		//}

		[HttpPost]
		[Route("ai-test/upload-image")]
		public async Task<string> AiTestUploadImage(AiTestUploadImageModel model)
		{
			var result = await _servicesProvider.TestResultService.GetAiTestUploadImageResult(model);
			return result;
		}

		[HttpPost("expert-test")]
		[Authorize(Roles = "User,Admin")] // Assumes user must be logged in
		public async Task<IActionResult> CreateExpertTestRequest(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("No file uploaded.");
			}

			// Get User ID from token
			var email = User.FindFirst(ClaimTypes.Email)?.Value;
			if (email == null)
			{
				return Unauthorized();
			}
			var user = await _servicesProvider.UserService.GetByEmailAsync(email);
			if (user == null)
			{
				return Unauthorized();
			}

			// 1. Upload image
			var imageUrl = await _imageUploadService.UploadImageAsync(file);

			// 2. Create the expert test request
			var testRequest = await _servicesProvider.TestResultService.CreateExpertTestRequestAsync(user.Id, imageUrl);

			return Ok(testRequest);
		}
	}
}

