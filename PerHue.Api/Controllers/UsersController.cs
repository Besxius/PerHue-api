using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.Authentication;
using PerHue.Application.Models.User;
using System.Security.Claims;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public UsersController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		[Authorize]
		[HttpGet("information")]
		public async Task<ActionResult<UserModel>> GetUserInforamtion()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = await _servicesProvider.UserService.GetByIdAsync(int.Parse(userId));

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutUser(int id, UpdateUserModel model)
		{
			try
			{
				await _servicesProvider.UserService.UpdateAsync(id, model);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await UserExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var user = await _servicesProvider.UserService.GetByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			await _servicesProvider.UserService.DeleteAsync(user.Email);

			return NoContent();
		}

		private async Task<bool> UserExists(int id)
		{
			var result = await _servicesProvider.UserService.GetByIdAsync(id);
			return result is not null;
		}

		[HttpPost]
		[Route("change-password")]
		public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
		{
			if (await _servicesProvider.UserService.ChangePasswordAsync(model))
			{
				return Ok();
			}
			return BadRequest("Failed to change password.");
		}

		[HttpPost("send-otp")]
		public async Task<IActionResult> SendOtp([FromBody] EmailRequestModel request)
		{
			bool isSent = await _servicesProvider.OtpService.SendOtpToEmailAsync(request.Email);
			return isSent ? Ok("OTP sent successfully.") : BadRequest("Failed to send OTP.");
		}

		[HttpPost("verify-otp-demo")]
		public IActionResult VerifyOtp([FromBody] VerifyOtpRequestModel request)
		{
			bool isValid = _servicesProvider.OtpService.VerifyOtp(request.Email, request.Otp);
			return isValid ? Ok("OTP verified!") : BadRequest("Invalid OTP.");
		}

		[HttpPost]
		[Route("change-password-Test-Only")]
		public async Task<IActionResult> ChangePasswordTestOnly(int id, string newPassword)
		{
			if (await _servicesProvider.UserService.ChangePasswordAsync(id, newPassword))
			{
				return Ok();
			}
			return BadRequest("Failed to change password.");
		}

	}
}
