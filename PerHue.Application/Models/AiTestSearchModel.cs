namespace PerHue.Application.Models
{
	public class AiTestSearchModel
	{
		public int PageIndex { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public int? UserId { get; set; }
		public string? Status { get; set; }
		public string? TypeOfTest { get; set; }
		public string? Fullname { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public AiTestSortBy? SortBy { get; set; } = AiTestSortBy.CreatedDate;
		public SortOrder SortOrder { get; set; } = SortOrder.Descending;
	}

	public enum AiTestSortBy
	{
		CreatedDate = 1,
		Status = 2,
		UserId = 3,
		TestRequestId = 4
	}
}