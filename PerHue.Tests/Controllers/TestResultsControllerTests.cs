using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerHue.Api.Controllers;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.ExpertTestResult;
using System.Security.Claims;
using Xunit;

namespace PerHue.Tests.Controllers
{
	public class TestResultsControllerTests
	{
		private readonly Mock<IServicesProvider> _mockServicesProvider;
		private readonly Mock<IExpertTestService> _mockExpertTestService;
		private readonly Mock<ITestResultService> _mockTestResultService;
		private readonly TestResultsController _controller;

		public TestResultsControllerTests()
		{
			_mockServicesProvider = new Mock<IServicesProvider>();
			_mockExpertTestService = new Mock<IExpertTestService>();
			_mockTestResultService = new Mock<ITestResultService>();

			_mockServicesProvider.Setup(sp => sp.ExpertTestService).Returns(_mockExpertTestService.Object);
			_mockServicesProvider.Setup(sp => sp.TestResultService).Returns(_mockTestResultService.Object);

			_controller = new TestResultsController(_mockServicesProvider.Object);
		}

		private void SetupUserClaims(int userId, string role = "User")
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
				new Claim(ClaimTypes.Role, role)
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};
		}

		#region RateExpertResponse Tests

		[Fact]
		public async Task RateExpertResponse_WithValidData_ShouldReturnOkResult()
		{
		// Arrange
		var userId = 1;
		var model = new RateExpertResponseModel
		{
			TestResponseId = 1,
			Rating = 5
		};			SetupUserClaims(userId);

			_mockExpertTestService
				.Setup(ets => ets.RateExpertResponseAsync(model, userId))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _controller.RateExpertResponse(model);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			_mockExpertTestService.Verify(ets => ets.RateExpertResponseAsync(model, userId), Times.Once);
		}

		[Fact]
		public async Task RateExpertResponse_WithInvalidModelState_ShouldReturnBadRequest()
		{
			// Arrange
			var userId = 1;
			var model = new RateExpertResponseModel();

			SetupUserClaims(userId);
			_controller.ModelState.AddModelError("Rating", "Required");

			// Act
			var result = await _controller.RateExpertResponse(model);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task RateExpertResponse_WithInvalidUserId_ShouldReturnUnauthorized()
		{
		// Arrange
		var model = new RateExpertResponseModel { TestResponseId = 1, Rating = 5 };			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, "invalid"),
				new Claim(ClaimTypes.Role, "User")
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			// Act
			var result = await _controller.RateExpertResponse(model);

			// Assert
			result.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Fact]
		public async Task RateExpertResponse_WithUnauthorizedException_ShouldReturnUnauthorized()
		{
		// Arrange
		var userId = 1;
		var model = new RateExpertResponseModel { TestResponseId = 1, Rating = 5 };

		SetupUserClaims(userId);

		_mockExpertTestService
			.Setup(ets => ets.RateExpertResponseAsync(model, userId))
			.ThrowsAsync(new UnauthorizedAccessException("Not authorized"));			// Act
			var result = await _controller.RateExpertResponse(model);

			// Assert
			result.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Fact]
		public async Task RateExpertResponse_WithInvalidOperationException_ShouldReturnBadRequest()
		{
		// Arrange
		var userId = 1;
		var model = new RateExpertResponseModel { TestResponseId = 1, Rating = 5 };

		SetupUserClaims(userId);

		_mockExpertTestService
			.Setup(ets => ets.RateExpertResponseAsync(model, userId))
			.ThrowsAsync(new InvalidOperationException("Already rated"));			// Act
			var result = await _controller.RateExpertResponse(model);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		#endregion

		#region RequestReview Tests

		[Fact]
		public async Task RequestReview_WithValidTestRequestId_ShouldReturnOkResult()
		{
			// Arrange
			var userId = 1;
			var testRequestId = 1;

			SetupUserClaims(userId);

			_mockTestResultService
				.Setup(trs => trs.RequestReviewAsync(testRequestId, userId))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _controller.RequestReview(testRequestId);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			_mockTestResultService.Verify(trs => trs.RequestReviewAsync(testRequestId, userId), Times.Once);
		}

		[Fact]
		public async Task RequestReview_WithInvalidUserId_ShouldReturnUnauthorized()
		{
			// Arrange
			var testRequestId = 1;

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, "abc"),
				new Claim(ClaimTypes.Role, "User")
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			// Act
			var result = await _controller.RequestReview(testRequestId);

			// Assert
			result.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Fact]
		public async Task RequestReview_WithInvalidOperationException_ShouldReturnBadRequest()
		{
			// Arrange
			var userId = 1;
			var testRequestId = 1;

			SetupUserClaims(userId);

			_mockTestResultService
				.Setup(trs => trs.RequestReviewAsync(testRequestId, userId))
				.ThrowsAsync(new InvalidOperationException("Test not found"));

			// Act
			var result = await _controller.RequestReview(testRequestId);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task RequestReview_WithGeneralException_ShouldReturnBadRequest()
		{
			// Arrange
			var userId = 1;
			var testRequestId = 1;

			SetupUserClaims(userId);

			_mockTestResultService
				.Setup(trs => trs.RequestReviewAsync(testRequestId, userId))
				.ThrowsAsync(new Exception("Unexpected error"));

			// Act
			var result = await _controller.RequestReview(testRequestId);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		#endregion
	}
}
