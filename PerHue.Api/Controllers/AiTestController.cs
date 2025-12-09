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
		[HttpPost("generate-virtual-tryon")]
		//[Consumes("multipart/form-data")]
		public async Task<IActionResult> GenerateVirtualTryOn([FromForm] VirtualTryOnRequest request)
		{
			try
			{		
				if (!request.SuggestedColorHexCodes.Any())
				{
					return BadRequest(new { message = "At least one color hex code is required" });
				}

				var result = await _aiTestService.GenerateVirtualTryOnAsync(request);

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
