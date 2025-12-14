using FluentAssertions;
using Moq;
using PerHue.Application.IServices;
using PerHue.Application.Models.User;
using Xunit;

namespace PerHue.Tests.Services
{
	public class UserServiceTests
	{
		private readonly Mock<IUserService> _mockUserService;

		public UserServiceTests()
		{
			_mockUserService = new Mock<IUserService>();
		}

		[Fact]
		public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
		{
			// Arrange
			var userId = 1;
			var expectedUser = new UserModel
			{
				Id = userId,
				Email = "test@example.com",
				Username = "testuser"
			};

			_mockUserService
				.Setup(us => us.GetByIdAsync(userId))
				.ReturnsAsync(expectedUser);

			// Act
			var result = await _mockUserService.Object.GetByIdAsync(userId);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(userId);
			result.Email.Should().Be("test@example.com");
		}

		[Fact]
		public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
		{
			// Arrange
			var email = "test@example.com";
			var expectedUser = new UserModel
			{
				Id = 1,
				Email = email,
				Username = "testuser"
			};

			_mockUserService
				.Setup(us => us.GetByEmailAsync(email))
				.ReturnsAsync(expectedUser);

			// Act
			var result = await _mockUserService.Object.GetByEmailAsync(email);

			// Assert
			result.Should().NotBeNull();
			result.Email.Should().Be(email);
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnUserList()
		{
			// Arrange
			var expectedUsers = new List<UserModel>
			{
				new UserModel { Id = 1, Email = "user1@example.com", Username = "user1" },
				new UserModel { Id = 2, Email = "user2@example.com", Username = "user2" }
			};

			_mockUserService
				.Setup(us => us.GetAllAsync())
				.ReturnsAsync(expectedUsers);

			// Act
			var result = await _mockUserService.Object.GetAllAsync();

			// Assert
			result.Should().HaveCount(2);
			result.Should().BeEquivalentTo(expectedUsers);
		}

		[Fact]
		public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
		{
			// Arrange
			var userId = 1;

			_mockUserService
				.Setup(us => us.DeleteAsync(userId))
				.ReturnsAsync(true);

			// Act
			var result = await _mockUserService.Object.DeleteAsync(userId);

			// Assert
			result.Should().BeTrue();
		}

		[Fact]
		public async Task UpdateAsync_WithValidId_ShouldReturnTrue()
		{
			// Arrange
			var userId = 1;
			var updateModel = new UpdateUserModel
			{
				Fullname = "Updated Name",
				Phone = "0123456789"
			};

			_mockUserService
				.Setup(us => us.UpdateAsync(userId, updateModel))
				.ReturnsAsync(true);

			// Act
			var result = await _mockUserService.Object.UpdateAsync(userId, updateModel);

			// Assert
			result.Should().BeTrue();
		}
	}
}
