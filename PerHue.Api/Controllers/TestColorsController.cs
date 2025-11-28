using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.ExpertTestResult;
using PerHue.Application.Models.ManualTest;
using PerHue.Application.Models.TestRequest;
using System.Security.Claims;

namespace PerHue.Api.Controllers
{
	[Route("api/normal-test")]
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
		[Route("/simple-color")]
		public async Task<IActionResult> NormalTestSimpleColor(ManualTestSimpleColorModel model)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var testResult = new CreateManualTestResultModel
				{
					//UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value),
					UserId = userId,
					SelectedColors = model.SelectedColors,
				};
				var result = await _servicesProvider.TestResultService.GetNormalTestSimpleColorResult(testResult);

				return Ok(new { success = true, data = result });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error while processing Manual Test", error = ex.Message });
			}
		}

		[HttpPost("expert-test")]
		[Authorize(Roles = "User,Admin")]
		// Use the new DTO from the correct namespace
		public async Task<IActionResult> CreateExpertTestRequest([FromForm] CreateExpertTestRequestModel model)
		{
			if (model.File == null || model.File.Length == 0)
			{
				return BadRequest("No file uploaded.");
			}

			// Get User ID from token
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userIdString))
			{
				return Unauthorized("User ID not found in token.");
			}

			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID format in token.");
			}

			// 1. Upload image
			var imageUrl = await _imageUploadService.UploadImageAsync(model.File);

			// 2. Create the new service parameter DTO
			var parameters = new ExpertTestCreationParameters
			{
				UserId = userId,
				ImageUrl = imageUrl,
				HairColor = model.HairColor,
				EyesColor = model.EyesColor,
				LipsColor = model.LipsColor,
				SkinColor = model.SkinColor
			};

			// 3. Create the expert test request
			var testRequest = await _servicesProvider.TestResultService.CreateExpertTestRequestAsync(parameters);

			return Ok(testRequest);
		}
	}
}

