using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Report;

namespace PerHue.Api.Controllers.Admin
{
	[Route("api/admin/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin")]
	public class ReportsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public ReportsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		/// <summary>
		/// Get paginated list of all reports with search and filter options
		/// </summary>
		/// <param name="searchModel">Search and pagination parameters</param>
		/// <returns>Paginated list of reports</returns>
		[HttpGet]
		public async Task<ActionResult<ServiceResponse<PaginatedResultV2<ReportModel>>>> GetAllReports([FromQuery] ReportSearchModel searchModel)
		{
			try
			{
				var result = await _servicesProvider.ReportService.GetAllReportsAsync(searchModel);
				return Ok(ServiceResponse<PaginatedResultV2<ReportModel>>.Ok(result, "Reports retrieved successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ServiceResponse<PaginatedResultV2<ReportModel>>.Error($"An error occurred while retrieving reports: {ex.Message}"));
			}
		}

		/// <summary>
		/// Get a specific report by ID
		/// </summary>
		/// <param name="id">Report ID</param>
		/// <returns>Report details</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<ServiceResponse<ReportModel>>> GetReport(int id)
		{
			try
			{
				var report = await _servicesProvider.ReportService.GetByIdAsync(id);
				if (report == null)
				{
					return NotFound(ServiceResponse<ReportModel>.NotFound("Report not found"));
				}

				return Ok(ServiceResponse<ReportModel>.Ok(report, "Report retrieved successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ServiceResponse<ReportModel>.Error($"An error occurred while retrieving the report: {ex.Message}"));
			}
		}

		/// <summary>
		/// Update a report
		/// </summary>
		/// <param name="id">Report ID</param>
		/// <param name="model">Report update model</param>
		/// <returns>Success or error response</returns>
		[HttpPut("{id}")]
		public async Task<ActionResult<ServiceResponse>> UpdateReport(int id, [FromBody] UpdateReportModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ServiceResponse.BadRequest("Invalid request data"));
				}

				var result = await _servicesProvider.ReportService.UpdateReportAsync(id, model);
				if (!result)
				{
					return NotFound(ServiceResponse.NotFound("Report not found"));
				}

				return Ok(ServiceResponse.Ok(null, "Report updated successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ServiceResponse.Error($"An error occurred while updating the report: {ex.Message}"));
			}
		}

		/// <summary>
		/// Update report status
		/// </summary>
		/// <param name="id">Report ID</param>
		/// <param name="status">New status</param>
		/// <param name="notice">Optional notice/comment</param>
		/// <returns>Success or error response</returns>
		[HttpPatch("{id}/status")]
		public async Task<ActionResult<ServiceResponse>> UpdateReportStatus(
			int id,
			[FromBody] ReportStatusUpdateModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ServiceResponse.BadRequest("Invalid request data"));
				}

				var result = await _servicesProvider.ReportService.UpdateReportStatusAsync(id, model.Status, model.Notice);
				if (!result)
				{
					return NotFound(ServiceResponse.NotFound("Report not found"));
				}

				return Ok(ServiceResponse.Ok(null, $"Report status updated to '{model.Status}' successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ServiceResponse.Error($"An error occurred while updating report status: {ex.Message}"));
			}
		}

		/// <summary>
		/// Delete a report
		/// </summary>
		/// <param name="id">Report ID</param>
		/// <returns>Success or error response</returns>
		[HttpDelete("{id}")]
		public async Task<ActionResult<ServiceResponse>> DeleteReport(int id)
		{
			try
			{
				var result = await _servicesProvider.ReportService.DeleteAsync(id);
				if (!result)
				{
					return NotFound(ServiceResponse.NotFound("Report not found"));
				}

				return Ok(ServiceResponse.Ok(null, "Report deleted successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ServiceResponse.Error($"An error occurred while deleting the report: {ex.Message}"));
			}
		}

		/// <summary>
		/// Get reports by user ID
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <returns>List of reports from the specified user</returns>
		[HttpGet("by-user/{userId}")]
		public async Task<ActionResult<ServiceResponse<IEnumerable<ReportModel>>>> GetReportsByUser(int userId)
		{
			try
			{
				var reports = await _servicesProvider.ReportService.GetReportsByUserIdAsync(userId);
				return Ok(ServiceResponse<IEnumerable<ReportModel>>.Ok(reports, "Reports retrieved successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ServiceResponse<IEnumerable<ReportModel>>.Error($"An error occurred while retrieving reports: {ex.Message}"));
			}
		}
	}
}
