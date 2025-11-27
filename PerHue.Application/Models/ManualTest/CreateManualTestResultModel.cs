using Microsoft.AspNetCore.Http;

namespace PerHue.Application.Models.ManualTest
{
	public class CreateManualTestResultModel
	{
		public int UserId { get; set; }

		public IFormFile? Picture { get; set; } = default;
		public List<string> SelectedColors { get; set; }
		public string? ColorType { get; set; } = string.Empty;
	}
}
