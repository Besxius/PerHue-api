using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Infrastructure.Services;
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


		[HttpGet("signin-google")]
		public async Task<IActionResult> SignInGoogleAsync()
		{
			if (User.Identity != null && User.Identity.IsAuthenticated)
			{
				var email = User.FindFirstValue(ClaimTypes.Email);
				return Accepted();
			}
			var redirectUrl = Url.Action("GoogleResponse", "users");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}

		[HttpGet("google-response")]
		public async Task<IActionResult> GoogleResponse()
		{
			var info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			if (User != null && User.Identity.IsAuthenticated)
			{
				var emailCookie = User.FindFirstValue(ClaimTypes.Email);
				var tokenCookie = await _servicesProvider.UserService.ValidateUserAsync(emailCookie);

				return Ok(tokenCookie);
			}

			if (!info.Succeeded)
			{
				var error = info.Failure?.Message ?? "Unknown error";
				Console.WriteLine($"Google Authentication Failed: {error}");
				return Unauthorized();
			}

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, info.Principal, info.Properties);

			var email = info.Principal.FindFirstValue(ClaimTypes.Email);
			var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);

			var user = await _servicesProvider.UserService.GetByEmailAsync(email);
			if (user != null && !user.Isactive)
			{
				return Accepted();
			}

			var token = await _servicesProvider.UserService.ValidateUserAsync(email);
			if (token.Length == 0)
				return BadRequest();

			return Ok(token);
		}

		[HttpPost]
		[Route("register-google")]
		public async Task<IActionResult> CreateUser(CreateUserByEmailModel user)
		{
			var existingUser = await _servicesProvider.UserService.GetByEmailAsync(user.Email);
			if (existingUser != null)
			{
				return BadRequest("Email has exited!");
			}
			await _servicesProvider.UserService.CreateAsync(user);
			return Ok("User created successfully.");
		}

		[HttpPost("get-token")]
		public async Task<IActionResult> GetToken(string email)
		{
			var user = await _servicesProvider.UserService.GetByEmailAsync(email);
			//var claims = User.Claims.ToList();
			//claims.Add(new Claim("UserId", user.Id.ToString()));
			//claims.Add(new Claim(ClaimTypes.Role, user.RoleId.ToString()));

			//var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			//var principal = new ClaimsPrincipal(identity);

			//// Đăng nhập lại để cập nhật claims
			//await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

			return await _servicesProvider.UserService.ValidateUserAsync(email) is string token
				? Ok(token)
				: BadRequest("Failed to get token.");
		}

		[HttpPost("signout")]
		public async Task<IActionResult> Logout()
		{
			// Đăng xuất khỏi cookie authentication
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return Ok(new { message = "Đăng xuất thành công" });
		}

		// GET: api/Users
		[HttpGet]
		public async Task<IEnumerable<UserModel>> GetUsers()
		{
			return await _servicesProvider.UserService.GetAllAsync();
		}

		// GET: api/Users/5
		[HttpGet("{id}")]
		public async Task<ActionResult<UserModel>> GetUser(int id)
		{
			var user = await _servicesProvider.UserService.GetByIdAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		// GET: api/Users
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

		// PUT: api/Users/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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
		

		// DELETE: api/Users/5
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
		[Route("login")]
		public async Task<IActionResult> Login(LoginModel model)
		{
			var account = await _servicesProvider.UserService.GetByEmailAsync(model.Email);
			if (account is null)
				return NotFound();
			if (account.Isactive == false)
				return Accepted();
			var token = await _servicesProvider.UserService.ValidateUserAsync(model);
			if (token.Length == 0)
				return BadRequest();

			return Ok(token);
		}
		//[HttpPost]
		//[Route("change-password")]
		//public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
		//{
		//	if (await _servicesProvider.UserService.ChangePasswordAsync(model))
		//	{
		//		return Ok();
		//	}
		//	return BadRequest("Failed to change password.");
		//}

		// POST: api/Users
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		[Route("register")]
		public async Task PostUser(CreateUserModel user)
		{
			await _servicesProvider.UserService.CreateAsync(user);
		}
	}
}
