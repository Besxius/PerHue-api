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

		//Get All by ColorType (No Paging)
		[HttpGet("by-type/{colorTypeId}/all")]
		public async Task<ActionResult<IEnumerable<CapsulePaletteModel>>> GetByColorTypeAll(int colorTypeId)
		{
			try
			{
				var palettes = await _servicesProvider.CapsulePaletteService.GetByColorTypeIdAsync(colorTypeId);
				return Ok(palettes);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		//Get by ColorType (Paged and Search)
		[HttpGet("by-type/{colorTypeId}")]
		public async Task<ActionResult<PaginatedResult<CapsulePaletteModel>>> GetByColorTypePaged(
			int colorTypeId,
			[FromQuery] int pageIndex = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] string? searchTerm = "")
		{
			try
			{
				var result = await _servicesProvider.CapsulePaletteService.GetByColorTypeIdPagedAsync(colorTypeId, pageIndex, pageSize, searchTerm);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}
