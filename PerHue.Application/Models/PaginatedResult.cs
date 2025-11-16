namespace PerHue.Application.Models
{
	public class PaginatedResult<T>
	{
		public IEnumerable<T> Items { get; set; }
		public int TotalCount { get; set; }
		public int PageSize { get; set; }
		public int PageIndex { get; set; }
		public int TotalPages { get; set; }
	}

	public class PaginatedResultV2<T>
	{
		public IEnumerable<T> List { get; set; }
		public int Total { get; set; }
		public int Current { get; set; }
	}
}
