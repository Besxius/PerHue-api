using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using System.Security.Claims;
using static PerHue.Application.Models.AiTestModel;
using PerHue.Application.Models.AiTest;


namespace PerHue.Api.Controllers
{
	[ApiController]
	[Route("api/ai-test")]
	//[Authorize(Roles = "User")]
	public class AiTestController : ControllerBase
	{
		private readonly IAiTestService _aiTestService;
		private readonly ITestRequestRepository _testRequestRepository;

		public AiTestController(
			IAiTestService aiTestService,
			ITestRequestRepository testRequestRepository)
		{
			_aiTestService = aiTestService;
			_testRequestRepository = testRequestRepository;
		}

		/// <summary>
		/// Tạo AI test request và upload ảnh
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> CreateAiTest([FromForm] AiTestModel.CreateAiTestRequestModel model)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var result = await _aiTestService.CreateAiTestRequestAsync(userId, model);
				return Ok(new { success = true, data = result });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = "An error occurred", details = ex.Message });
			}
		}

		/// <summary>
		/// Lấy kết quả AI test theo ID testRequest
		/// </summary>
		[HttpGet("{testRequestId}")]
		public async Task<IActionResult> GetAiTestResult(int testRequestId)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var result = await _aiTestService.GetAiTestResultAsync(testRequestId, userId);

				if (result == null)
				{
					return NotFound(new { success = false, message = "Test not found" });
				}

				return Ok(new { success = true, data = result });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy tất cả AI tests của user
		/// </summary>
		[HttpGet("my-tests")]
		public async Task<IActionResult> GetMyAiTests()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var results = await _aiTestService.GetUserAiTestsAsync(userId);
				return Ok(new { success = true, data = results });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Xử lý lại một AI test (nếu failed)
		/// </summary>
		[HttpPost("{testRequestId}/reprocess")]
		public async Task<IActionResult> ReprocessAiTest(int testRequestId)
		{
			try
			{
				var result = await _aiTestService.ProcessAiTestAsync(testRequestId);
				return Ok(new { success = true, data = result });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Tạo và xử lý toàn bộ luồng AI Test (phân tích màu + matching + virtual try-on)
		/// </summary>
		[HttpPost("create-and-process")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> CreateAndProcessAiTest([FromForm] AiTestCompleteRequest requestDto)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
		
				if (userId == 0)
				{
					return Unauthorized(new { message = "User not authenticated" });
				}

				// Validate images
				if (requestDto.FaceImages == null || requestDto.FaceImages.Count == 0)
				{
					return BadRequest(new { message = "At least one face image is required" });
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

				return Ok(new
				{
					success = true,
					data = result,
					message = "AI Test created and processed successfully"
				});
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

		/// <summary>
		/// Chỉ phân tích màu từ ảnh (không generate virtual try-on)
		/// </summary>
		[HttpPost("analyze-colors/{testRequestId}")]
		public async Task<IActionResult> AnalyzeColors(int testRequestId, [FromBody] Application.Models.AiTest.GeminiAnalysisRequest request)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

				var testRequest = await _testRequestRepository.GetByIdAsync(testRequestId);
				if (testRequest == null)
				{
					return NotFound(new { message = "Test request not found" });
				}

				if (testRequest.UserAccountId != userId)
				{
					return Forbid();
				}

				if (!request.ImageUrls.Any())
				{
					return BadRequest(new { message = "At least one image is required" });
				}

				var result = await _aiTestService.AnalyzeColorsOnlyAsync(testRequestId, request);

				return Ok(new
				{
					success = true,
					data = result,
					message = "Color analysis completed successfully"
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error analyzing colors", error = ex.Message });
			}
		}

		/// <summary>
		/// Generate virtual try-on images
		/// </summary>
		[HttpPost("generate-virtual-tryon/{testRequestId}")]
		public async Task<IActionResult> GenerateVirtualTryOn(int testRequestId, [FromBody] VirtualTryOnRequest request)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

				var testRequest = await _testRequestRepository.GetByIdAsync(testRequestId);
				if (testRequest == null)
				{
					return NotFound(new { message = "Test request not found" });
				}

				if (testRequest.UserAccountId != userId)
				{
					return Forbid();
				}

				if (!request.SuggestedColorHexCodes.Any())
				{
					return BadRequest(new { message = "At least one color hex code is required" });
				}

				var result = await _aiTestService.GenerateVirtualTryOnAsync(testRequestId, request);

				return Ok(new
				{
					success = true,
					data = result,
					message = "Virtual try-on images generated successfully"
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error generating virtual try-on images", error = ex.Message });
			}
		}

		//TEST REQUEST

	}
}
