using System.ComponentModel.DataAnnotations;

namespace PerHue.Application.Models.Report
{
	public class UpdateReportModel
	{
		[StringLength(2000, ErrorMessage = "Content cannot exceed 2000 characters")]
		public string? Content { get; set; }

		[StringLength(100, ErrorMessage = "Type cannot exceed 100 characters")]
		public string? Type { get; set; }

		[StringLength(500, ErrorMessage = "Notice cannot exceed 500 characters")]
		public string? Notice { get; set; }

		[StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
		public string? Status { get; set; }
	}
}
