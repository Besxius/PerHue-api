namespace PerHue.Application.Models.TestRequest
{
	public class AiTestResultModel
	{
		public string ColorType { get; set; } = string.Empty;
		public List<string> SuggestedColor { get; set; }
		public List<string> AvoidedColor { get; set; }
	}
}
