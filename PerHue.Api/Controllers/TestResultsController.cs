using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using PerHue.Application.IServicesProvider;
using System.Security.Claims;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class TestResultsController : ControllerBase
	{
		private readonly IServicesProvider _services;

		public TestResultsController(IServicesProvider services)
		{
			_services = services;
		}

		[HttpGet("expert-test/{testRequestId}")]
		[Authorize(Roles = "User,Admin")]
		public async Task<IActionResult> GetExpertTestResult(int testRequestId)
		{
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID format in token.");
			}

			try
			{
				var responses = await _services.ExpertTestService.GetExpertResponsesForUserAsync(testRequestId, userId);
				return Ok(responses);
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
		[Authorize(Roles = "Admin")] // Secured for Admin only
		public async Task<IActionResult> GetAllExpertTestResults()
		{
			try
			{
				var results = await _services.ExpertTestService.GetAllCompletedExpertTestsAsync();
				return Ok(results);
			}
			catch (Exception ex)
			{
				// This will be caught by your GlobalExceptionHandler
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// --- ADD THIS NEW ENDPOINT ---
		[HttpGet("expert-tests/my-history")]
		[Authorize(Roles = "User,Admin")] // Secured for the logged-in user
		public async Task<IActionResult> GetMyExpertTestHistory()
		{
			// Get User ID from the token, just like in [HttpGet("information")]
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID format in token.");
			}

			try
			{
				var results = await _services.ExpertTestService.GetMyCompletedExpertTestsAsync(userId);
				return Ok(results);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}
