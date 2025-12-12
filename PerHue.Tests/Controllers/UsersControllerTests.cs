using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PerHue.Api.Controllers;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.Authentication;
using PerHue.Application.Models.User;
using System.Security.Claims;
using Xunit;

namespace PerHue.Tests.Controllers
{
	/// <summary>
	/// Unit tests for UsersController
	/// Testing all user management endpoints and business logic branches
	/// </summary>
	public class UsersControllerTests
	{
		private readonly Mock<IServicesProvider> _mockServicesProvider;
		private readonly Mock<IUserService> _mockUserService;
		private readonly Mock<IOtpService> _mockOtpService;
		private readonly Mock<IImageUploadService> _mockImageUploadService;
		private readonly UsersController _controller;

		public UsersControllerTests()
		{
			_mockServicesProvider = new Mock<IServicesProvider>();
			_mockUserService = new Mock<IUserService>();
			_mockOtpService = new Mock<IOtpService>();
			_mockImageUploadService = new Mock<IImageUploadService>();

			// Setup service provider
			_mockServicesProvider.Setup(sp => sp.UserService).Returns(_mockUserService.Object);
			_mockServicesProvider.Setup(sp => sp.OtpService).Returns(_mockOtpService.Object);

			_controller = new UsersController(_mockServicesProvider.Object, _mockImageUploadService.Object);
		}

		#region GetUserInformation Tests

		[Fact]
		public async Task GetUserInformation_WithValidUser_ShouldReturnUser()
		{
			// Arrange
			var userId = 1;
			var user = new UserModel
			{
				Id = userId,
				Email = "test@example.com",
				Username = "testuser",
				Isactive = true
			};

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId.ToString())
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

		_mockUserService
			.Setup(us => us.GetByIdAsync(userId))
			.ReturnsAsync(user);

		// Act
		var result = await _controller.GetUserInforamtion();

		// Assert
		result.Value.Should().NotBeNull();
		result.Value!.Id.Should().Be(userId);
		result.Value.Email.Should().Be("test@example.com");
	}		[Fact]
		public async Task GetUserInformation_WithNonExistentUser_ShouldReturnNotFound()
		{
			// Arrange
			var userId = 999;
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId.ToString())
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			_mockUserService
				.Setup(us => us.GetByIdAsync(userId))
				.ReturnsAsync((UserModel)null!);

			// Act
			var result = await _controller.GetUserInforamtion();

