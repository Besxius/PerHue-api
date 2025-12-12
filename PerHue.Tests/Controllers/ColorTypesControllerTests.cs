using FluentAssertions;
using Moq;
using PerHue.Api.Controllers;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.ColorType;
using Xunit;

namespace PerHue.Tests.Controllers
{
	public class ColorTypesControllerTests
	{
		private readonly Mock<IServicesProvider> _mockServicesProvider;
		private readonly Mock<IColorTypeService> _mockColorTypeService;
		private readonly ColorTypesController _controller;

		public ColorTypesControllerTests()
		{
			_mockServicesProvider = new Mock<IServicesProvider>();
			_mockColorTypeService = new Mock<IColorTypeService>();

			_mockServicesProvider.Setup(sp => sp.ColorTypeService).Returns(_mockColorTypeService.Object);

			_controller = new ColorTypesController(_mockServicesProvider.Object);
		}

		#region Get All Tests

		[Fact]
		public async Task GetAll_ShouldReturnAllColorTypes()
		{
			// Arrange
			var colorTypes = new List<ColorTypeModel>
			{
				new ColorTypeModel { Id = 1, Name = "Spring" },
				new ColorTypeModel { Id = 2, Name = "Summer" },
				new ColorTypeModel { Id = 3, Name = "Autumn" },
				new ColorTypeModel { Id = 4, Name = "Winter" }
			};

			_mockColorTypeService
				.Setup(cts => cts.GetAllAsync())
				.ReturnsAsync(colorTypes);

			// Act
			var result = await _controller.Get();

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(4);
			result.Should().Contain(ct => ct.Name == "Spring");
			result.Should().Contain(ct => ct.Name == "Winter");
		}

		[Fact]
		public async Task GetAll_WithEmptyResult_ShouldReturnEmptyList()
		{
			// Arrange
			_mockColorTypeService
				.Setup(cts => cts.GetAllAsync())
				.ReturnsAsync(new List<ColorTypeModel>());

			// Act
			var result = await _controller.Get();

			// Assert
			result.Should().NotBeNull();
			result.Should().BeEmpty();
		}

		#endregion

		#region Get By Id Tests

		[Fact]
		public async Task GetById_WithValidId_ShouldReturnColorType()
		{
			// Arrange
			var colorTypeId = 1;
			var expectedColorType = new ColorTypeModel { Id = colorTypeId, Name = "Spring" };

			_mockColorTypeService
				.Setup(cts => cts.GetByIdAsync(colorTypeId))
				.ReturnsAsync(expectedColorType);

			// Act
			var result = await _controller.Get(colorTypeId);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(colorTypeId);
			result.Name.Should().Be("Spring");
		}

		[Fact]
		public async Task GetById_WithNonExistentId_ShouldReturnNull()
		{
			// Arrange
			var colorTypeId = 999;

			_mockColorTypeService
				.Setup(cts => cts.GetByIdAsync(colorTypeId))
				.ReturnsAsync((ColorTypeModel)null!);

			// Act
			var result = await _controller.Get(colorTypeId);

			// Assert
			result.Should().BeNull();
		}

		#endregion
	}
}
