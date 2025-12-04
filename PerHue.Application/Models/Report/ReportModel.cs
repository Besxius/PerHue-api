namespace PerHue.Application.Models.Report
{
	public class ReportModel
	{
		public int Id { get; set; }

		public string? Content { get; set; }

		public string? Type { get; set; }

		public string Status { get; set; } = null!;

		public string? Notice { get; set; }

		public int UserAccountId { get; set; }

		public string? UserEmail { get; set; }

		public string? Username { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }
	}
}