			// Assert
			result.Result.Should().BeOfType<NotFoundResult>();
		}

		#endregion

		#region PutUser Tests

		[Fact]
		public async Task PutUser_WithValidData_ShouldReturnNoContent()
		{
			// Arrange
			var userId = 1;
			var updateModel = new UpdateUserModel
			{
				Fullname = "Updated Name",
				Phone = "1234567890",
				Gender = true,
				Dob = new DateOnly(1990, 1, 1)
			};

			_mockUserService
				.Setup(us => us.UpdateAsync(userId, updateModel))
				.Returns(Task.FromResult(true));

			// Act
			var result = await _controller.PutUser(userId, updateModel);

			// Assert
			result.Should().BeOfType<NoContentResult>();
			_mockUserService.Verify(us => us.UpdateAsync(userId, updateModel), Times.Once);
		}

		[Fact]
		public async Task PutUser_WithNonExistentUser_ShouldReturnNotFound()
		{
			// Arrange
			var userId = 999;
			var updateModel = new UpdateUserModel
			{
				Fullname = "Updated Name"
			};

			_mockUserService
				.Setup(us => us.UpdateAsync(userId, updateModel))
				.ThrowsAsync(new DbUpdateConcurrencyException());

			_mockUserService
				.Setup(us => us.GetByIdAsync(userId))
				.ReturnsAsync((UserModel)null!);

			// Act
			var result = await _controller.PutUser(userId, updateModel);

			// Assert
			result.Should().BeOfType<NotFoundResult>();
		}

		[Fact]
		public async Task PutUser_WithConcurrencyException_ShouldRethrow()
		{
			// Arrange
			var userId = 1;
			var updateModel = new UpdateUserModel
			{
				Fullname = "Updated Name"
			};

			var existingUser = new UserModel
			{
				Id = userId,
				Email = "test@example.com",
				Username = "testuser"
			};

			_mockUserService
				.Setup(us => us.UpdateAsync(userId, updateModel))
				.ThrowsAsync(new DbUpdateConcurrencyException());

			_mockUserService
				.Setup(us => us.GetByIdAsync(userId))
				.ReturnsAsync(existingUser);

			// Act & Assert
			await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
				async () => await _controller.PutUser(userId, updateModel)
			);
		}

		#endregion

		#region DeleteUser Tests

		[Fact]
		public async Task DeleteUser_WithValidUser_ShouldReturnNoContent()
		{
			// Arrange
			var userId = 1;
			var user = new UserModel
			{
				Id = userId,
				Email = "test@example.com",
				Username = "testuser"
			};

			_mockUserService
				.Setup(us => us.GetByIdAsync(userId))
				.ReturnsAsync(user);

			_mockUserService
				.Setup(us => us.DeleteAsync(user.Email))
				.Returns(Task.FromResult(true));

			// Act
			var result = await _controller.DeleteUser(userId);

			// Assert
			result.Should().BeOfType<NoContentResult>();
			_mockUserService.Verify(us => us.DeleteAsync(user.Email), Times.Once);
		}

		[Fact]
		public async Task DeleteUser_WithNonExistentUser_ShouldReturnNotFound()
		{
			// Arrange
			var userId = 999;

			_mockUserService
				.Setup(us => us.GetByIdAsync(userId))
				.ReturnsAsync((UserModel)null!);

			// Act
			var result = await _controller.DeleteUser(userId);

			// Assert
			result.Should().BeOfType<NotFoundResult>();
			_mockUserService.Verify(us => us.DeleteAsync(It.IsAny<string>()), Times.Never);
		}

		#endregion

		#region VerifyOtp Tests

		[Fact]
		public void VerifyOtp_WithValidOtp_ShouldReturnOk()
		{
			// Arrange
			var request = new VerifyOtpRequestModel
			{
				Email = "test@example.com",
				Otp = "123456"
			};

			_mockOtpService
				.Setup(os => os.VerifyOtp(request.Email, request.Otp))
				.Returns(true);

			// Act
			var result = _controller.VerifyOtp(request);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			var okResult = result as OkObjectResult;
			okResult!.Value.Should().Be("OTP verified!");
		}

		[Fact]
		public void VerifyOtp_WithInvalidOtp_ShouldReturnBadRequest()
		{
			// Arrange
			var request = new VerifyOtpRequestModel
			{
				Email = "test@example.com",
				Otp = "000000"
			};

			_mockOtpService
				.Setup(os => os.VerifyOtp(request.Email, request.Otp))
				.Returns(false);

			// Act
			var result = _controller.VerifyOtp(request);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			var badRequestResult = result as BadRequestObjectResult;
			badRequestResult!.Value.Should().Be("Invalid OTP.");
		}

		#endregion

		#region ChangePasswordTestOnly Tests

		[Fact]
		public async Task ChangePasswordTestOnly_WithSuccess_ShouldReturnOk()
		{
			// Arrange
			var userId = 1;
			var newPassword = "NewPassword@123";

			_mockUserService
				.Setup(us => us.ChangePasswordAsync(userId, newPassword))
				.ReturnsAsync(true);

			// Act
			var result = await _controller.ChangePasswordTestOnly(userId, newPassword);

			// Assert
			result.Should().BeOfType<OkResult>();
		}

		[Fact]
		public async Task ChangePasswordTestOnly_WithFailure_ShouldReturnBadRequest()
		{
			// Arrange
			var userId = 1;
			var newPassword = "NewPassword@123";

			_mockUserService
				.Setup(us => us.ChangePasswordAsync(userId, newPassword))
				.ReturnsAsync(false);

			// Act
			var result = await _controller.ChangePasswordTestOnly(userId, newPassword);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			var badRequestResult = result as BadRequestObjectResult;
			badRequestResult!.Value.Should().Be("Failed to change password.");
		}

		#endregion

		#region UploadProfilePicture Tests

		[Fact]
		public async Task UploadProfilePicture_WithValidFile_ShouldReturnOkWithUrl()
		{
			// Arrange
			var userId = 1;
			var imageUrl = "https://cloudinary.com/image.jpg";
			var user = new UserModel
			{
				Id = userId,
				Email = "test@example.com",
				Username = "testuser",
				Fullname = "Test User",
				Phone = "1234567890",
				Gender = true,
				Dob = new DateOnly(1990, 1, 1)
			};

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId.ToString())
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			var fileMock = new Mock<IFormFile>();
			fileMock.Setup(f => f.Length).Returns(1024);
			fileMock.Setup(f => f.FileName).Returns("test.jpg");

			_mockUserService
				.Setup(us => us.GetByIdAsync(userId))
				.ReturnsAsync(user);

			_mockImageUploadService
				.Setup(ius => ius.UploadImageAsync(It.IsAny<IFormFile>()))
				.ReturnsAsync(imageUrl);

			_mockUserService
				.Setup(us => us.UpdateAsync(userId, It.IsAny<UpdateUserModel>()))
				.Returns(Task.FromResult(true));

			// Act
			var result = await _controller.UploadProfileImage(fileMock.Object);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			var okResult = result as OkObjectResult;
			okResult.Should().NotBeNull();
			var value = okResult!.Value;
			value.Should().NotBeNull();
			var urlProperty = value!.GetType().GetProperty("url")?.GetValue(value)?.ToString();
			urlProperty.Should().Be(imageUrl);

			_mockUserService.Verify(us => us.UpdateAsync(userId, It.IsAny<UpdateUserModel>()), Times.Once);
		}

		[Fact]
		public async Task UploadProfilePicture_WithInvalidUserId_ShouldReturnUnauthorized()
		{
			// Arrange
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, "invalid")
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			var fileMock = new Mock<IFormFile>();

			// Act
			var result = await _controller.UploadProfileImage(fileMock.Object);

			// Assert
			result.Should().BeOfType<UnauthorizedObjectResult>();
			var unauthorizedResult = result as UnauthorizedObjectResult;
			unauthorizedResult!.Value.Should().Be("Invalid User ID format in token.");
		}

		[Fact]
		public async Task UploadProfilePicture_WithNonExistentUser_ShouldReturnNotFound()
		{
			// Arrange
			var userId = 999;
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId.ToString())
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			var fileMock = new Mock<IFormFile>();

			_mockUserService
				.Setup(us => us.GetByIdAsync(userId))
				.ReturnsAsync((UserModel)null!);

			// Act
			var result = await _controller.UploadProfileImage(fileMock.Object);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
			var notFoundResult = result as NotFoundObjectResult;
			notFoundResult!.Value.Should().Be("User not found.");
		}

		[Fact]
		public async Task UploadProfilePicture_WithNoFile_ShouldReturnBadRequest()
		{
			// Arrange
			var userId = 1;
			var user = new UserModel
			{
				Id = userId,
				Email = "test@example.com",
				Username = "testuser"
			};

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId.ToString())
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			var fileMock = new Mock<IFormFile>();

			_mockUserService
				.Setup(us => us.GetByIdAsync(userId))
				.ReturnsAsync(user);

			_mockImageUploadService
				.Setup(ius => ius.UploadImageAsync(It.IsAny<IFormFile>()))
				.ReturnsAsync((string)null!);

			// Act
			var result = await _controller.UploadProfileImage(fileMock.Object);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			var badRequestResult = result as BadRequestObjectResult;
			badRequestResult!.Value.Should().Be("No file was uploaded.");
		}

		[Fact]
		public async Task UploadProfilePicture_WithException_ShouldReturnInternalServerError()
		{
			// Arrange
			var userId = 1;
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId.ToString())
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			var fileMock = new Mock<IFormFile>();

			_mockUserService
				.Setup(us => us.GetByIdAsync(userId))
				.ThrowsAsync(new Exception("Database error"));

			// Act
			var result = await _controller.UploadProfileImage(fileMock.Object);

			// Assert
			result.Should().BeOfType<ObjectResult>();
			var objectResult = result as ObjectResult;
			objectResult!.StatusCode.Should().Be(500);
			objectResult.Value.Should().NotBeNull();
			objectResult.Value!.ToString().Should().Contain("Internal server error");
		}

		#endregion
	}
}
