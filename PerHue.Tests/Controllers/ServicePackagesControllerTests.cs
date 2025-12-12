using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerHue.Api.Controllers;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.ServicePackage;
using System.Security.Claims;
using Xunit;

namespace PerHue.Tests.Controllers
{
	public class ServicePackagesControllerTests
	{
		private readonly Mock<IServicesProvider> _mockServicesProvider;
		private readonly Mock<IServicePackageService> _mockServicePackageService;
		private readonly ServicePackagesController _controller;

		public ServicePackagesControllerTests()
		{
			_mockServicesProvider = new Mock<IServicesProvider>();
			_mockServicePackageService = new Mock<IServicePackageService>();

			_mockServicesProvider.Setup(sp => sp.ServicePackageService).Returns(_mockServicePackageService.Object);

			_controller = new ServicePackagesController(_mockServicesProvider.Object);
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
		public async Task Gets_WithAuthenticatedUser_ShouldReturnAllPackages()
		{
			// Arrange
			var userId = 1;
			SetupUserClaims(userId);

			var packages = new List<ServicePackageModel>
			{
				new ServicePackageModel { Id = 1, Price = 100000, Uses = 10 },
				new ServicePackageModel { Id = 2, Price = 200000, Uses = 25 }
			};

			_mockServicePackageService
				.Setup(sps => sps.GetAllAsync())
				.ReturnsAsync(packages);

			// Act
			var result = await _controller.Gets();

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task Get_WithValidId_ShouldReturnPackage()
		{
			// Arrange
			var packageId = 1;
			var package = new ServicePackageModel { Id = packageId, Price = 150000, Uses = 15 };

			_mockServicePackageService
				.Setup(sps => sps.GetByIdAsync(packageId))
				.ReturnsAsync(package);

			// Act
			var result = await _controller.Get(packageId);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(packageId);
			result.Price.Should().Be(150000);
		}
	}
}
