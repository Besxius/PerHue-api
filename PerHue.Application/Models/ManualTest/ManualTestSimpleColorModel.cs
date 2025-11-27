using Microsoft.AspNetCore.Http;

namespace PerHue.Application.Models.ManualTest
{
	public class ManualTestSimpleColorModel
	{
		public IFormFile? Picture { get; set; } = default;
		public List<string> SelectedColors { get; set; }
	}
}
