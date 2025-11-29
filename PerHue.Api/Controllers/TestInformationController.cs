using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.ExpertTestResult;
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

		[HttpGet("expert-test/{testRequestId}")]
		[Authorize(Roles = "User,Admin")]
		public async Task<ActionResult<ExpertTestResultModel>> GetExpertTestResult(int testRequestId) // <-- Changed return type
		{
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID format in token.");
			}

			try
			{
				// This now returns the complete object
				var result = await _services.ExpertTestService.GetExpertResponsesForUserAsync(testRequestId, userId);
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
		public async Task<IActionResult> GetAllExpertTestRequests()
		{
			try
			{
				var results = await _services.ExpertTestService.GetAllExpertTestRequestsAsync();
				return Ok(results);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
		[HttpGet("expert-tests/my-history")]
		[Authorize(Roles = "User,Admin")]
		public async Task<IActionResult> GetMyExpertTestHistory(
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