using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Report;
using System.Security.Claims;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ReportsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public ReportsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		/// <summary>
		/// Create a new report
		/// </summary>
		/// <param name="model">Report creation model</param>
		/// <returns>Created report</returns>
		[HttpPost]
		public async Task<ActionResult<ServiceResponse<ReportModel>>> CreateReport([FromBody] CreateReportModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ServiceResponse<ReportModel>.BadRequest("Invalid request data"));
				}

				var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
				var report = await _servicesProvider.ReportService.CreateReportAsync(userId, model); 
				return Ok(ServiceResponse<ReportModel>.Ok(report, "Report created successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ServiceResponse<ReportModel>.Error($"An error occurred while creating the report: {ex.Message}"));
			}
		}

		/// <summary>
		/// Get all reports created by the current user
		/// </summary>
		/// <returns>List of user's reports</returns>
		[HttpGet("my-reports")]
		public async Task<ActionResult<ServiceResponse<IEnumerable<ReportModel>>>> GetMyReports()
		{
			try
			{
				var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
				var reports = await _servicesProvider.ReportService.GetReportsByUserIdAsync(userId); 
				return Ok(ServiceResponse<IEnumerable<ReportModel>>.Ok(reports, "Reports retrieved successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ServiceResponse<IEnumerable<ReportModel>>.Error($"An error occurred while retrieving reports: {ex.Message}"));
			}
		}

		/// <summary>
		/// Get a specific report by ID (only if it belongs to the current user)
		/// </summary>
		/// <param name="id">Report ID</param>
		/// <returns>Report details</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<ServiceResponse<ReportModel>>> GetReport(int id)
		{
			try
			{
				var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
				var report = await _servicesProvider.ReportService.GetByIdAsync(id); if (report == null)
				{
					return NotFound(ServiceResponse<ReportModel>.NotFound("Report not found"));
				}

				// Check if the report belongs to the current user
				if (report.UserAccountId != userId)
				{
					return Forbid();
				}

				return Ok(ServiceResponse<ReportModel>.Ok(report, "Report retrieved successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ServiceResponse<ReportModel>.Error($"An error occurred while retrieving the report: {ex.Message}"));
			}
		}

		/// <summary>
		/// Delete a report (only if it belongs to the current user)
		/// </summary>
		/// <param name="id">Report ID</param>
		/// <returns>Success or error response</returns>
		[HttpDelete("{id}")]
		public async Task<ActionResult<ServiceResponse>> DeleteReport(int id)
		{
			try
			{
				var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
				var report = await _servicesProvider.ReportService.GetByIdAsync(id); if (report == null)
				{
					return NotFound(ServiceResponse.NotFound("Report not found"));
				}

				// Check if the report belongs to the current user
				if (report.UserAccountId != userId)
				{
					return Forbid();
				}

				var result = await _servicesProvider.ReportService.DeleteAsync(id);
				if (!result)
				{
					return BadRequest(ServiceResponse.BadRequest("Failed to delete report"));
				}

				return Ok(ServiceResponse.Ok(null, "Report deleted successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ServiceResponse.Error($"An error occurred while deleting the report: {ex.Message}"));
			}
		}
	}
}
