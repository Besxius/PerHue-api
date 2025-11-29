using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.CapsulePalette;

namespace PerHue.Api.Controllers.Admin
{
	[Route("api/admin/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin")]
	public class CapsulePaletteManagementController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public CapsulePaletteManagementController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		/// <summary>
		/// Get all capsule palettes with paging, search and sorting
		/// </summary>
		[HttpGet("palettes")]
		public async Task<ActionResult<PaginatedResultV2<AdminCapsulePaletteModel>>> GetPalettes([FromQuery] AdminCapsulePaletteSearchModel searchModel)
		{
			try
			{
				var result = await _servicesProvider.AdminCapsulePaletteService.GetAllAsync(searchModel);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Get capsule palette by id
		/// </summary>
		[HttpGet("palettes/{id}")]
		public async Task<ActionResult<AdminCapsulePaletteModel>> GetPalette(int id)
		{
			try
			{
				var palette = await _servicesProvider.AdminCapsulePaletteService.GetByIdAsync(id);
				if (palette == null)
				{
					return NotFound(new { message = "Palette not found" });
				}
				return Ok(palette);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Create new capsule palette
		/// </summary>
		[HttpPost("palettes")]
		public async Task<ActionResult<AdminCapsulePaletteModel>> CreatePalette([FromBody] AdminCapsulePaletteCreateModel model)
		{
			try
			{
				var palette = await _servicesProvider.AdminCapsulePaletteService.CreateAsync(model);
				return CreatedAtAction(nameof(GetPalette), new { id = palette.Id }, palette);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Update capsule palette
		/// </summary>
		[HttpPut("palettes/{id}")]
		public async Task<ActionResult<AdminCapsulePaletteModel>> UpdatePalette(int id, [FromBody] AdminCapsulePaletteUpdateModel model)
		{
			try
			{
				var palette = await _servicesProvider.AdminCapsulePaletteService.UpdateAsync(model);
				return Ok(palette);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Delete capsule palette (hard delete)
		/// </summary>
		[HttpDelete("palettes/{id}")]
		public async Task<ActionResult> DeletePalette(int id)
		{
			try
			{
				var result = await _servicesProvider.AdminCapsulePaletteService.DeleteAsync(id);
				if (!result)
				{
					return NotFound(new { message = "Palette not found" });
				}
				return NoContent();
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
