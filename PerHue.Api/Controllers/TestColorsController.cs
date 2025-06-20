using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestColorsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		public TestColorsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		[HttpPost]
		[Route("upload-image-test")]
		public async Task<NormalTestResultModel> UploadImageTest(string colorsListJson)
		{
			var selectedColor = System.Text.Json.JsonSerializer.Deserialize<List<string>>(colorsListJson);
			var palettes = await _servicesProvider.CapsulePaletteService.GetRelativeCapsulePalettes(selectedColor);
			return new NormalTestResultModel
			{
				SelectedColors = selectedColor,
				totalResultCount = palettes.Count(),
				CapsulePalettes = palettes
			};
		}
	}
}
