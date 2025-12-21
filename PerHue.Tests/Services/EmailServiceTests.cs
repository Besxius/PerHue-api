using FluentAssertions;
using Moq;
using PerHue.Application.IServices;
using Xunit;

namespace PerHue.Tests.Services
{
	public class EmailServiceTests
	{
		private readonly Mock<IEmailService> _mockEmailService;

		public EmailServiceTests()
		{
			_mockEmailService = new Mock<IEmailService>();
		}

		[Fact]
		public async Task SendEmailAsync_WithValidData_ShouldComplete()
		{
			// Arrange
			var toEmail = "test@example.com";
			var subject = "Test Subject";
			var body = "Test Body";

			_mockEmailService
				.Setup(es => es.SendEmailAsync(toEmail, subject, body))
				.Returns(Task.CompletedTask);

			// Act
			await _mockEmailService.Object.SendEmailAsync(toEmail, subject, body);

			// Assert
			_mockEmailService.Verify(es => es.SendEmailAsync(toEmail, subject, body), Times.Once);
		}

		[Fact]
		public async Task SendEmailAsync_ShouldBeCalledWithCorrectParameters()
		{
			// Arrange
			var toEmail = "user@example.com";
			var subject = "Welcome";
			var body = "Welcome to PerHue";

			_mockEmailService
				.Setup(es => es.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(Task.CompletedTask);

			// Act
			await _mockEmailService.Object.SendEmailAsync(toEmail, subject, body);

			// Assert
			_mockEmailService.Verify(es => es.SendEmailAsync(
				It.Is<string>(e => e == toEmail),
				It.Is<string>(s => s == subject),
				It.Is<string>(b => b == body)
			), Times.Once);
		}
	}
}
