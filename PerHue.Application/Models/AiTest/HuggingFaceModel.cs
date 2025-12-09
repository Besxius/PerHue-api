using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.Models.AiTest
{
	public class HuggingFaceModel
	{
		// Models
		public class HFVirtualTryOnResponse
		{
			public HFGeneratedImage GeneratedImages { get; set; } = new();
		}

		public class HFGeneratedImage
		{
			public string ImageUrl { get; set; } = string.Empty;        // URL (nếu có)
			public string ImageBase64 { get; set; } = string.Empty;     // Base64 string
			public byte[] ImageBytes { get; set; }                      // Raw bytes
			public string Environment { get; set; } = string.Empty;
			public string ClothingType { get; set; } = string.Empty;
			public string ColorHex { get; set; } = string.Empty;
			public string Prompt { get; set; } = string.Empty;
		}
	}
}
