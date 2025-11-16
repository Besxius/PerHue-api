namespace PerHue.Application.Models
{
	public class AiTestResultModel
	{
		public string ColorType { get; set; } = string.Empty;
		public int ColorTypeId { get; set; }
		public List<string> SuggestedColor { get; set; }
		public List<string> AvoidedColor { get; set; }
	}
}
