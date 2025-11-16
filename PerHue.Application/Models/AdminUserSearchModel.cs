namespace PerHue.Application.Models
{
	public class AdminUserSearchModel
	{
		public int PageIndex { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string? Email { get; set; }
		public string? Fullname { get; set; }
		public string? Phone { get; set; }
		public bool? IsActive { get; set; }
		public string? RoleName { get; set; }
		public UserSortBy? SortBy { get; set; } = UserSortBy.CreatedDate;
		public SortOrder SortOrder { get; set; } = SortOrder.Descending;
	}

	public enum UserSortBy
	{
		CreatedDate = 1,
		UpdatedDate = 2,
		Username = 3,
		Email = 4,
		AccountId = 5,
		Fullname = 6
	}

	public enum SortOrder
	{
		Ascending = 1,
		Descending = 2
	}
}