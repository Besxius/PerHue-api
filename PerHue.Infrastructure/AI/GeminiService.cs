using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace PerHue.Infrastructure.AI
{
	internal class GeminiService
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiKey;

		private const string PROMT = "Hi Gemini, I want you to help me analyze Personal Color based on my characteristics. I attached a selfie without makeup, taken in natural light.\r\n\r\nBased on this photos, you can analyze my Personal Color according to the 12 -season system (Cool Winter, Deep Winter, Clear Winter, Cool Summer, Soft Summe, Light Summer, Warm Autumn, Soft Autumn, Deep Autumn, Warm Spring, Light Spring, Clear Spring)?\r\n\r\nGive me the results in a JSON series with the following sample:\r\n{\r\n  \"colorType\": \"string\",\r\n  \"suggestedColor\": [\r\n    \"string\"\r\n  ],\r\n  \"avoidedColor\": [\r\n    \"string\"\r\n  ]\r\n}\r\nIn particular, Colortype is a 12 -season system name, SuggestedColor is a list of recommended color codes, and AvoidedColor is a list of color codes to avoid.";

		public GeminiService(HttpClient httpClient, IConfiguration config)
		{
			_httpClient = httpClient;
			_apiKey = config["Gemini:ApiKey"]; // đọc từ appsettings.json	
		}

		public async Task<string> GeneratePromptWithImageFromUrl(string imageUrl)
		{
			

			// 1. Download ảnh
			var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);

			// 2. Lấy Content-Type
			string mimeType = await GetMimeTypeFromHttpHeader(imageUrl);

			// 3. Convert base64
			string base64Image = Convert.ToBase64String(imageBytes);

			// 4. Gửi Gemini
			var requestBody = new {
				contents = new[] {
					new {
						parts = new object[] {
							new { text = PROMT },
							new { inline_data = 
								new {
									mime_type = mimeType,
									data = base64Image
								}
							}
						}
					}
				}
			};

			var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";
			//var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={_apiKey}";
			var response = await _httpClient.PostAsJsonAsync(url, requestBody);
			response.EnsureSuccessStatusCode();

			var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
			return result?.candidates?[0]?.content?.parts?[0]?.text ?? "No response.";
		}

		private async Task<string> GetMimeTypeFromHttpHeader(string url)
		{
			//// Rất đơn giản, chỉ nhìn phần đuôi file
			//if (url.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) return "image/png";
			//if (url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || url.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)) return "image/jpeg";
			//return "image/png"; // mặc định

			using var request = new HttpRequestMessage(HttpMethod.Head, url);
			using var response = await _httpClient.SendAsync(request);

			if (response.Content.Headers.ContentType != null)
			{
				return response.Content.Headers.ContentType.MediaType;
			}

			// fallback
			return "image/jpeg";
		}
	}

	// Đảm bảo đã có lớp để deserialize
	public class GeminiResponse
	{
		public Candidate[] candidates { get; set; }
	}

	public class Candidate
	{
		public Content content { get; set; }
	}

	public class Content
	{
		public Part[] parts { get; set; }
	}

	public class Part
	{
		public string text { get; set; }
	}
}
