using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerHue.Api.Controllers;
using PerHue.Application.IServices;
using PerHue.Application.Models.VerifyInformation;
using System.Security.Claims;
using Xunit;

namespace PerHue.Tests.Controllers
{
	public class VerificationControllerTests
	{
		private readonly Mock<IVerificationService> _mockVerificationService;
		private readonly VerificationController _controller;

		public VerificationControllerTests()
		{
			_mockVerificationService = new Mock<IVerificationService>();
			_controller = new VerificationController(_mockVerificationService.Object);
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

		[Fact]
		public async Task SubmitRequest_WithValidModel_ShouldReturnOk()
		{
			// Arrange
			var userId = 1;
			SetupUserClaims(userId);

			var model = new VerifyRequestModel();

			_mockVerificationService
				.Setup(vs => vs.SubmitVerificationAsync(userId, model))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _controller.SubmitRequest(model);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			_mockVerificationService.Verify(vs => vs.SubmitVerificationAsync(userId, model), Times.Once);
		}

		[Fact]
		public async Task SubmitRequest_WithInvalidOperation_ShouldReturnBadRequest()
		{
			// Arrange
			var userId = 1;
			SetupUserClaims(userId);

			var model = new VerifyRequestModel();

			_mockVerificationService
				.Setup(vs => vs.SubmitVerificationAsync(userId, model))
				.ThrowsAsync(new InvalidOperationException("Already submitted"));

			// Act
			var result = await _controller.SubmitRequest(model);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Fact]
		public async Task SubmitRequestHasPhotoModel_WithValidModel_ShouldReturnOk()
		{
			// Arrange
			var userId = 1;
			SetupUserClaims(userId);

			var model = new VerifyInformationModel();

			_mockVerificationService
				.Setup(vs => vs.Version2SubmitVerificationAsync(userId, model))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _controller.SubmitRequestHasPhotoModel(model);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
		}

		[Fact]
		public async Task CheckPendingRequest_WithPendingRequest_ShouldReturnTrue()
		{
			// Arrange
			var userId = 1;
			SetupUserClaims(userId);

			_mockVerificationService
				.Setup(vs => vs.HasPendingVerificationAsync(userId))
				.ReturnsAsync(true);

			// Act
			var result = await _controller.CheckPendingRequest();

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			var okResult = result as OkObjectResult;
			okResult.Should().NotBeNull();
		}

		[Fact]
		public async Task CheckPendingRequest_WithException_ShouldReturnStatusCode()
		{
			// Arrange
			var userId = 1;
			SetupUserClaims(userId);

			_mockVerificationService
				.Setup(vs => vs.HasPendingVerificationAsync(userId))
				.ThrowsAsync(new Exception("Database error"));

			// Act
			var result = await _controller.CheckPendingRequest();

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
		}
	}
}
