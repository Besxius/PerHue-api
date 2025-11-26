using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.Authentication;
using PerHue.Application.Models.User;
using System.Security.Claims;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		private readonly IConfiguration _configuration;

		public AuthController(IServicesProvider servicesProvider, IConfiguration configuration)
		{
			_servicesProvider = servicesProvider;
			_configuration = configuration;
		}

		/*[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequestModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var account = await _servicesProvider.UserService.GetByEmailAsync(model.Email);
			if (account is null) return NotFound("Tên đăng nhập hoặc mật khẩu không đúng.");
			if (account.Isactive == false) return Accepted("Tài khoản chưa được kích hoạt hoặc đã bị khóa.");

			var token = await _servicesProvider.UserService.ValidateUserAsync(model);
			if (string.IsNullOrEmpty(token))
			{
				return Unauthorized("Tên đăng nhập hoặc mật khẩu không đúng.");
			}

			return Ok(new
			{
				accessToken = token,
				tokenType = "Bearer",
				expiresIn = 120,
				// refresh_token = "..." // (Tùy chọn) Nếu bạn dùng refresh token
			});
		}*/
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequestModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// 1. Get user and check active status
			var account = await _servicesProvider.UserService.GetByEmailAsync(model.Email);
			if (account is null)
			{
				return Unauthorized("Tên đăng nhập hoặc mật khẩu không đúng.");
			}

			if (account.Isactive == false)
			{
				return StatusCode(403, "Tài khoản chưa được kích hoạt hoặc đã bị khóa.");
			}

			try
			{
				// 2. Validate user and get tokens
				var loginResponse = await _servicesProvider.UserService.ValidateUserAsync(model);

				// 3. Get expiration time from config (e.g., 30) and convert to seconds (e.g., 1800)
				var expiresInMinutes = _configuration.GetValue<int>("Jwt:DurationInMinutes");
				//var expiresInSeconds = expiresInMinutes * 60;

				// 4. Return the new object with all fields
				return Ok(new
				{
					accessToken = loginResponse.AccessToken,
					refreshToken = loginResponse.RefreshToken,
					tokenType = "Bearer",
					expiresIn = expiresInMinutes
				});
			}
			catch (SecurityTokenException ex)
			{
				return Unauthorized(ex.Message);
			}
		}

		[HttpPost("register")]
		public async Task Register(CreateUserRequestModel user)
		{
			await _servicesProvider.UserService.CreateAsync(user);
		}

		[HttpPost("google")]
		public async Task<IActionResult> GoogleLogin([FromBody] GoogleTokenModel tokenDto)
		{
			if (string.IsNullOrEmpty(tokenDto.IdToken))
			{
				return BadRequest("ID Token không được cung cấp.");
			}

			GoogleJsonWebSignature.Payload payload;
			try
			{
				var settings = new GoogleJsonWebSignature.ValidationSettings
				{
					Audience = new List<string> { _configuration["Google:ClientId"] }
				};

				payload = await GoogleJsonWebSignature.ValidateAsync(tokenDto.IdToken, settings);
			}
			catch (InvalidJwtException)
			{
				return Unauthorized("Google ID Token không hợp lệ hoặc đã hết hạn.");
			}

			var userEmail = payload.Email;
			var account = await _servicesProvider.UserService.GetByEmailAsync(userEmail);

			if (account is null)
			{
				account = await _servicesProvider.UserService.CreateOrLinkGoogleUserAsync(payload.Email, payload.Name, payload.Picture);
			}

			if (account is null) return StatusCode(500, "Không thể tạo hoặc tìm thấy tài khoản người dùng.");
			if (account.Isactive == false) return Accepted("Tài khoản chưa được kích hoạt hoặc đã bị khóa.");

			var loginResponse = await _servicesProvider.UserService.ValidateUserAsync(userEmail);
			var expiresInMinutes = _configuration.GetValue<int>("Jwt:DurationInMinutes");

			if (string.IsNullOrEmpty(loginResponse.AccessToken))
			{
				return StatusCode(500, "Lỗi khi tạo token truy cập.");
			}

			return Ok(new
			{
				accessToken = loginResponse.AccessToken,
				refreshToken = loginResponse.RefreshToken,
				tokenType = "Bearer",
				expiresIn = expiresInMinutes
			});
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			return Ok(new { Message = "Đăng xuất thành công. Token đã được client loại bỏ." });
		}

		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				var loginResponse = await _servicesProvider.UserService.RefreshTokenAsync(model);

				// Get expiration time from config
				var expiresInMinutes = _configuration.GetValue<int>("Jwt:DurationInMinutes");
				//var expiresInSeconds = expiresInMinutes * 60;

				return Ok(new
				{
					accessToken = loginResponse.AccessToken,
					refreshToken = loginResponse.RefreshToken,
					tokenType = "Bearer",
					expiresIn = expiresInMinutes
				});
			}
			catch (SecurityTokenException ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
			}
		}

		/// <summary>
		/// Get current logged-in user information (stateless)
		/// </summary>
		/// <returns>Current user information</returns>
		[HttpGet]
		[Route("user-info")]
		public async Task<IActionResult> GetCurrentUserInfo()
		{
			try
			{
				// Extract user ID from JWT token claims
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
				{
					return Unauthorized(new { message = "Invalid or missing user ID in token" });
				}

				// Get user information from database
				var userInfo = await _servicesProvider.UserService.GetUserInfoAsync(userId);

				if (userInfo == null)
				{
					return NotFound(new { message = "User not found" });
				}

				// Check if user is still active
				if (!userInfo.Isactive)
				{
					return StatusCode(403, new { message = "User account is deactivated" });
				}

				return Ok(new
				{
					code = 200,
					result = new
					{
						id = userId,
						avatar = userInfo.Profilepicture,
						username = userInfo.Username,
						email = userInfo.Email,
						phoneNumber = userInfo.Phone,
						roles = new List<string> { userInfo.RoleName }
					},
					success = true,
					message = "User information retrieved successfully"
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					success = false,
					message = "An error occurred while retrieving user information",
					error = ex.Message
				});
			}
		}
	}
}
