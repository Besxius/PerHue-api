using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.User;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FilesController : ControllerBase
	{
		private readonly IImageUploadService _imageUploadService;
		private readonly IServicesProvider _servicesProvider;
		public FilesController(IImageUploadService imageUploadService, IServicesProvider servicesProvider)
		{
			_imageUploadService = imageUploadService;
			_servicesProvider = servicesProvider;

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


		[HttpPost("upload")]
		public async Task<IActionResult> UploadImage(IFormFile file)
		{
			try
			{
				var imageUrl = await _imageUploadService.UploadImageAsync(file);
				if (imageUrl == null)
				{
					return BadRequest("No file was uploaded.");
				}

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