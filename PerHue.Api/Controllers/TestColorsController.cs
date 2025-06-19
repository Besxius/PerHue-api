using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

		[HttpGet]
		public async Task<NormalTestResultModel> UploadImageTest(int? pageIndex = 1, int? pageSize = 15, string? selectedColor = "")
		{
			var palettes = await _servicesProvider.CapsulePaletteService.GetAllAsync(pageIndex ?? 1, pageSize ?? 15, selectedColor);
			return new NormalTestResultModel
			{
				SelectedColor = selectedColor,
				CapsulePalettes = palettes,
			};
		}
	}
}
