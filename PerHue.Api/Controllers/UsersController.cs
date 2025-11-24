using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerHue.Application.IServices;
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
		private readonly IImageUploadService _imageUploadService;

		public UsersController(IServicesProvider servicesProvider, IImageUploadService imageUploadService)
		{
			_servicesProvider = servicesProvider;
			_imageUploadService = imageUploadService;
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
		[Authorize]
		[HttpPost("upload_profile_picture")]
		public async Task<IActionResult> UploadProfileImage(IFormFile file)
		{
			try
			{
				// Get the User ID from the token's claims
				var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
				/*if (string.IsNullOrEmpty(userIdString))
				{
					return Unauthorized("User ID not found in token.");
				}*/

				// Parse the ID to an integer
				if (!int.TryParse(userIdString, out var id))
				{
					return Unauthorized("Invalid User ID format in token.");
				}

				// Use the 'id' from the token for the rest of the logic
				var result = await _servicesProvider.UserService.GetByIdAsync(id);
				if (result == null)
				{
					return NotFound("User not found.");
				}

				var imageUrl = await _imageUploadService.UploadImageAsync(file);
				if (imageUrl == null)
				{
					return BadRequest("No file was uploaded.");
				}
				result.Profilepicture = imageUrl;

				UpdateUserModel updateUserModel = new UpdateUserModel
				{
					Profilepicture = result.Profilepicture,
					Fullname = result.Fullname,
					Phone = result.Phone,
					Gender = result.Gender,
					Dob = result.Dob
				};

				await _servicesProvider.UserService.UpdateAsync(id, updateUserModel);
				// Return the secure URL of the uploaded image
				return Ok(new { url = imageUrl });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

	}
}
