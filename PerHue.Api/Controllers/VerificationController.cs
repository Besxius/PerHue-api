using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServices;
using PerHue.Application.Models.VerifyInformation;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PerHue.Api.Controllers;

[Route("api/verification")]
[ApiController]
public class VerificationController : ControllerBase
{
	private readonly IVerificationService _verificationService;

	public VerificationController(IVerificationService verificationService)
	{
		_verificationService = verificationService;
	}

	[HttpPost]
	[Authorize]
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> SubmitRequest([FromForm] VerifyRequestModel model)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

		try
		{
			await _verificationService.SubmitVerificationAsync(currentUserId, model);
			return Ok(new { message = "Verification request submitted successfully" });
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[HttpPost("model-photo")]
	[Authorize]
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> SubmitRequestHasPhotoModel([FromForm] VerifyInformationModel model)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

		try
		{
			await _verificationService.Version2SubmitVerificationAsync(currentUserId, model);
			return Ok(new { message = "Verification request submitted successfully" });
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[HttpGet("pending")]
	[Authorize]
	public async Task<IActionResult> CheckPendingRequest()
	{
		var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

		try
		{
			var hasPending = await _verificationService.HasPendingVerificationAsync(currentUserId);
			return Ok(new { hasPending });
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}
}