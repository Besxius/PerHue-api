using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Expert;
using PerHue.Infrastructure.Utils;

namespace PerHue.Api.Controllers.Admin
{
	[Route("api/admin/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin, Moderator")]
	public class ExpertManagementController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		private readonly IDateTimeService _dateTimeService;

		public ExpertManagementController(IServicesProvider servicesProvider, IDateTimeService dateTimeService)
		{
			_servicesProvider = servicesProvider;
			_dateTimeService = dateTimeService;
		}

		/// <summary>
		/// Get all experts with paging, search and sorting
		/// </summary>
		[HttpGet("experts")]
		public async Task<ActionResult<PaginatedResultV2<ExpertModel>>> GetExperts([FromQuery] ExpertSearchModel searchModel)
		{
			try
			{
				var result = await _servicesProvider.AdminExpertService.GetAllAsync(searchModel);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Get expert salary report by id
		/// </summary>
		[HttpGet("experts/{id}/salary-report")]
		public async Task<ActionResult<ExpertSalaryModel>> GetExpertSalaryReport(
			int id,
			[FromQuery] DateTime? startDate,
			[FromQuery] DateTime? endDate)
		{
			try
			{
				if (startDate == null && endDate == null)
				{
					var now = _dateTimeService.GetCurrentTime(); ;
					startDate = new DateTime(now.Year, now.Month, 1);
					endDate = startDate.Value.AddMonths(1).AddDays(-1);
				}
				var salaryReport = await _servicesProvider.ExpertService.CalculateSalaryAsync(id, startDate, endDate);
				return Ok(salaryReport);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Get all experts salary report with pagination
		/// </summary>
		[HttpGet("experts/salary-report")]
		public async Task<ActionResult<PaginatedResultV2<ExpertSalaryModel>>> GetAllExpertsSalaryReport(
			[FromQuery] ExpertSalarySearchModel searchModel)
		{
			try
			{
				var result = await _servicesProvider.AdminExpertService.GetAllSalaryReportsAsync(searchModel);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		///// <summary>
		///// Get expert by id
		///// </summary>
		//[HttpGet("experts/{id}")]
		//public async Task<ActionResult<ExpertModel>> GetExpert(int id)
		//{
		//	try
		//	{
		//		var expert = await _servicesProvider.ExpertService.GetByIdAsync(id);
		//		if (expert == null)
		//		{
		//			return NotFound(new { message = "Expert not found" });
		//		}
		//		return Ok(expert);
		//	}
		//	catch (Exception ex)
		//	{
		//		return BadRequest(new { message = ex.Message });
		//	}
		//}

		///// <summary>
		///// Update expert
		///// </summary>
		//[HttpPut("experts/{id}")]
		//public async Task<ActionResult> UpdateExpert(int id, [FromBody] UpdateExpertModel model)
		//{
		//	try
		//	{
		//		var result = await _servicesProvider.ExpertService.UpdateAsync(id, model);
		//		if (!result)
		//		{
		//			return NotFound(new { message = "Expert not found" });
		//		}
		//		return Ok(new { message = "Expert updated successfully" });
		//	}
		//	catch (Exception ex)
		//	{
		//		return BadRequest(new { message = ex.Message });
		//	}
		//}

		///// <summary>
		///// Delete expert
		///// </summary>
		//[HttpDelete("experts/{id}")]
		//public async Task<ActionResult> DeleteExpert(int id)
		//{
		//	try
		//	{
		//		var result = await _servicesProvider.ExpertService.DeleteAsync(id);
		//		if (!result)
		//		{
		//			return NotFound(new { message = "Expert not found" });
		//		}
		//		return NoContent();
		//	}
		//	catch (Exception ex)
		//	{
		//		return BadRequest(new { message = ex.Message });
		//	}
		//}
	}
}
