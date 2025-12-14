using FluentAssertions;
using Moq;
using PerHue.Application.IServices;
using PerHue.Application.Models.CapsulePalette;
using Xunit;

namespace PerHue.Tests.Services
{
	public class CapsulePaletteServiceTests
	{
		private readonly Mock<ICapsulePaletteService> _mockCapsulePaletteService;

		public CapsulePaletteServiceTests()
		{
			_mockCapsulePaletteService = new Mock<ICapsulePaletteService>();
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllCapsulePalettes()
		{
			// Arrange
			var palettes = new List<CapsulePaletteModel>
			{
				new CapsulePaletteModel { Id = 1, ColorTypeId = 1 },
				new CapsulePaletteModel { Id = 2, ColorTypeId = 2 }
			};

			_mockCapsulePaletteService
				.Setup(cps => cps.GetAllAsync())
				.ReturnsAsync(palettes);

			// Act
			var result = await _mockCapsulePaletteService.Object.GetAllAsync();

			// Assert
			result.Should().HaveCount(2);
		}

		[Fact]
		public async Task GetByIdAsync_WithValidId_ShouldReturnCapsulePalette()
		{
			// Arrange
			var paletteId = 1;
			var palette = new CapsulePaletteModel
			{
				Id = paletteId,
				ColorTypeId = 1
			};

			_mockCapsulePaletteService
				.Setup(cps => cps.GetByIdAsync(paletteId))
				.ReturnsAsync(palette);

			// Act
			var result = await _mockCapsulePaletteService.Object.GetByIdAsync(paletteId);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(paletteId);
		}

		[Fact]
		public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
		{
			// Arrange
			var paletteId = 1;

			_mockCapsulePaletteService
				.Setup(cps => cps.DeleteAsync(paletteId))
				.ReturnsAsync(true);

			// Act
			var result = await _mockCapsulePaletteService.Object.DeleteAsync(paletteId);

			// Assert
			result.Should().BeTrue();
		}
	}
}
