using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Color;

namespace PerHue.Api.Controllers.Admin
{
	[Route("api/admin/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin")]
	public class ColorManagementController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public ColorManagementController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		/// <summary>
		/// Get all colors with paging, search and sorting
		/// </summary>
		[HttpGet("colors")]
		public async Task<ActionResult<PaginatedResultV2<AdminColorModel>>> GetColors([FromQuery] AdminColorSearchModel searchModel)
		{
			try
			{
				var result = await _servicesProvider.AdminColorService.GetAllAsync(searchModel);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Get color by id
		/// </summary>
		[HttpGet("colors/{id}")]
		public async Task<ActionResult<AdminColorModel>> GetColor(int id)
		{
			try
			{
				var color = await _servicesProvider.AdminColorService.GetByIdAsync(id);
				if (color == null)
				{
					return NotFound(new { message = "Color not found" });
				}
				return Ok(color);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Create new color
		/// </summary>
		[HttpPost("colors")]
		public async Task<ActionResult<AdminColorModel>> CreateColor([FromBody] AdminColorCreateModel model)
		{
			try
			{
				var color = await _servicesProvider.AdminColorService.CreateAsync(model);
				return CreatedAtAction(nameof(GetColor), new { id = color.Id }, color);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Update color
		/// </summary>
		[HttpPut("colors/{id}")]
		public async Task<ActionResult<AdminColorModel>> UpdateColor(int id, [FromBody] AdminColorUpdateModel model)
		{
			try
			{
				var color = await _servicesProvider.AdminColorService.UpdateAsync(model);
				return Ok(color);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Delete color (hard delete)
		/// </summary>
		[HttpDelete("colors/{id}")]
		public async Task<ActionResult> DeleteColor(int id)
		{
			try
			{
				var result = await _servicesProvider.AdminColorService.DeleteAsync(id);
				if (!result)
				{
					return NotFound(new { message = "Color not found" });
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
