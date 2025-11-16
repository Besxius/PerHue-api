using GenerativeAI;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Infrastructure.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Services
{
	public class AiImageAnalysisService : IAIImageAnalysisService
	{
		private readonly string _apiKey;
		private readonly ILogger<AiImageAnalysisService> _logger;

		public AiImageAnalysisService(IConfiguration configuration, ILogger<AiImageAnalysisService> logger)
		{
			_apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini API Key not found");
			_logger = logger;
		}

		public async Task<AiTestResultModel> AnalyzeColorTypeAsync(AiTestModel.GeminiAnalysisRequest request)
		{
			try
			{
				var client = new GoogleAi(_apiKey);
				var model = client.CreateGenerativeModel("gemini-2.5-flash");

				// Tạo prompt chi tiết cho Gemini
				var prompt = BuildAnalysisPrompt(request);

				// Download và chuyển đổi ảnh thành base64
				var imageParts = new List<object>();
				using var httpClient = new HttpClient();

				foreach (var imageUrl in request.ImageUrls)
				{
					var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
					var base64Image = Convert.ToBase64String(imageBytes);
					imageParts.Add(new
					{
						inline_data = new
						{
							mime_type = "image/jpeg",
							data = base64Image
						}
					});
				}

				// Gọi Gemini API với ảnh và prompt
				var contents = new List<object> { prompt };
				contents.AddRange(imageParts);

				var response = await model.GenerateContentAsync((IEnumerable<GenerativeAI.Types.Part>)contents.ToArray());
				var resultText = response.Text;

				// Parse kết quả JSON từ Gemini
				var result = ParseGeminiResponse(resultText);

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error analyzing images with Gemini");
				throw new Exception("Failed to analyze images with AI", ex);
			}
		}

		private string BuildAnalysisPrompt(AiTestModel.GeminiAnalysisRequest request)
		{
			return $@"
You are a professional color analysis expert. Analyze the provided images and determine the person's seasonal color type.

User Information:
- Hair Color: {request.HairColor ?? "Not specified"}
- Eye Color: {request.EyesColor ?? "Not specified"}
- Lips Color: {request.LipsColor ?? "Not specified"}
- Skin Color: {request.SkinColor ?? "Not specified"}

Based on the images and information provided, determine:
1. The seasonal color type (Spring, Summer, Autumn, or Winter)
2. The corresponding ColorTypeId (1=Spring, 2=Summer, 3=Autumn, 4=Winter)
3. A list of 5-7 suggested colors that complement this color type
4. A list of 5-7 colors to avoid

Return ONLY a valid JSON object with this exact structure (no additional text):
{{
  ""ColorType"": ""Spring/Summer/Autumn/Winter"",
  ""ColorTypeId"": 1-4,
  ""SuggestedColor"": [""color1"", ""color2"", ...],
  ""AvoidedColor"": [""color1"", ""color2"", ...]
}}

Use specific color names like 'Coral Pink', 'Warm Beige', 'Deep Emerald', etc.
";
		}


	private AiTestResultModel ParseGeminiResponse(string responseText)
		{
			try
			{
				// Loại bỏ markdown code blocks nếu có
				responseText = responseText.Trim();
				if (responseText.StartsWith("```json"))
				{
					responseText = responseText.Substring(7);
				}
				if (responseText.StartsWith("```"))
				{
					responseText = responseText.Substring(3);
				}
				if (responseText.EndsWith("```"))
				{
					responseText = responseText.Substring(0, responseText.Length - 3);
				}

				responseText = responseText.Trim();

				var result = System.Text.Json.JsonSerializer.Deserialize<AiTestResultModel>(responseText, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});

				if (result == null)
				{
					throw new Exception("Failed to deserialize Gemini response");
				}

				return result;
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to parse Gemini response: {responseText}", ex);
			}
		}
	}
}
