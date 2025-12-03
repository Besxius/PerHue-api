using System.ComponentModel.DataAnnotations;

namespace PerHue.Application.Models.Report
{
	public class ReportStatusUpdateModel
	{
		[Required(ErrorMessage = "Status is required")]
		[StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
		public string Status { get; set; } = null!;

		[StringLength(500, ErrorMessage = "Notice cannot exceed 500 characters")]
		public string? Notice { get; set; }
	}
}
