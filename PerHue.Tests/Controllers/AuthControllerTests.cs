using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PerHue.Api.Controllers;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.Authentication;
using PerHue.Application.Models.User;
using Xunit;

namespace PerHue.Tests.Controllers
{
	/// <summary>
	/// Unit tests for AuthController
	/// Testing all authentication endpoints and business logic branches
	/// </summary>
	public class AuthControllerTests
	{
		private readonly Mock<IServicesProvider> _mockServicesProvider;
		private readonly Mock<IUserService> _mockUserService;
		private readonly Mock<IOtpService> _mockOtpService;
		private readonly Mock<IConfiguration> _mockConfiguration;
		private readonly AuthController _controller;

		public AuthControllerTests()
		{
			_mockServicesProvider = new Mock<IServicesProvider>();
			_mockUserService = new Mock<IUserService>();
			_mockOtpService = new Mock<IOtpService>();
			_mockConfiguration = new Mock<IConfiguration>();

			// Setup service provider
			_mockServicesProvider.Setup(sp => sp.UserService).Returns(_mockUserService.Object);
			_mockServicesProvider.Setup(sp => sp.OtpService).Returns(_mockOtpService.Object);

			// Setup configuration - using index accessor instead of GetValue
			var mockConfigSection = new Mock<IConfigurationSection>();
			mockConfigSection.Setup(s => s.Value).Returns("30");
			_mockConfiguration.Setup(c => c["Jwt:DurationInMinutes"]).Returns("30");
			_mockConfiguration.Setup(c => c.GetSection("Jwt:DurationInMinutes")).Returns(mockConfigSection.Object);

			_controller = new AuthController(_mockServicesProvider.Object, _mockConfiguration.Object);
		}

		#region Login Tests

