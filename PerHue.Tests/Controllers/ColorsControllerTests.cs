using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerHue.Api.Controllers;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Color;
using Xunit;

namespace PerHue.Tests.Controllers
{
	public class ColorsControllerTests
	{
		private readonly Mock<IServicesProvider> _mockServicesProvider;
		private readonly Mock<IColorService> _mockColorService;
		private readonly ColorsController _controller;

		public ColorsControllerTests()
		{
			_mockServicesProvider = new Mock<IServicesProvider>();
			_mockColorService = new Mock<IColorService>();

			_mockServicesProvider.Setup(sp => sp.ColorService).Returns(_mockColorService.Object);

			_controller = new ColorsController(_mockServicesProvider.Object);
		}

		#region Get Tests

		[Fact]
		public async Task Get_WithPagination_ShouldReturnPaginatedResult()
		{
			// Arrange
			var pageIndex = 1;
			var pageSize = 30;
			var searchTerm = "red";
			var expectedResult = new PaginatedResult<ColorModel>
			{
				Items = new List<ColorModel>
				{
					new ColorModel { Id = 1, Name = "Red" },
					new ColorModel { Id = 2, Name = "Dark Red" }
				},
				TotalCount = 2
			};

			_mockColorService
				.Setup(cs => cs.GetAllAsync(pageIndex, pageSize, searchTerm))
				.ReturnsAsync(expectedResult);

			// Act
			var result = await _controller.Get(pageIndex, pageSize, searchTerm);

			// Assert
			result.Should().NotBeNull();
			result.Items.Should().HaveCount(2);
			result.TotalCount.Should().Be(2);
		}

		[Fact]
		public async Task Get_WithDefaultParameters_ShouldUseDefaults()
		{
			// Arrange
			var expectedResult = new PaginatedResult<ColorModel>
			{
				Items = new List<ColorModel>(),
				TotalCount = 0
			};

			_mockColorService
				.Setup(cs => cs.GetAllAsync(1, 30, ""))
				.ReturnsAsync(expectedResult);

			// Act
			var result = await _controller.Get();

			// Assert
			result.Should().NotBeNull();
			_mockColorService.Verify(cs => cs.GetAllAsync(1, 30, ""), Times.Once);
		}

		#endregion

		#region GetById Tests

		[Fact]
		public async Task GetById_WithValidId_ShouldReturnColor()
		{
			// Arrange
			var colorId = 1;
			var expectedColor = new ColorModel { Id = colorId, Name = "Blue" };

			_mockColorService
				.Setup(cs => cs.GetByIdAsync(colorId))
				.ReturnsAsync(expectedColor);

			// Act
			var result = await _controller.Get(colorId);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(colorId);
			result.Name.Should().Be("Blue");
		}

		#endregion

		#region GetColorsBySpectrum Tests

