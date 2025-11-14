using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.Authentication;
using PerHue.Application.Models.User;

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

		[HttpPost]
		[Route("login")]
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

			return Ok(token);
		}

		[HttpPost]
		[Route("register")]
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
					Audience = new List<string> { _configuration["Authentication:Google:ClientId"] }
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

			var token = _servicesProvider.UserService.GenerateTokenForUser(account);

			if (string.IsNullOrEmpty(token))
			{
				return StatusCode(500, "Lỗi khi tạo token truy cập.");
			}

			return Ok(new
			{
				accessToken = token,
				tokenType = "Bearer",
			});
		}

		[HttpPost("get-token")]
		public async Task<IActionResult> GetToken(string email)
		{
			var user = await _servicesProvider.UserService.GetByEmailAsync(email);

			return await _servicesProvider.UserService.ValidateUserAsync(email) is string token
				? Ok(token)
				: BadRequest("Failed to get token.");
		}
	}
}