		[Fact]
		public async Task Login_WithInvalidModelState_ShouldReturnBadRequest()
		{
			// Arrange
			var model = new LoginRequestModel { Email = "", Password = "" };
			_controller.ModelState.AddModelError("Email", "Email is required.");

			// Act
			var result = await _controller.Login(model);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task Login_WithNonExistentUser_ShouldReturnNotFound()
		{
			// Arrange
			var model = new LoginRequestModel 
			{ 
				Email = "nonexistent@example.com", 
				Password = "password123" 
			};

			_mockUserService
				.Setup(us => us.GetByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((UserModel)null!);

			// Act
			var result = await _controller.Login(model);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
			var notFoundResult = result as NotFoundObjectResult;
			notFoundResult?.Value.Should().Be("Tài khoản không tồn tại");
		}

		[Fact]
		public async Task Login_WithInactiveAccount_ShouldReturnForbidden()
		{
			// Arrange
			var model = new LoginRequestModel 
			{ 
				Email = "inactive@example.com", 
				Password = "password123" 
			};

			var inactiveUser = new UserModel
			{
				Id = 1,
				Email = model.Email,
				Isactive = false
			};

			_mockUserService
				.Setup(us => us.GetByEmailAsync(model.Email))
				.ReturnsAsync(inactiveUser);

			// Act
			var result = await _controller.Login(model);

			// Assert
			result.Should().BeOfType<ObjectResult>();
			var objectResult = result as ObjectResult;
			objectResult?.StatusCode.Should().Be(403);
		}

		[Fact]
		public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
		{
			// Arrange
			var model = new LoginRequestModel 
			{ 
				Email = "user@example.com", 
				Password = "wrongpassword" 
			};

			var user = new UserModel
			{
				Id = 1,
				Email = model.Email,
				Isactive = true
			};

			_mockUserService
				.Setup(us => us.GetByEmailAsync(model.Email))
				.ReturnsAsync(user);

			_mockUserService
				.Setup(us => us.ValidateUserAsync(model))
				.ThrowsAsync(new SecurityTokenException("Invalid credentials"));

			// Act
			var result = await _controller.Login(model);

			// Assert
			result.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Fact]
		public async Task Login_WithValidCredentials_ShouldReturnOkWithTokens()
		{
			// Arrange
			var model = new LoginRequestModel 
			{ 
				Email = "user@example.com", 
				Password = "correctpassword" 
			};

			var user = new UserModel
			{
				Id = 1,
				Email = model.Email,
				Isactive = true
			};

			var loginResponse = new LoginResponseModel
			{
				AccessToken = "valid-access-token",
				RefreshToken = "valid-refresh-token"
			};

			_mockUserService
				.Setup(us => us.GetByEmailAsync(model.Email))
				.ReturnsAsync(user);

			_mockUserService
				.Setup(us => us.ValidateUserAsync(model))
				.ReturnsAsync(loginResponse);

			// Act
			var result = await _controller.Login(model);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			var okResult = result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult!.Value.Should().NotBeNull();
		}

		#endregion

		#region Register Tests

		[Fact]
		public async Task Register_WithExistingEmail_ShouldReturnUnauthorized()
		{
			// Arrange
			var model = new CreateUserRequestModel
			{
				Email = "existing@example.com",
				Password = "Password@123",
				ConfirmPassword = "Password@123",
				Gender = false
			};

			var existingUser = new UserModel
			{
				Id = 1,
				Email = model.Email
			};

			_mockUserService
				.Setup(us => us.GetByEmailAsync(model.Email))
				.ReturnsAsync(existingUser);

			// Act
			var result = await _controller.Register(model);

			// Assert
			result.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Fact]
		public async Task Register_WithValidData_ShouldReturnOk()
		{
			// Arrange
			var model = new CreateUserRequestModel
			{
				Email = "newuser@example.com",
				Password = "Password@123",
				ConfirmPassword = "Password@123",
				Gender = false
			};

			_mockUserService
				.Setup(us => us.GetByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((UserModel)null!);

			_mockUserService
				.Setup(us => us.CreateAsync(model))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _controller.Register(model);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			_mockUserService.Verify(us => us.CreateAsync(model), Times.Once);
		}

		#endregion

		#region Refresh Token Tests

		[Fact]
		public async Task Refresh_WithInvalidToken_ShouldReturnUnauthorized()
		{
			// Arrange
			var model = new RefreshTokenRequestModel
			{
				RefreshToken = "invalid-refresh-token"
			};

			_mockUserService
				.Setup(us => us.RefreshTokenAsync(model))
				.ThrowsAsync(new SecurityTokenException("Invalid refresh token"));

			// Act
			var result = await _controller.Refresh(model);

			// Assert
			result.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Fact]
		public async Task Refresh_WithValidToken_ShouldReturnOkWithNewTokens()
		{
			// Arrange
			var model = new RefreshTokenRequestModel
			{
				RefreshToken = "valid-refresh-token"
			};

			var loginResponse = new LoginResponseModel
			{
				AccessToken = "new-access-token",
				RefreshToken = "new-refresh-token"
			};

			_mockUserService
				.Setup(us => us.RefreshTokenAsync(model))
				.ReturnsAsync(loginResponse);

			// Act
			var result = await _controller.Refresh(model);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		#endregion

		#region Send OTP Tests

		[Fact]
		public async Task SendOtp_WithEmptyEmail_ShouldReturnBadRequest()
		{
			// Arrange
			var request = new EmailRequestModel { Email = "" };

			// Act
			var result = await _controller.SendOtp(request);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task SendOtp_WithNonExistentUser_ShouldReturnNotFound()
		{
			// Arrange
			var request = new EmailRequestModel { Email = "nonexistent@example.com" };

			_mockUserService
				.Setup(us => us.UserExistsAsync(request.Email))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.SendOtp(request);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public async Task SendOtp_WhenSuccessful_ShouldReturnOk()
		{
			// Arrange
			var request = new EmailRequestModel { Email = "user@example.com" };

			_mockUserService
				.Setup(us => us.UserExistsAsync(request.Email))
				.ReturnsAsync(true);

			_mockOtpService
				.Setup(os => os.SendOtpToEmailAsync(request.Email))
				.ReturnsAsync(true);

			// Act
			var result = await _controller.SendOtp(request);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		#endregion

		#region Change Password Tests

		[Fact]
		public async Task ChangePassword_WithInvalidOtp_ShouldReturnBadRequest()
		{
			// Arrange
			var model = new ChangePasswordModel
			{
				SentEmail = "user@example.com",
				NewPassword = "Password@123",
				ConfirmPassword = "Password@123",
				Otp = "123456"
			};

			_mockUserService
				.Setup(us => us.ChangePasswordAsync(model))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.ChangePassword(model);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task ChangePassword_WithValidData_ShouldReturnOk()
		{
			// Arrange
			var model = new ChangePasswordModel
			{
				SentEmail = "user@example.com",
				NewPassword = "Password@123",
				ConfirmPassword = "Password@123",
				Otp = "123456"
			};

			_mockUserService
				.Setup(us => us.ChangePasswordAsync(model))
				.ReturnsAsync(true);

			// Act
			var result = await _controller.ChangePassword(model);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		#endregion

		#region Logout Tests

		[Fact]
		public async Task Logout_ShouldReturnOkWithMessage()
		{
			// Arrange & Act
			var result = await _controller.Logout();

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		#endregion
	}
}
