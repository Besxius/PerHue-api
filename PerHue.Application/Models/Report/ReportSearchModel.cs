namespace PerHue.Application.Models.Report
{
	public class ReportSearchModel : BaseSearchModel
	{
		public string? Type { get; set; }

		public string? Status { get; set; }

		public string? SearchKeyword { get; set; }

		public DateTime? FromDate { get; set; }

		public DateTime? ToDate { get; set; }
	}
}
