using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using PerHue.Application.IServicesProvider;
using System.Security.Claims;
using PerHue.Application.Models;
using PerHue.Application.Models.ExpertTestResult;

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

		/*[HttpGet("expert-test/{testRequestId}")]
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
		}*/
		[HttpPost("expert-test/rate")]
		[Authorize(Roles = "User,Admin")]
		public async Task<IActionResult> RateExpertResponse([FromBody] RateExpertResponseModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID format in token.");
			}

			try
			{
				await _services.ExpertTestService.RateExpertResponseAsync(model, userId);
				return Ok(new { message = "Rating submitted successfully." });
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message }); // e.g., "Already rated"
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpPost("request-review/{testRequestId}")]
		[Authorize(Roles = "User,Admin")]
		public async Task<IActionResult> RequestReview(int testRequestId)
		{
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID format in token.");
			}

			try
			{
				await _services.TestResultService.RequestReviewAsync(testRequestId, userId);
				return Ok(new { message = "Review request sent to a new expert successfully." });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
