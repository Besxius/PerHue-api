using FluentAssertions;
using Moq;
using PerHue.Application.IServices;
using Xunit;

namespace PerHue.Tests.Services
{
	public class OtpServiceTests
	{
		private readonly Mock<IOtpService> _mockOtpService;

		public OtpServiceTests()
		{
			_mockOtpService = new Mock<IOtpService>();
		}

		[Fact]
		public void GenerateOTP_ShouldReturnNumericString()
		{
			// Arrange
			var expectedOtp = "1234";
			_mockOtpService
				.Setup(os => os.GenerateOTP(4))
				.Returns(expectedOtp);

			// Act
			var otp = _mockOtpService.Object.GenerateOTP(4);

			// Assert
			otp.Should().NotBeNullOrEmpty();
			otp.Should().HaveLength(4);
		}

		[Fact]
		public void GenerateOTP_WithDifferentLength_ShouldReturnCorrectLength()
		{
			// Arrange
			_mockOtpService
				.Setup(os => os.GenerateOTP(6))
				.Returns("123456");

			_mockOtpService
				.Setup(os => os.GenerateOTP(8))
				.Returns("12345678");

			// Act
			var otp6 = _mockOtpService.Object.GenerateOTP(6);
			var otp8 = _mockOtpService.Object.GenerateOTP(8);

			// Assert
			otp6.Should().HaveLength(6);
			otp8.Should().HaveLength(8);
		}

		[Fact]
		public async Task SendOtpToEmailAsync_WithValidEmail_ShouldReturnTrue()
		{
			// Arrange
			var email = "test@example.com";

			_mockOtpService
				.Setup(os => os.SendOtpToEmailAsync(email))
				.ReturnsAsync(true);

			// Act
			var result = await _mockOtpService.Object.SendOtpToEmailAsync(email);

			// Assert
			result.Should().BeTrue();
		}

		[Fact]
		public async Task SendOtpToEmailAsync_WhenEmailFails_ShouldReturnFalse()
		{
			// Arrange
			var email = "test@example.com";

			_mockOtpService
				.Setup(os => os.SendOtpToEmailAsync(email))
				.ReturnsAsync(false);

			// Act
			var result = await _mockOtpService.Object.SendOtpToEmailAsync(email);

			// Assert
			result.Should().BeFalse();
		}

		[Fact]
		public void VerifyOtp_WithValidOtp_ShouldReturnTrue()
		{
			// Arrange
			var email = "test@example.com";
			var otp = "1234";

			_mockOtpService
				.Setup(os => os.VerifyOtp(email, otp))
				.Returns(true);

			// Act
			var result = _mockOtpService.Object.VerifyOtp(email, otp);

			// Assert
			result.Should().BeTrue();
		}

		[Fact]
		public void VerifyOtp_WithInvalidOtp_ShouldReturnFalse()
		{
			// Arrange
			var email = "test@example.com";
			var wrongOtp = "9999";

			_mockOtpService
				.Setup(os => os.VerifyOtp(email, wrongOtp))
				.Returns(false);

			// Act
			var result = _mockOtpService.Object.VerifyOtp(email, wrongOtp);

			// Assert
			result.Should().BeFalse();
		}

		[Fact]
		public void VerifyOtp_WithExpiredOtp_ShouldReturnFalse()
		{
			// Arrange
			var email = "test@example.com";
			var otp = "1234";

			_mockOtpService
				.Setup(os => os.VerifyOtp(email, otp))
				.Returns(false);

			// Act
			var result = _mockOtpService.Object.VerifyOtp(email, otp);

			// Assert
			result.Should().BeFalse();
		}
	}
}
