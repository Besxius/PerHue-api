using FluentAssertions;
using Moq;
using PerHue.Application.IServices;
using PerHue.Application.Models.Payment;
using Xunit;

namespace PerHue.Tests.Services
{
	public class PaymentServiceTests
	{
		private readonly Mock<IPaymentService> _mockPaymentService;

		public PaymentServiceTests()
		{
			_mockPaymentService = new Mock<IPaymentService>();
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllPayments()
		{
			// Arrange
			var payments = new List<PaymentModel>
			{
				new PaymentModel { Amount = 100000, UserId = 1 },
				new PaymentModel { Amount = 200000, UserId = 2 }
			};

			_mockPaymentService
				.Setup(ps => ps.GetAllAsync())
				.ReturnsAsync(payments);

			// Act
			var result = await _mockPaymentService.Object.GetAllAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetByIdAsync_WithValidId_ShouldReturnPayment()
		{
			// Arrange
			var paymentId = 1;
			var payment = new PaymentModel
			{
				Amount = 100000,
				UserId = 1
			};

			_mockPaymentService
				.Setup(ps => ps.GetByIdAsync(paymentId))
				.ReturnsAsync(payment);

			// Act
			var result = await _mockPaymentService.Object.GetByIdAsync(paymentId);

			// Assert
			result.Should().NotBeNull();
			result.Amount.Should().Be(100000);
		}
	}
}
