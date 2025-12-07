namespace PerHue.Application.Models.Color
{
	public class AdminColorSearchModel
	{
		public int PageIndex { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string? SearchTerm { get; set; }
		public string? SearchBy { get; set; }
		public string? SortBy { get; set; } = "createdDate"; // createdDate, updatedDate, name, id
		public string? SortOrder { get; set; } = "desc"; // asc, desc
	}
}