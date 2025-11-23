namespace PerHue.Application.Models
{
    /// <summary>
    /// Base search model for pagination and sorting
    /// </summary>
    public class BaseSearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public SortOrder SortOrder { get; set; } = SortOrder.Descending;
    }
}