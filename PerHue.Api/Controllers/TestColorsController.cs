using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.AiTest;
using PerHue.Application.Models.ExpertTestResult;
using PerHue.Application.Models.ManualTest;
using System.Security.Claims;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestColorsController : ControllerBase
	{
		private readonly IImageUploadService _imageUploadService;
		private readonly IServicesProvider _servicesProvider;
		private readonly IAiTestService _aiTestService;
		public TestColorsController(IServicesProvider servicesProvider, IImageUploadService imageUploadService, IAiTestService aiTestService)
		{
			_servicesProvider = servicesProvider;
			_imageUploadService = imageUploadService;
			_aiTestService = aiTestService;
		}

		[HttpPost("manual-test")]
		[Authorize(Roles = "User,Admin")]
		public async Task<ActionResult<TestResultModel>> NormalTestSimpleColor(ManualTestSimpleColorModel model)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var testResult = new CreateManualTestResultModel
				{
					UserId = userId,
					SelectedColors = model.SelectedColors,
				};
				var result = await _servicesProvider.TestResultService.GetNormalTestSimpleColorResult(testResult);
				return Ok(result);
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

		/// <summary>
		/// Tạo và xử lý toàn bộ luồng AI Test (phân tích màu + matching + virtual try-on)
		/// </summary>
		[HttpPost("ai-test")]
		[Consumes("multipart/form-data")]
		[Authorize(Roles = "User,Admin")]
		public async Task<ActionResult<AiTestResultResponseModel>> CreateAndProcessAiTest([FromForm] AiTestCompleteRequest requestDto)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

				if (userId == 0)
				{
					return Unauthorized(new { message = "User not authenticated" });
				}

				
				// Validate image files
				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
				var maxFileSize = 10 * 1024 * 1024; // 10MB

				foreach (var image in requestDto.FaceImages)
				{
					var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
					if (!allowedExtensions.Contains(extension))
					{
						return BadRequest(new { message = $"Invalid file type: {image.FileName}. Only JPG, JPEG, and PNG are allowed." });
					}

					if (image.Length > maxFileSize)
					{
						return BadRequest(new { message = $"File too large: {image.FileName}. Maximum size is 10MB." });
					}

					if (image.Length == 0)
					{
						return BadRequest(new { message = $"Empty file: {image.FileName}" });
					}
				}

				// Gọi service với userId
				var result = await _aiTestService.ProcessAiTestAsync2(userId, requestDto);

				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error processing AI Test", error = ex.Message });
			}
		}

		[HttpPost("expert-test")]
		[Authorize(Roles = "User,Admin")]
		public async Task<ActionResult<TestRequestModel>> CreateExpertTestRequest([FromForm] CreateExpertTestRequestModel model)
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

