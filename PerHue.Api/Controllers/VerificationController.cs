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
    private readonly IUserService _userService;

    public VerificationController(IVerificationService verificationService, IUserService userService)
    {
        _verificationService = verificationService;
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllRequests()
    {
        var requests = await _verificationService.GetAllVerificationRequestsAsync();
        return Ok(requests);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRequestById(int id)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && currentUserId != id)
        {
            return Forbid();
        }

        var request = await _verificationService.GetVerificationRequestByIdAsync(id);
        if (request == null)
        {
            return NotFound();
        }

        return Ok(request);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SubmitRequest(VerifyRequestModel model)
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

	[HttpPost("accept/{id}")]
	[Authorize(Roles = "Admin")]
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
	[Authorize(Roles = "Admin")]
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