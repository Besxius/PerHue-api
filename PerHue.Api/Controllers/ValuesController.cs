using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.ExpertTestResult;
using System.Security.Claims;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Expert")] // Secure this controller for Experts only
	public class ExpertTestController : ControllerBase
	{
		private readonly IServicesProvider _services;

		public ExpertTestController(IServicesProvider services)
		{
			_services = services;
		}

		private async Task<int> GetCurrentExpertId()
		{
			var email = User.FindFirst(ClaimTypes.Email)?.Value;
			var user = await _services.UserService.GetByEmailAsync(email);
			return user.Id; // In this system, ExpertId is the same as UserAccount.Id
		}

		[HttpGet("requests")]
		public async Task<IActionResult> GetPendingRequests()
		{
			var expertId = await GetCurrentExpertId();
			var requests = await _services.ExpertTestService.GetPendingRequestsAsync(expertId);
			return Ok(requests);
		}

		[HttpPost("respond")]
		public async Task<IActionResult> SubmitResponse([FromBody] CreateTestResponseModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var expertId = await GetCurrentExpertId();
			var response = await _services.ExpertTestService.SubmitResponseAsync(model, expertId);
			return Ok(response);
		}
		[HttpGet("review-requests")]
		public async Task<IActionResult> GetPendingReviewRequests()
		{
			var expertId = await GetCurrentExpertId();
			var requests = await _services.ExpertTestService.GetPendingReviewRequestsAsync(expertId);
			return Ok(requests);
		}
		[HttpPost("vote")]
		public async Task<IActionResult> VoteForResponse([FromBody] VoteResponseModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var expertId = await GetCurrentExpertId();
				var response = await _services.ExpertTestService.VoteForResponseAsync(model, expertId);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}