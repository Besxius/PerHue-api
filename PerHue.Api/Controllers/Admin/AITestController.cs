using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using System.Security.Claims;
using static PerHue.Application.Models.AiTestModel;

namespace PerHue.Api.Controllers.Admin
{
	[Route("api/admin/[controller]")]
	[ApiController]
	[Authorize(Roles = "admin")]
	public class AITestController : ControllerBase
	{
		private readonly IAiTestService _aiTestService;

		public AITestController(IAiTestService aiTestService)
		{
			_aiTestService = aiTestService;
		}

		///// <summary>
		///// Get AI test result by test request ID
		///// </summary>
		///// <param name="testRequestId">Test Request ID</param>
		///// <returns>AI test result details</returns>
		//[HttpGet("result/{testRequestId}")]
		//public async Task<ActionResult<ServiceResponse<AiTestResponseModel>>> GetAIResult(int testRequestId)
		//{
		//	try
		//	{
		//		// Extract user ID from JWT token claims for validation
		//		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		//		if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
		//		{
		//			return ServiceResponse<AiTestResponseModel>.Unauthorized("Invalid or missing user ID in token");
		//		}

		//		// For admin, we can get any user's test result by temporarily using their ID
		//		// In a real scenario, you might want to modify the service to allow admin access
		//		var result = await _aiTestService.GetAiTestResultAsync(testRequestId, currentUserId);
				
		//		if (result == null)
		//		{
		//			return ServiceResponse<AiTestResponseModel>.NotFound("AI test result not found");
		//		}

		//		return ServiceResponse<AiTestResponseModel>.Ok(result, "AI test result retrieved successfully");
		//	}
		//	catch (Exception ex)
		//	{
		//		return ServiceResponse<AiTestResponseModel>.Error($"An error occurred while retrieving the AI test result: {ex.Message}");
		//	}
		//}

		///// <summary>
		///// Get all AI tests for a specific user
		///// </summary>
		///// <param name="userId">User ID</param>
		///// <returns>List of AI test results for the user</returns>
		//[HttpGet("user/{userId}/tests")]
		//public async Task<ActionResult<ServiceResponse<List<AiTestResponseModel>>>> GetUserAIResults(int userId)
		//{
		//	try
		//	{
		//		var results = await _aiTestService.GetUserAiTestsAsync(userId);
		//		return ServiceResponse<List<AiTestResponseModel>>.Ok(results, "User AI test results retrieved successfully");
		//	}
		//	catch (Exception ex)
		//	{
		//		return ServiceResponse<List<AiTestResponseModel>>.Error($"An error occurred while retrieving user AI test results: {ex.Message}");
		//	}
		//}

		/// <summary>
		/// Get AI test results with pagination and filtering
		/// </summary>
		/// <param name="searchModel">Search and pagination parameters</param>
		/// <returns>Paginated list of AI test results</returns>
		[HttpGet("tests")]
		public async Task<ActionResult<ServiceResponse<PaginatedResultV2<AiTestResponseModel>>>> GetAITests([FromQuery] AiTestSearchModel searchModel)
		{
			try
			{
				var paginatedResult = await _aiTestService.GetAiTestsWithFilterAsync(searchModel);
				return ServiceResponse<PaginatedResultV2<AiTestResponseModel>>.Ok(paginatedResult, "AI tests retrieved successfully");
			}
			catch (Exception ex)
			{
				return ServiceResponse<PaginatedResultV2<AiTestResponseModel>>.Error($"An error occurred while retrieving AI tests: {ex.Message}");
			}
		}

		///// <summary>
		///// Process AI test manually (for admin use)
		///// </summary>
		///// <param name="testRequestId">Test Request ID to process</param>
		///// <returns>Processing result</returns>
		//[HttpPost("process/{testRequestId}")]
		//public async Task<ActionResult<ServiceResponse<AiTestResponseModel>>> ProcessAITest(int testRequestId)
		//{
		//	try
		//	{
		//		var result = await _aiTestService.ProcessAiTestAsync(testRequestId);
		//		return ServiceResponse<AiTestResponseModel>.Ok(result, "AI test processed successfully");
		//	}
		//	catch (Exception ex)
		//	{
		//		return ServiceResponse<AiTestResponseModel>.Error($"An error occurred while processing AI test: {ex.Message}");
		//	}
		//}
		[HttpPatch("tests/{testId}/confirm")]
		public async Task<ActionResult<ServiceResponse<bool>>> MarkTestAsCompleted(int testId)
		{
			try
			{
				var result = await _aiTestService.MarkTestAsCompletedAsync(testId);
				if (result)
				{
					return ServiceResponse<bool>.Ok(true, "AI test marked as completed successfully");
				}
				else
				{
					return ServiceResponse<bool>.NotFound("AI test not found");
				}
			}
			catch (Exception ex)
			{
				return ServiceResponse<bool>.Error($"An error occurred while marking AI test as completed: {ex.Message}");
			}
		}
	}
}
