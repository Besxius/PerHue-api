namespace PerHue.Application.Models.Expert
{
	public class ExpertSalarySearchModel
	{
		public int PageIndex { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string? SearchTerm { get; set; }
		public string? SortBy { get; set; } = "totalSalary"; // totalSalary, totalRequests, averageRating, expertId
		public string? SortOrder { get; set; } = "desc"; // asc, desc
	}
}
