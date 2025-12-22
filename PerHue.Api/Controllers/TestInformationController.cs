using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
	[Authorize]
	public class TestInformationController : ControllerBase
	{
		private readonly IServicesProvider _services;

		public TestInformationController(IServicesProvider services)
		{
			_services = services;
		}

		[HttpGet("manual-test/my-history")]
		[Authorize(Roles = "User,Admin,Expert")]
		public async Task<ActionResult<IEnumerable<TestResultModel>>> GetMyManualTests()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var results = await _services.TestResultService.GetAllAsyncByUserId(userId);
				if (results == null)
				{
					return NotFound(new { success = false, message = "There are no manual tests yet." });
				}
				return Ok(results);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		[HttpGet("manual-test/{id}")]
		[Authorize(Roles = "User,Admin,Expert")]
		public async Task<ActionResult<TestResultModel>> GetManualTestById(int id)
		{
			try
			{
				var result = await _services.TestResultService.GetByTestResultIdAsync(id);
				if (result == null)
				{
					return NotFound(new { success = false, message = "Manual test not found." });
				}
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		[HttpGet("ai-test/my-history")]
		[Authorize(Roles = "User,Admin")]
		//public async Task<ActionResult<List<AiTestModel.AiTestResponseModel>>> GetMyAiTests()
		public async Task<ActionResult<List<NewTestRequestReponseModel>>> GetMyAiTests()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				//var results = await _services.AiTestService.GetUserAiTestsAsync(userId);
				var results = await _services.AiTestService.GetListTestRequestByTypeAiAsync(userId);
				return Ok(results);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		[HttpGet("ai-test/{id}")]
		[Authorize(Roles = "User,Admin")]
		//public async Task<ActionResult<AiTestModel.AiTestResponseModel?>> GetAiTestResult(int id)
		public async Task<ActionResult<NewTestRequestReponseModel>> GetAiTestResult(int id)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var result = await _services.AiTestService.GetDetailTestRequestByTypeAiAsync(id, userId);

				if (result == null)
				{
					return NotFound(new { success = false, message = "Test not found" });
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		[HttpGet("expert-tests/my-history")]
		[Authorize(Roles = "User,Admin")]
		public async Task<ActionResult<IEnumerable<TestRequestModel>>> GetAllExpertTestRequests()
		{
			try
			{
				// Retrieve User ID from Token Claims
				var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (!int.TryParse(userIdString, out var userId))
				{
					return Unauthorized("Invalid User ID.");
				}

				// Call the new service method filtered by User ID
				var results = await _services.ExpertTestService.GetExpertTestRequestsByUserIdAsync(userId);
				return Ok(results);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("expert-test/{id}")]
		[Authorize(Roles = "User,Admin")]
		public async Task<ActionResult<ExpertTestResultModel>> GetExpertTestResult(int id)
		{
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID format in token.");
			}

			try
			{
				// This now returns the complete object
				var result = await _services.ExpertTestService.GetExpertResponsesForUserAsync(id, userId);
				return Ok(result);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpGet("expert-tests/all")]
		[Authorize(Roles = "User,Admin")]
		public async Task<ActionResult<PaginatedResult<ExpertTestResultModel>>> GetMyExpertTestHistory(
			[FromQuery] int pageIndex = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] DateTime? fromDate = null,
			[FromQuery] DateTime? toDate = null)
		{
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID format in token.");
			}

			try
			{
				// Call the service with pagination parameters
				var results = await _services.ExpertTestService.GetMyCompletedExpertTestsAsync(userId, pageIndex, pageSize, fromDate, toDate);
				return Ok(results);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		
	}
}