		[Fact]
		public async Task GetColorsBySpectrum_WithSuccess_ShouldReturnOkResult()
		{
			// Arrange
			var colors = new List<ColorModel>
			{
				new ColorModel { Id = 1, Name = "Red" },
				new ColorModel { Id = 2, Name = "Blue" }
			};

			_mockColorService
				.Setup(cs => cs.GetAllBySpectrumAsync())
				.ReturnsAsync(colors);

			// Act
			var result = await _controller.GetColorsBySpectrum();

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var returnedColors = okResult!.Value as IEnumerable<ColorModel>;
			returnedColors.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetColorsBySpectrum_WithException_ShouldReturnInternalServerError()
		{
			// Arrange
			_mockColorService
				.Setup(cs => cs.GetAllBySpectrumAsync())
				.ThrowsAsync(new Exception("Database error"));

			// Act
			var result = await _controller.GetColorsBySpectrum();

			// Assert
			result.Result.Should().BeOfType<ObjectResult>();
			var objectResult = result.Result as ObjectResult;
			objectResult!.StatusCode.Should().Be(500);
		}

		#endregion

		#region GetColorsBySpectrumPaged Tests

		[Fact]
		public async Task GetColorsBySpectrumPaged_WithValidParameters_ShouldReturnOkResult()
		{
			// Arrange
			var pageIndex = 1;
			var pageSize = 10;
			var searchTerm = "blue";
			var expectedResult = new PaginatedResult<ColorModel>
			{
				Items = new List<ColorModel> { new ColorModel { Id = 1, Name = "Blue" } },
				TotalCount = 1
			};

			_mockColorService
				.Setup(cs => cs.GetAllBySpectrumPagedAsync(pageIndex, pageSize, searchTerm))
				.ReturnsAsync(expectedResult);

			// Act
			var result = await _controller.GetColorsBySpectrumPaged(pageIndex, pageSize, searchTerm);

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var paginatedResult = okResult!.Value as PaginatedResult<ColorModel>;
			paginatedResult.Should().NotBeNull();
			paginatedResult!.Items.Should().HaveCount(1);
		}

		[Fact]
		public async Task GetColorsBySpectrumPaged_WithException_ShouldReturnInternalServerError()
		{
			// Arrange
			_mockColorService
				.Setup(cs => cs.GetAllBySpectrumPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
				.ThrowsAsync(new Exception("Error"));

			// Act
			var result = await _controller.GetColorsBySpectrumPaged();

			// Assert
			result.Result.Should().BeOfType<ObjectResult>();
			var objectResult = result.Result as ObjectResult;
			objectResult!.StatusCode.Should().Be(500);
		}

		#endregion

		#region GetColorsByColorTypeAll Tests

		[Fact]
		public async Task GetColorsByColorTypeAll_WithValidColorType_ShouldReturnOkResult()
		{
		// Arrange
		var colorTypeId = 1;
		var colors = new List<ColorModel>
		{
			new ColorModel { Id = 1, Name = "Spring Red", HexCode = "#FF0000" }
		};			_mockColorService
				.Setup(cs => cs.GetColorsByColorTypeNormalAsync(colorTypeId))
				.ReturnsAsync(colors);

			// Act
			var result = await _controller.GetColorsByColorTypeAll(colorTypeId);

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var returnedColors = okResult!.Value as IEnumerable<ColorModel>;
			returnedColors.Should().HaveCount(1);
		}

		[Fact]
		public async Task GetColorsByColorTypeAll_WithException_ShouldReturnInternalServerError()
		{
			// Arrange
			_mockColorService
				.Setup(cs => cs.GetColorsByColorTypeNormalAsync(It.IsAny<int>()))
				.ThrowsAsync(new Exception("Service error"));

			// Act
			var result = await _controller.GetColorsByColorTypeAll(1);

			// Assert
			result.Result.Should().BeOfType<ObjectResult>();
			var objectResult = result.Result as ObjectResult;
			objectResult!.StatusCode.Should().Be(500);
		}

		#endregion

		#region GetColorsByColorTypePaging Tests

		[Fact]
		public async Task GetColorsByColorTypePaging_WithValidParameters_ShouldReturnOkResult()
		{
			// Arrange
			var colorTypeId = 1;
			var pageIndex = 1;
			var pageSize = 10;
			var searchTerm = "warm";
			var expectedResult = new PaginatedResult<ColorModel>
			{
				Items = new List<ColorModel> { new ColorModel { Id = 1, Name = "Warm Color", HexCode = "#FFA500" } },
				TotalCount = 1
			};

			_mockColorService
				.Setup(cs => cs.GetColorsByColorTypePagingAsync(colorTypeId, pageIndex, pageSize, searchTerm))
				.ReturnsAsync(expectedResult);

			// Act
			var result = await _controller.GetColorsByColorTypePaging(colorTypeId, pageIndex, pageSize, searchTerm);

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var paginatedResult = okResult!.Value as PaginatedResult<ColorModel>;
			paginatedResult.Should().NotBeNull();
		}

		[Fact]
		public async Task GetColorsByColorTypePaging_WithException_ShouldReturnInternalServerError()
		{
			// Arrange
			_mockColorService
				.Setup(cs => cs.GetColorsByColorTypePagingAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
				.ThrowsAsync(new Exception("Paging error"));

			// Act
			var result = await _controller.GetColorsByColorTypePaging(1);

			// Assert
			result.Result.Should().BeOfType<ObjectResult>();
			var objectResult = result.Result as ObjectResult;
			objectResult!.StatusCode.Should().Be(500);
		}

		#endregion
	}
}
