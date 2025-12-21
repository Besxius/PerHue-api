namespace PerHue.Application.Models.Expert
{
	public class ExpertSearchModel
	{
		public int PageIndex { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string? SearchTerm { get; set; }
		public string? SearchBy { get; set; } // email, username, nickname, specialization
		public string? SortBy { get; set; } = "rating"; // rating, yearsOfExperience, id
		public string? SortOrder { get; set; } = "desc"; // asc, desc
	}
}
