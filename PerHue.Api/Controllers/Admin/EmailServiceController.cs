using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.Models;
using PerHue.Infrastructure.Services;

namespace PerHue.Api.Controllers.Admin
{
	[Route("api/admin/[controller]")]
	[ApiController]
	[Authorize(Roles = "admin")]
	public class EmailServiceController(EmailService emailService) : ControllerBase
	{
		private readonly EmailService _emailService = emailService;

		// Gửi email về cho người dùng
		[HttpPost("send-email")]
		public async Task<IActionResult> SendEmail([FromBody] EmailServiceRequestModel request)
		{
			try
			{
				await _emailService.SendEmailAsync(request.ToEmail, request.Subject, request.Body);
				return Ok(new { message = "Email sent successfully." });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
