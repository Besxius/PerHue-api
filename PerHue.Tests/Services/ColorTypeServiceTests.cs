using FluentAssertions;
using Moq;
using PerHue.Application.IServices;
using PerHue.Application.Models.ColorType;
using Xunit;

namespace PerHue.Tests.Services
{
	public class ColorTypeServiceTests
	{
		private readonly Mock<IColorTypeService> _mockColorTypeService;

		public ColorTypeServiceTests()
		{
			_mockColorTypeService = new Mock<IColorTypeService>();
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllColorTypes()
		{
			// Arrange
			var colorTypes = new List<ColorTypeModel>
			{
				new ColorTypeModel { Id = 1, Name = "Spring" },
				new ColorTypeModel { Id = 2, Name = "Summer" }
			};

			_mockColorTypeService
				.Setup(cts => cts.GetAllAsync())
				.ReturnsAsync(colorTypes);

			// Act
			var result = await _mockColorTypeService.Object.GetAllAsync();

			// Assert
			result.Should().HaveCount(2);
			result.Should().BeEquivalentTo(colorTypes);
		}

		[Fact]
		public async Task GetByIdAsync_WithValidId_ShouldReturnColorType()
		{
			// Arrange
			var colorTypeId = 1;
			var colorType = new ColorTypeModel
			{
				Id = colorTypeId,
				Name = "Spring"
			};

			_mockColorTypeService
				.Setup(cts => cts.GetByIdAsync(colorTypeId))
				.ReturnsAsync(colorType);

			// Act
			var result = await _mockColorTypeService.Object.GetByIdAsync(colorTypeId);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(colorTypeId);
			result.Name.Should().Be("Spring");
		}

		[Fact]
		public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
		{
			// Arrange
			var colorTypeId = 1;

			_mockColorTypeService
				.Setup(cts => cts.DeleteAsync(colorTypeId))
				.ReturnsAsync(true);

			// Act
			var result = await _mockColorTypeService.Object.DeleteAsync(colorTypeId);

			// Assert
			result.Should().BeTrue();
		}
	}
}
