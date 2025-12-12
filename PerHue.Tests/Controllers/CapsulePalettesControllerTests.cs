using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PerHue.Api.Controllers;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.CapsulePalette;
using Xunit;

namespace PerHue.Tests.Controllers
{
	public class CapsulePalettesControllerTests
	{
		private readonly Mock<IServicesProvider> _mockServicesProvider;
		private readonly Mock<ICapsulePaletteService> _mockCapsulePaletteService;
		private readonly CapsulePalettesController _controller;

		public CapsulePalettesControllerTests()
		{
			_mockServicesProvider = new Mock<IServicesProvider>();
			_mockCapsulePaletteService = new Mock<ICapsulePaletteService>();

			_mockServicesProvider.Setup(sp => sp.CapsulePaletteService).Returns(_mockCapsulePaletteService.Object);

			_controller = new CapsulePalettesController(_mockServicesProvider.Object);
		}

		#region Get Tests

		[Fact]
		public async Task Get_WithPagination_ShouldReturnPaginatedResult()
		{
			// Arrange
			var pageIndex = 1;
			var pageSize = 15;
			var searchTerm = "casual";
			var expectedResult = new PaginatedResult<CapsulePaletteModel>
			{
				Items = new List<CapsulePaletteModel>
				{
					new CapsulePaletteModel { Id = 1, ColorTypeId = 1 },
					new CapsulePaletteModel { Id = 2, ColorTypeId = 1 }
				},
				TotalCount = 2
			};

			_mockCapsulePaletteService
				.Setup(cps => cps.GetAllAsync(pageIndex, pageSize, searchTerm))
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
			var expectedResult = new PaginatedResult<CapsulePaletteModel>
			{
				Items = new List<CapsulePaletteModel>(),
				TotalCount = 0
			};

			_mockCapsulePaletteService
				.Setup(cps => cps.GetAllAsync(1, 15, ""))
				.ReturnsAsync(expectedResult);

			// Act
			var result = await _controller.Get();

			// Assert
			result.Should().NotBeNull();
			_mockCapsulePaletteService.Verify(cps => cps.GetAllAsync(1, 15, ""), Times.Once);
		}

		#endregion

		#region GetById Tests

		[Fact]
		public async Task GetById_WithValidId_ShouldReturnCapsulePalette()
		{
		// Arrange
		var paletteId = 1;
		var expectedPalette = new CapsulePaletteModel { Id = paletteId, ColorTypeId = 2 };

		_mockCapsulePaletteService
			.Setup(cps => cps.GetByIdAsync(paletteId))
			.ReturnsAsync(expectedPalette);

		// Act
		var result = await _controller.Get(paletteId);

		// Assert
		result.Should().NotBeNull();
		result.Id.Should().Be(paletteId);
		result.ColorTypeId.Should().Be(2);
		}

		#endregion

		#region GetByColorTypeAll Tests

		[Fact]
		public async Task GetByColorTypeAll_WithValidColorType_ShouldReturnOkResult()
		{
			// Arrange
			var colorTypeId = 1;
			var palettes = new List<CapsulePaletteModel>
			{
				new CapsulePaletteModel { Id = 1, ColorTypeId = colorTypeId },
				new CapsulePaletteModel { Id = 2, ColorTypeId = colorTypeId }
			};

			_mockCapsulePaletteService
				.Setup(cps => cps.GetByColorTypeIdAsync(colorTypeId))
				.ReturnsAsync(palettes);

			// Act
			var result = await _controller.GetByColorTypeAll(colorTypeId);

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var returnedPalettes = okResult!.Value as IEnumerable<CapsulePaletteModel>;
			returnedPalettes.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetByColorTypeAll_WithException_ShouldReturnInternalServerError()
		{
			// Arrange
			_mockCapsulePaletteService
				.Setup(cps => cps.GetByColorTypeIdAsync(It.IsAny<int>()))
				.ThrowsAsync(new Exception("Database error"));

			// Act
			var result = await _controller.GetByColorTypeAll(1);

			// Assert
			result.Result.Should().BeOfType<ObjectResult>();
			var objectResult = result.Result as ObjectResult;
			objectResult!.StatusCode.Should().Be(500);
		}

		#endregion

		#region GetByColorTypePaged Tests

		[Fact]
		public async Task GetByColorTypePaged_WithValidParameters_ShouldReturnOkResult()
		{
			// Arrange
			var colorTypeId = 1;
			var pageIndex = 1;
			var pageSize = 10;
			var searchTerm = "formal";
			var expectedResult = new PaginatedResult<CapsulePaletteModel>
			{
				Items = new List<CapsulePaletteModel>
				{
					new CapsulePaletteModel { Id = 1, ColorTypeId = colorTypeId }
				},
				TotalCount = 1
			};

			_mockCapsulePaletteService
				.Setup(cps => cps.GetByColorTypeIdPagedAsync(colorTypeId, pageIndex, pageSize, searchTerm))
				.ReturnsAsync(expectedResult);

			// Act
			var result = await _controller.GetByColorTypePaged(colorTypeId, pageIndex, pageSize, searchTerm);

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			var okResult = result.Result as OkObjectResult;
			var paginatedResult = okResult!.Value as PaginatedResult<CapsulePaletteModel>;
			paginatedResult.Should().NotBeNull();
			paginatedResult!.Items.Should().HaveCount(1);
		}

		[Fact]
		public async Task GetByColorTypePaged_WithDefaultParameters_ShouldUseDefaults()
		{
			// Arrange
			var colorTypeId = 1;
			var expectedResult = new PaginatedResult<CapsulePaletteModel>
			{
				Items = new List<CapsulePaletteModel>(),
				TotalCount = 0
			};

			_mockCapsulePaletteService
				.Setup(cps => cps.GetByColorTypeIdPagedAsync(colorTypeId, 1, 10, ""))
				.ReturnsAsync(expectedResult);

			// Act
			var result = await _controller.GetByColorTypePaged(colorTypeId);

			// Assert
			result.Result.Should().BeOfType<OkObjectResult>();
			_mockCapsulePaletteService.Verify(
				cps => cps.GetByColorTypeIdPagedAsync(colorTypeId, 1, 10, ""),
				Times.Once);
		}

		[Fact]
		public async Task GetByColorTypePaged_WithException_ShouldReturnInternalServerError()
		{
			// Arrange
			_mockCapsulePaletteService
				.Setup(cps => cps.GetByColorTypeIdPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
				.ThrowsAsync(new Exception("Service error"));

			// Act
			var result = await _controller.GetByColorTypePaged(1);

			// Assert
			result.Result.Should().BeOfType<ObjectResult>();
			var objectResult = result.Result as ObjectResult;
			objectResult!.StatusCode.Should().Be(500);
		}

		#endregion
	}
}
