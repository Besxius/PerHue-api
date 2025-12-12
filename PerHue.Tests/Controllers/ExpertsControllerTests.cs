using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerHue.Api.Controllers;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.Expert;
using System.Security.Claims;
using Xunit;

namespace PerHue.Tests.Controllers
{
	public class ExpertsControllerTests
	{
		private readonly Mock<IServicesProvider> _mockServicesProvider;
		private readonly Mock<IExpertService> _mockExpertService;
		private readonly ExpertsController _controller;

		public ExpertsControllerTests()
		{
			_mockServicesProvider = new Mock<IServicesProvider>();
			_mockExpertService = new Mock<IExpertService>();

			_mockServicesProvider.Setup(sp => sp.ExpertService).Returns(_mockExpertService.Object);

			_controller = new ExpertsController(_mockServicesProvider.Object);
		}

		private void SetupUserClaims(int userId, string role = "Expert")
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

		[Fact]
		public async Task GetExpertsByRating_ShouldReturnOrderedExperts()
		{
			// Arrange
			var experts = new List<ExpertModel>
			{
				new ExpertModel { Id = 1, Rating = 4.8m },
				new ExpertModel { Id = 2, Rating = 4.5m }
			};

			_mockExpertService
				.Setup(es => es.GetAllByRatingDescendingAsync())
				.ReturnsAsync(experts);

			// Act
			var result = await _controller.GetExpertsByRating();

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var returnedExperts = okResult!.Value as IEnumerable<ExpertModel>;
			returnedExperts.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetAllExperts_ShouldReturnAllExperts()
		{
			// Arrange
			var experts = new List<ExpertModel>
			{
				new ExpertModel { Id = 1 },
				new ExpertModel { Id = 2 },
				new ExpertModel { Id = 3 }
			};

			_mockExpertService
				.Setup(es => es.GetAllAsync())
				.ReturnsAsync(experts);

			// Act
			var result = await _controller.GetAllExperts();

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var returnedExperts = okResult!.Value as IEnumerable<ExpertModel>;
			returnedExperts.Should().HaveCount(3);
		}

		[Fact]
		public async Task GetExpertById_WithValidId_ShouldReturnExpert()
		{
			// Arrange
			var expertId = 1;
			var expert = new ExpertModel { Id = expertId, Rating = 4.7m };

			_mockExpertService
				.Setup(es => es.GetByIdAsync(expertId))
				.ReturnsAsync(expert);

			// Act
			var result = await _controller.GetExpertById(expertId);

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var returnedExpert = okResult!.Value as ExpertModel;
			returnedExpert.Should().NotBeNull();
			returnedExpert!.Id.Should().Be(expertId);
		}

		[Fact]
		public async Task GetExpertById_WithNonExistentId_ShouldReturnNotFound()
		{
			// Arrange
			var expertId = 999;

			_mockExpertService
				.Setup(es => es.GetByIdAsync(expertId))
				.ReturnsAsync((ExpertModel)null!);

			// Act
			var result = await _controller.GetExpertById(expertId);

			// Assert
			result.Result.Should().BeOfType<NotFoundResult>();
		}

		[Fact]
		public async Task GetMySalary_WithValidExpert_ShouldReturnSalary()
		{
			// Arrange
			var expertId = 1;
			SetupUserClaims(expertId, "Expert");

			var salaryModel = new ExpertSalaryModel
			{
				TotalSalary = 5000000,
				TotalRequests = 20
			};

			_mockExpertService
				.Setup(es => es.CalculateSalaryAsync(expertId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
				.ReturnsAsync(salaryModel);

			// Act
			var result = await _controller.GetMySalary(null, null);

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var returnedSalary = okResult!.Value as ExpertSalaryModel;
			returnedSalary.Should().NotBeNull();
			returnedSalary!.TotalSalary.Should().Be(5000000);
		}

		[Fact]
		public async Task GetMySalary_WithInvalidUserId_ShouldReturnUnauthorized()
		{
			// Arrange
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, "invalid"),
				new Claim(ClaimTypes.Role, "Expert")
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = claimsPrincipal }
			};

			// Act
			var result = await _controller.GetMySalary(null, null);

			// Assert
			result.Result.Should().BeOfType<UnauthorizedObjectResult>();
		}

		[Fact]
		public async Task GetMySalary_WithException_ShouldReturnInternalServerError()
		{
			// Arrange
			var expertId = 1;
			SetupUserClaims(expertId, "Expert");

			_mockExpertService
				.Setup(es => es.CalculateSalaryAsync(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
				.ThrowsAsync(new Exception("Calculation error"));

			// Act
			var result = await _controller.GetMySalary(null, null);

			// Assert
			result.Result.Should().BeOfType<ObjectResult>();
			var objectResult = result.Result as ObjectResult;
			objectResult!.StatusCode.Should().Be(500);
		}
	}
}
