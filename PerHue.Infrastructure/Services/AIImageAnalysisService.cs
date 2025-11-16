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
using PerHue.Application.Models.TestRequest;

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

				// Tạo list các Part objects
				var parts = new List<GenerativeAI.Types.Part>();

				// Thêm text prompt
				parts.Add(new GenerativeAI.Types.Part { Text = prompt });

				// Download và thêm ảnh
				using var httpClient = new HttpClient();

				foreach (var imageUrl in request.ImageUrls)
				{
					var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

					parts.Add(new GenerativeAI.Types.Part
					{
						InlineData = new GenerativeAI.Types.Blob
						{
							MimeType = "image/jpeg",
							Data = Convert.ToBase64String(imageBytes)
						}
					});
				}

				// Gọi Gemini API
				var response = await model.GenerateContentAsync(parts);
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
1. The seasonal color type (Spring Warm, Summer Cool, Autumn Warm, Winter Cool, Soft Neutral,Deep Neutral)
2. The corresponding ColorTypeId (1=Spring Warm, 2=SummSummer Cooler, 3=Autumn Warm, 4=Winter Cool, 5=Soft Neutral, 6=Deep Neutral)
3. A list of 5-7 suggested colors that complement this color type
4. A list of 5-7 colors to avoid

Return ONLY a valid JSON object with this exact structure (no additional text):
{{
  ""ColorType"": ""Spring Warm/Summer Cool/Autumn Warm/Winter Cool/Soft Neutral/Deep Neutral"",
  ""ColorTypeId"": 1-6,
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
