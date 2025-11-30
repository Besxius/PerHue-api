namespace PerHue.Application.Models.VerifyInformation
{
	public class VerificationSearchModel
	{
		public int PageIndex { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string? SearchTerm { get; set; }
		public string? SearchBy { get; set; } // email, nickname, specialization
		public string? SortBy { get; set; } = "id"; // id, email, nickname, specialization, yearsOfExperience
		public string? SortOrder { get; set; } = "desc"; // asc, desc
	}
}