using System.ComponentModel.DataAnnotations;

namespace PerHue.Application.Models.Report
{
	public class CreateReportModel
	{
		[Required(ErrorMessage = "Content is required")]
		[StringLength(2000, ErrorMessage = "Content cannot exceed 2000 characters")]
		public string Content { get; set; } = null!;

		[StringLength(100, ErrorMessage = "Type cannot exceed 100 characters")]
		public string? Type { get; set; }
	}
}
