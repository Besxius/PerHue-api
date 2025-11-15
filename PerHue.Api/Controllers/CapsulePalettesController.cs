using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.CapsulePalette;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CapsulePalettesController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		public CapsulePalettesController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}
		[HttpGet]
		public async Task<PaginatedResult<CapsulePaletteModel>> Get(int? pageIndex = 1, int? pageSize = 15, string? searchTerm = "")
		{
			return await _servicesProvider.CapsulePaletteService.GetAllAsync(pageIndex ?? 1, pageSize ?? 15, searchTerm);
		}

		[HttpGet("{id}")]
		public async Task<CapsulePaletteModel> Get(int id)
		{
			return await _servicesProvider.CapsulePaletteService.GetByIdAsync(id);
		}
	}
}
