using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerHue.Api.Controllers;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Payment.An;
using System.Security.Claims;
using Xunit;

namespace PerHue.Tests.Controllers
{
	public class PaymentsControllerTests
	{
		private readonly Mock<IServicesProvider> _mockServicesProvider;
		private readonly Mock<IPaymentService> _mockPaymentService;
		private readonly PaymentsController _controller;

		public PaymentsControllerTests()
		{
			_mockServicesProvider = new Mock<IServicesProvider>();
			_mockPaymentService = new Mock<IPaymentService>();

			_mockServicesProvider.Setup(sp => sp.PaymentService).Returns(_mockPaymentService.Object);

			_controller = new PaymentsController(_mockServicesProvider.Object);
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

		#region GetPaymentHistory Tests

		[Fact]
		public async Task GetPaymentHistory_WithValidUser_ShouldReturnOkResult()
		{
			// Arrange
			var userId = 1;
			var pageIndex = 1;
			var pageSize = 10;
			var expectedResult = new PaginatedResultV2<PaymentDetailModel>
			{
				List = new List<PaymentDetailModel>
				{
					new PaymentDetailModel { Id = 1, UserId = userId, Amount = 100000 },
					new PaymentDetailModel { Id = 2, UserId = userId, Amount = 200000 }
				},
				Total = 2
			};

			SetupUserClaims(userId);

			_mockPaymentService
				.Setup(ps => ps.GetPaymentHistoryByUserIdAsync(userId, pageIndex, pageSize))
				.ReturnsAsync(expectedResult);

			// Act
			var result = await _controller.GetPaymentHistory(pageIndex, pageSize);

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
		}

		[Fact]
		public async Task GetPaymentHistory_WithException_ShouldReturnInternalServerError()
		{
			// Arrange
			var userId = 1;
			SetupUserClaims(userId);

			_mockPaymentService
				.Setup(ps => ps.GetPaymentHistoryByUserIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Database error"));

			// Act
			var result = await _controller.GetPaymentHistory();

			// Assert
			result.Result.Should().BeOfType<ObjectResult>();
			var objectResult = result.Result as ObjectResult;
			objectResult!.StatusCode.Should().Be(500);
		}

		#endregion

		#region GetAllPayments Tests

		[Fact]
		public async Task GetAllPayments_WithValidUser_ShouldReturnOkResult()
		{
			// Arrange
			var userId = 1;
			var payments = new List<PaymentDetailModel>
			{
				new PaymentDetailModel { Id = 1, UserId = userId, Amount = 100000 },
				new PaymentDetailModel { Id = 2, UserId = userId, Amount = 200000 }
			};

			SetupUserClaims(userId);

			_mockPaymentService
				.Setup(ps => ps.GetAllPaymentsByUserIdAsync(userId))
				.ReturnsAsync(payments);

			// Act
			var result = await _controller.GetAllPayments();

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var returnedPayments = okResult!.Value as List<PaymentDetailModel>;
			returnedPayments.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetAllPayments_WithException_ShouldReturnInternalServerError()
		{
			// Arrange
			var userId = 1;
			SetupUserClaims(userId);

			_mockPaymentService
				.Setup(ps => ps.GetAllPaymentsByUserIdAsync(It.IsAny<int>()))
				.ThrowsAsync(new Exception("Service error"));

			// Act
			var result = await _controller.GetAllPayments();

			// Assert
			result.Result.Should().BeOfType<ObjectResult>();
			var objectResult = result.Result as ObjectResult;
			objectResult!.StatusCode.Should().Be(500);
		}

		#endregion

		#region GetPaymentDetail Tests

		[Fact]
		public async Task GetPaymentDetail_WithValidPayment_ShouldReturnOkResult()
		{
			// Arrange
			var userId = 1;
			var paymentId = 1;
			var payment = new PaymentDetailModel { Id = paymentId, UserId = userId, Amount = 150000 };

			SetupUserClaims(userId);

			_mockPaymentService
				.Setup(ps => ps.GetPaymentDetailByIdAsync(paymentId, userId))
				.ReturnsAsync(payment);

			// Act
			var result = await _controller.GetPaymentDetail(paymentId);

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
		}

		[Fact]
		public async Task GetPaymentDetail_WithNonExistentPayment_ShouldReturnNotFound()
		{
			// Arrange
			var userId = 1;
			var paymentId = 999;

			SetupUserClaims(userId);

			_mockPaymentService
				.Setup(ps => ps.GetPaymentDetailByIdAsync(paymentId, userId))
				.ReturnsAsync((PaymentDetailModel)null!);

			// Act
			var result = await _controller.GetPaymentDetail(paymentId);

			// Assert
			result.Result.Should().BeOfType<NotFoundObjectResult>();
		}

		[Fact]
		public async Task GetPaymentDetail_WithException_ShouldReturnInternalServerError()
		{
			// Arrange
			var userId = 1;
			var paymentId = 1;
			SetupUserClaims(userId);

			_mockPaymentService
				.Setup(ps => ps.GetPaymentDetailByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
				.ThrowsAsync(new Exception("Payment error"));

			// Act
			var result = await _controller.GetPaymentDetail(paymentId);

			// Assert
			result.Result.Should().BeOfType<ObjectResult>();
			var objectResult = result.Result as ObjectResult;
			objectResult!.StatusCode.Should().Be(500);
		}

		#endregion
	}
}
