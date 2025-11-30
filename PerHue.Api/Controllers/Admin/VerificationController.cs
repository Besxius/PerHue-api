using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.VerifyInformation;
using System.Security.Claims;

namespace PerHue.Api.Controllers.Admin;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class VerificationController : ControllerBase
{
	private readonly IVerificationService _verificationService;

	public VerificationController(IVerificationService verificationService)
	{
		_verificationService = verificationService;
	}

	/// <summary>
	/// Get all verification requests with paging, search and sorting
	/// </summary>
	[HttpGet]
	public async Task<IActionResult> GetAllRequests([FromQuery] VerificationSearchModel searchModel)
	{
		try
		{
			var result = await _verificationService.GetAllAsync(searchModel);
			return Ok(result);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetRequestById(int id)
	{
		var request = await _verificationService.GetVerificationRequestByIdAsync(id);
		if (request == null)
		{
			return NotFound();
		}

		return Ok(request);
	}

	[HttpPost("accept/{id}")]
	public async Task<IActionResult> AcceptRequest(int id)
	{
		try
		{
			var result = await _verificationService.AcceptVerificationAsync(id);
			if (result)
				return Ok(new { message = "Verification request accepted" });
			else
				return BadRequest(new { error = "Failed to accept verification request" });
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[HttpDelete("deny/{id}")]
	public async Task<IActionResult> DenyRequest(int id, [FromBody] string reason)
	{
		if (string.IsNullOrWhiteSpace(reason))
		{
			return BadRequest(new { error = "A reason for denial must be provided" });
		}

		try
		{
			var result = await _verificationService.DenyVerificationAsync(id, reason);
			if (result)
				return Ok(new { message = "Verification request denied" });
			else
				return BadRequest(new { error = "Failed to deny verification request" });
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}
}