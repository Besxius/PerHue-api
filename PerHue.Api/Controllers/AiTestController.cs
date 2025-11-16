using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using System.Security.Claims;
using static PerHue.Application.Models.AiTestModel;

namespace PerHue.Api.Controllers
{
	[ApiController]
	[Route("api/AiTest")]
	//[Authorize(Roles = "User")]
	public class AiTestController : ControllerBase
	{
		private readonly IAiTestService _aiTestService;

		public AiTestController(IAiTestService aiTestService)
		{
			_aiTestService = aiTestService;
		}

		/// <summary>
		/// Tạo AI test request và upload ảnh
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> CreateAiTest([FromForm] CreateAiTestRequestModel model)
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
	}
}
