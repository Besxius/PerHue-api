using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerHue.Api.Controllers;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.Notification;
using System.Security.Claims;
using Xunit;

namespace PerHue.Tests.Controllers
{
	public class NotificationsControllerTests
	{
		private readonly Mock<IServicesProvider> _mockServicesProvider;
		private readonly Mock<INotificationService> _mockNotificationService;
		private readonly NotificationsController _controller;

		public NotificationsControllerTests()
		{
			_mockServicesProvider = new Mock<IServicesProvider>();
			_mockNotificationService = new Mock<INotificationService>();

			_mockServicesProvider.Setup(sp => sp.NotificationService).Returns(_mockNotificationService.Object);

			_controller = new NotificationsController(_mockServicesProvider.Object);
		}

		private void SetupUserClaims(int userId)
		{
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
		}

		#region GetMyNotifications Tests

		[Fact]
		public async Task GetMyNotifications_WithValidUser_ShouldReturnOkResult()
		{
			// Arrange
			var userId = 1;
			var notifications = new List<NotificationModel>
			{
				new NotificationModel { Id = 1, Receiver = userId, Title = "Test 1", IsRead = false },
				new NotificationModel { Id = 2, Receiver = userId, Title = "Test 2", IsRead = true }
			};

			SetupUserClaims(userId);

			_mockNotificationService
				.Setup(ns => ns.GetByReceiverAsync(userId))
				.ReturnsAsync(notifications);

			// Act
			var result = await _controller.GetMyNotifications();

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var returnedNotifications = okResult!.Value as IEnumerable<NotificationModel>;
			returnedNotifications.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetMyNotifications_WithInvalidUserId_ShouldReturnUnauthorized()
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

			// Act
			var result = await _controller.GetMyNotifications();

			// Assert
			result.Result.Should().BeOfType<UnauthorizedObjectResult>();
		}

		#endregion

		#region GetMyUnreadNotifications Tests

		[Fact]
		public async Task GetMyUnreadNotifications_WithValidUser_ShouldReturnOnlyUnread()
		{
			// Arrange
			var userId = 1;
			var unreadNotifications = new List<NotificationModel>
			{
				new NotificationModel { Id = 1, Receiver = userId, Title = "Unread 1", IsRead = false },
				new NotificationModel { Id = 2, Receiver = userId, Title = "Unread 2", IsRead = false }
			};

			SetupUserClaims(userId);

			_mockNotificationService
				.Setup(ns => ns.GetUnreadByReceiverAsync(userId))
				.ReturnsAsync(unreadNotifications);

			// Act
			var result = await _controller.GetMyUnreadNotifications();

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var returnedNotifications = okResult!.Value as IEnumerable<NotificationModel>;
		returnedNotifications.Should().HaveCount(2);
		returnedNotifications.Should().OnlyContain(n => n.IsRead == false);
		}

		[Fact]
		public async Task GetMyUnreadNotifications_WithInvalidUserId_ShouldReturnUnauthorized()
		{
			// Arrange
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, "not_a_number")
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			// Act
			var result = await _controller.GetMyUnreadNotifications();

			// Assert
			result.Result.Should().BeOfType<UnauthorizedObjectResult>();
		}

		#endregion

		#region MarkAsRead Tests

		[Fact]
		public async Task MarkAsRead_WithValidNotification_ShouldReturnOkResult()
		{
			// Arrange
			var userId = 1;
			var notificationId = 1;

			SetupUserClaims(userId);

			_mockNotificationService
				.Setup(ns => ns.MarkAsReadAsync(notificationId))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _controller.MarkAsRead(notificationId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			_mockNotificationService.Verify(ns => ns.MarkAsReadAsync(notificationId), Times.Once);
		}

		[Fact]
		public async Task MarkAsRead_WithInvalidUserId_ShouldReturnUnauthorized()
		{
			// Arrange
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, "abc")
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			// Act
			var result = await _controller.MarkAsRead(1);

			// Assert
			result.Should().BeOfType<UnauthorizedObjectResult>();
		}

		#endregion

		#region MarkAllAsRead Tests

		[Fact]
		public async Task MarkAllAsRead_WithValidUser_ShouldReturnOkResult()
		{
			// Arrange
			var userId = 1;

			SetupUserClaims(userId);

			_mockNotificationService
				.Setup(ns => ns.MarkAllAsReadAsync(userId))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _controller.MarkAllAsRead();

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			_mockNotificationService.Verify(ns => ns.MarkAllAsReadAsync(userId), Times.Once);
		}

		[Fact]
		public async Task MarkAllAsRead_WithInvalidUserId_ShouldReturnUnauthorized()
		{
			// Arrange
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, "xyz")
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			// Act
			var result = await _controller.MarkAllAsRead();

			// Assert
			result.Should().BeOfType<UnauthorizedObjectResult>();
		}

		#endregion
	}
}
