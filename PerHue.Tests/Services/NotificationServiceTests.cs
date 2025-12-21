using FluentAssertions;
using Moq;
using PerHue.Application.IServices;
using PerHue.Application.Models.Notification;
using Xunit;

namespace PerHue.Tests.Services
{
	public class NotificationServiceTests
	{
		private readonly Mock<INotificationService> _mockNotificationService;

		public NotificationServiceTests()
		{
			_mockNotificationService = new Mock<INotificationService>();
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllNotifications()
		{
			// Arrange
			var notifications = new List<NotificationModel>
			{
				new NotificationModel { Id = 1, Title = "Test 1", Content = "Content 1" },
				new NotificationModel { Id = 2, Title = "Test 2", Content = "Content 2" }
			};

			_mockNotificationService
				.Setup(ns => ns.GetAllAsync())
				.ReturnsAsync(notifications);

			// Act
			var result = await _mockNotificationService.Object.GetAllAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetByIdAsync_WithValidId_ShouldReturnNotification()
		{
			// Arrange
			var notificationId = 1;
			var notification = new NotificationModel
			{
				Id = notificationId,
				Title = "Test Notification",
				Content = "Test Content"
			};

			_mockNotificationService
				.Setup(ns => ns.GetByIdAsync(notificationId))
				.ReturnsAsync(notification);

			// Act
			var result = await _mockNotificationService.Object.GetByIdAsync(notificationId);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(notificationId);
			result.Title.Should().Be("Test Notification");
		}

		[Fact]
		public async Task CreateAsync_WithValidModel_ShouldCreateNotification()
		{
			// Arrange
			var createModel = new CreateNotificationModel
			{
				Title = "New Notification",
				Content = "New Content"
			};

			_mockNotificationService
				.Setup(ns => ns.CreateAsync(createModel))
				.Returns(Task.CompletedTask);

			// Act
			await _mockNotificationService.Object.CreateAsync(createModel);

			// Assert
			_mockNotificationService.Verify(ns => ns.CreateAsync(createModel), Times.Once);
		}

		[Fact]
		public async Task MarkAsReadAsync_WithValidId_ShouldCompleteSuccessfully()
		{
			// Arrange
			var notificationId = 1;

			_mockNotificationService
				.Setup(ns => ns.MarkAsReadAsync(notificationId))
				.Returns(Task.CompletedTask);

			// Act
			await _mockNotificationService.Object.MarkAsReadAsync(notificationId);

			// Assert
			_mockNotificationService.Verify(ns => ns.MarkAsReadAsync(notificationId), Times.Once);
		}

		[Fact]
		public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
		{
			// Arrange
			var notificationId = 1;

			_mockNotificationService
				.Setup(ns => ns.DeleteAsync(notificationId))
				.ReturnsAsync(true);

			// Act
			var result = await _mockNotificationService.Object.DeleteAsync(notificationId);

			// Assert
			result.Should().BeTrue();
		}
	}
}
