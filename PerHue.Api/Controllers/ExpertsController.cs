using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Expert;
using PerHue.Application.Models.ExpertTestResult;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExpertsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public ExpertsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		[HttpGet("ranking")]
		public async Task<ActionResult<IEnumerable<ExpertModel>>> GetExpertsByRating()
		{
			var experts = await _servicesProvider.ExpertService.GetAllByRatingDescendingAsync();
			return Ok(experts);
		}

		// You can also add the GetAll endpoint here if needed for clearer API structure
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ExpertModel>>> GetAllExperts()
		{
			var experts = await _servicesProvider.ExpertService.GetAllAsync();
			return Ok(experts);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ExpertModel>> GetExpertById(int id)
		{
			var expert = await _servicesProvider.ExpertService.GetByIdAsync(id);
			if (expert == null)
			{
				return NotFound();
			}
			return Ok(expert);
		}
		[HttpGet("my-salary")]
		[Authorize(Roles = "Expert")]
		public async Task<ActionResult<ExpertSalaryModel>> GetMySalary(
			[FromQuery] DateTime? startDate,
			[FromQuery] DateTime? endDate)
		{
			// Get User ID from token
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var expertId))
			{
				return Unauthorized("Invalid User ID in token.");
			}
			try
			{
				var salaryReport = await _servicesProvider.ExpertService.CalculateSalaryAsync(expertId, startDate, endDate);
				return Ok(salaryReport);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("information")]
		public async Task<ActionResult<ExpertModel>> GetExpertInformation()
		{
			var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var expert = await _servicesProvider.ExpertService.GetByIdAsync(int.Parse(id));
			if (expert == null)
			{
				return NotFound();
			}
			return Ok(expert);
		}

		private async Task<int> GetCurrentExpertId()
		{
			var email = User.FindFirst(ClaimTypes.Email)?.Value;
			var user = await _servicesProvider.UserService.GetByEmailAsync(email);
			return user.Id; // In this system, ExpertId is the same as UserAccount.Id
		}

		[HttpGet("requests")]
		[Authorize(Roles = "Expert")]
		public async Task<ActionResult<IEnumerable<TestRequestModel>>> GetPendingRequests()
		{
			var expertId = await GetCurrentExpertId();
			var requests = await _servicesProvider.ExpertTestService.GetPendingRequestsAsync(expertId);
			return Ok(requests);
		}
		[HttpGet("requests/{id}")]
		[Authorize(Roles = "Expert")]
		public async Task<ActionResult<ExpertTestResultModel>> GetExpertTestResult(int id)
		{
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var userId))
			{
				return Unauthorized("Invalid User ID format in token.");
			}

			try
			{
				// Now returns the filtered ExpertTestResultModel
				var result = await _servicesProvider.ExpertTestService.GetExpertResponsesForExpertAsync(id, userId);
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
		[HttpPut("response/{id}")]
		[Authorize(Roles = "Expert")]
		public async Task<ActionResult<TestResponseModel>> UpdateResponse(int id, [FromBody] UpdateTestResponseModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!int.TryParse(userIdString, out var expertId))
			{
				return Unauthorized("Invalid User ID format in token.");
			}

			try
			{
				var updatedResponse = await _servicesProvider.ExpertTestService.UpdateResponseAsync(id, model, expertId);
				return Ok(updatedResponse);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				// This catches the "test request is already completed" error
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
		[HttpGet("all-requests")]
		public async Task<ActionResult<IEnumerable<ExpertAssignmentModel>>> GetAllRequests()
		{
			var expertId = await GetCurrentExpertId();
			var requests = await _servicesProvider.ExpertTestService.GetAllRequestsAsync(expertId);
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
			var response = await _servicesProvider.ExpertTestService.SubmitResponseAsync(model, expertId);
			return Ok(response);
		}

		[HttpGet("review-requests")]
		public async Task<ActionResult<ReviewTestRequestModel>> GetPendingReviewRequests()
		{
			var expertId = await GetCurrentExpertId();
			var requests = await _servicesProvider.ExpertTestService.GetPendingReviewRequestsAsync(expertId);
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
				var response = await _servicesProvider.ExpertTestService.VoteForResponseAsync(model, expertId);
				return Ok(response);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}