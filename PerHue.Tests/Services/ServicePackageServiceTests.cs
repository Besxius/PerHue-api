using FluentAssertions;
using Moq;
using PerHue.Application.IServices;
using PerHue.Application.Models.ServicePackage;
using Xunit;

namespace PerHue.Tests.Services
{
	public class ServicePackageServiceTests
	{
		private readonly Mock<IServicePackageService> _mockServicePackageService;

		public ServicePackageServiceTests()
		{
			_mockServicePackageService = new Mock<IServicePackageService>();
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllServicePackages()
		{
			// Arrange
			var packages = new List<ServicePackageModel>
			{
				new ServicePackageModel { Id = 1, Name = "Basic", Price = 50000, Uses = 10 },
				new ServicePackageModel { Id = 2, Name = "Premium", Price = 100000, Uses = 25 }
			};

			_mockServicePackageService
				.Setup(sps => sps.GetAllAsync())
				.ReturnsAsync(packages);

			// Act
			var result = await _mockServicePackageService.Object.GetAllAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetByIdAsync_WithValidId_ShouldReturnServicePackage()
		{
			// Arrange
			var packageId = 1;
			var package = new ServicePackageModel
			{
				Id = packageId,
				Name = "Basic",
				Price = 50000,
				Uses = 10
			};

			_mockServicePackageService
				.Setup(sps => sps.GetByIdAsync(packageId))
				.ReturnsAsync(package);

			// Act
			var result = await _mockServicePackageService.Object.GetByIdAsync(packageId);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(packageId);
			result.Name.Should().Be("Basic");
		}

		[Fact]
		public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
		{
			// Arrange
			var packageId = 1;

			_mockServicePackageService
				.Setup(sps => sps.DeleteAsync(packageId))
				.ReturnsAsync(true);

			// Act
			var result = await _mockServicePackageService.Object.DeleteAsync(packageId);

			// Assert
			result.Should().BeTrue();
		}
	}
}
