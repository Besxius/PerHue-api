namespace PerHue.Application.Models.CapsulePalette
{
	public class AdminCapsulePaletteSearchModel
	{
		public int PageIndex { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string? SearchTerm { get; set; }
		public string? SearchBy { get; set; } = "name"; // name, colorType, id, season
		public string? SortBy { get; set; } = "createdDate"; // createdDate, updatedDate, name, id
		public string? SortOrder { get; set; } = "desc"; // asc, desc
		public int? ColorTypeId { get; set; }
	}
}