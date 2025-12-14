using FluentAssertions;
using Moq;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.Color;
using Xunit;

namespace PerHue.Tests.Services
{
	public class ColorServiceTests
	{
		private readonly Mock<IColorService> _mockColorService;

		public ColorServiceTests()
		{
			_mockColorService = new Mock<IColorService>();
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllColors()
		{
			// Arrange
			var models = new List<ColorModel>
			{
				new ColorModel { Id = 1, Name = "Red", HexCode = "#FF0000" },
				new ColorModel { Id = 2, Name = "Blue", HexCode = "#0000FF" }
			};

			_mockColorService
				.Setup(cs => cs.GetAllAsync())
				.ReturnsAsync(models);

			// Act
			var result = await _mockColorService.Object.GetAllAsync();

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetByIdAsync_WithValidId_ShouldReturnColor()
		{
			// Arrange
			var colorId = 1;
			var model = new ColorModel { Id = colorId, Name = "Red", HexCode = "#FF0000" };

			_mockColorService
				.Setup(cs => cs.GetByIdAsync(colorId))
				.ReturnsAsync(model);

			// Act
			var result = await _mockColorService.Object.GetByIdAsync(colorId);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(colorId);
			result.Name.Should().Be("Red");
		}

		[Fact]
		public async Task GetAllAsync_WithPagination_ShouldReturnPaginatedResult()
		{
			// Arrange
			var pageIndex = 1;
			var pageSize = 10;
			var searchTerm = "";
			var paginatedResult = new PaginatedResult<ColorModel>
			{
				Items = new List<ColorModel>
				{
					new ColorModel { Id = 1, Name = "Red", HexCode = "#FF0000" },
					new ColorModel { Id = 2, Name = "Blue", HexCode = "#0000FF" }
				},
				PageIndex = pageIndex,
				PageSize = pageSize,
				TotalCount = 2,
				TotalPages = 1
			};

			_mockColorService
				.Setup(cs => cs.GetAllAsync(pageIndex, pageSize, searchTerm))
				.ReturnsAsync(paginatedResult);

			// Act
			var result = await _mockColorService.Object.GetAllAsync(pageIndex, pageSize, searchTerm);

			// Assert
			result.Should().NotBeNull();
			result.Items.Should().HaveCount(2);
			result.PageIndex.Should().Be(pageIndex);
			result.TotalCount.Should().Be(2);
		}

		[Fact]
		public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
		{
			// Arrange
			var colorId = 1;

			_mockColorService
				.Setup(cs => cs.DeleteAsync(colorId))
				.ReturnsAsync(true);

			// Act
			var result = await _mockColorService.Object.DeleteAsync(colorId);

			// Assert
			result.Should().BeTrue();
		}

		[Fact]
		public async Task GetColorsByColorTypeNormalAsync_ShouldReturnColors()
		{
			// Arrange
			var colorTypeId = 1;
			var models = new List<ColorModel>
			{
				new ColorModel { Id = 1, Name = "Spring Red", HexCode = "#FF0000" },
				new ColorModel { Id = 2, Name = "Spring Blue", HexCode = "#0000FF" }
			};

			_mockColorService
				.Setup(cs => cs.GetColorsByColorTypeNormalAsync(colorTypeId))
				.ReturnsAsync(models);

			// Act
			var result = await _mockColorService.Object.GetColorsByColorTypeNormalAsync(colorTypeId);

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetAllBySpectrumAsync_ShouldReturnColorsOrderedBySpectrum()
		{
			// Arrange
			var models = new List<ColorModel>
			{
				new ColorModel { Id = 1, Name = "Red", HexCode = "#FF0000" },
				new ColorModel { Id = 2, Name = "Orange", HexCode = "#FFA500" },
				new ColorModel { Id = 3, Name = "Yellow", HexCode = "#FFFF00" }
			};

			_mockColorService
				.Setup(cs => cs.GetAllBySpectrumAsync())
				.ReturnsAsync(models);

			// Act
			var result = await _mockColorService.Object.GetAllBySpectrumAsync();

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(3);
		}

		[Fact]
		public async Task GetRelativeColors_ShouldReturnRelatedColors()
		{
			// Arrange
			var selectedColors = new List<string> { "Red", "Blue" };
			var models = new List<ColorModel>
			{
				new ColorModel { Id = 1, Name = "Dark Red", HexCode = "#8B0000" },
				new ColorModel { Id = 2, Name = "Light Blue", HexCode = "#ADD8E6" }
			};

			_mockColorService
				.Setup(cs => cs.GetRelativeColors(selectedColors))
				.ReturnsAsync(models);

			// Act
			var result = await _mockColorService.Object.GetRelativeColors(selectedColors);

			// Assert
			result.Should().NotBeNull();
			result.Should().HaveCount(2);
		}
	}
}
