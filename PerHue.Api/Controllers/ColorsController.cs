using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Color;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ColorsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		public ColorsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}
		// GET: api/<ColorsController>
		[HttpGet]
		public async Task<PaginatedResult<ColorModel>> Get(int? pageIndex = 1, int? pageSize = 30, string? searchTerm = "")
		{
			return await _servicesProvider.ColorService.GetAllAsync(pageIndex ?? 1, pageSize ?? 30, searchTerm);
		}

		// GET api/<ColorsController>/5
		[HttpGet("{id}")]
		public async Task<ColorModel> Get(int id)
		{
			return await _servicesProvider.ColorService.GetByIdAsync(id);
		}

		[HttpGet("by-spectrum")]
		public async Task<ActionResult<IEnumerable<ColorModel>>> GetColorsBySpectrum()
		{
			try
			{
				var colors = await _servicesProvider.ColorService.GetAllBySpectrumAsync();
				return Ok(colors);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("by-spectrum-paging")]
		public async Task<ActionResult<PaginatedResult<ColorModel>>> GetColorsBySpectrumPaged(
			[FromQuery] int pageIndex = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] string? searchTerm = "")
		{
			try
			{
				var result = await _servicesProvider.ColorService.GetAllBySpectrumPagedAsync(pageIndex, pageSize, searchTerm);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
		// Normal API (No Paging)
		[HttpGet("by-type/{colorTypeId}/all")]
		public async Task<ActionResult<IEnumerable<ColorModel>>> GetColorsByColorTypeAll(int colorTypeId)
		{
			try
			{
				var colors = await _servicesProvider.ColorService.GetColorsByColorTypeNormalAsync(colorTypeId);
				return Ok(colors);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// Paging API
		[HttpGet("by-type/{colorTypeId}")]
		public async Task<ActionResult<PaginatedResult<ColorModel>>> GetColorsByColorTypePaging(
			int colorTypeId,
			[FromQuery] int pageIndex = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] string? searchTerm = "")
		{
			try
			{
				var result = await _servicesProvider.ColorService.GetColorsByColorTypePagingAsync(colorTypeId, pageIndex, pageSize, searchTerm);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}
