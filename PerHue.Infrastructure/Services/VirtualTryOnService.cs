using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PerHue.Application.IServices;
using PerHue.Application.Models.AiTest;
using GenerativeAI;
using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace PerHue.Infrastructure.Services
{
	public class VirtualTryOnService : IVirtualTryOnService
	{
		private readonly string _apiKey;
		private readonly ILogger<VirtualTryOnService> _logger;

		public VirtualTryOnService(IConfiguration configuration, ILogger<VirtualTryOnService> logger)
		{
			_apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini API Key not found");
			_logger = logger;
		}

		public async Task<VirtualTryOnResponse> GenerateVirtualTryOnImagesAsync(VirtualTryOnRequest request)
		{
			try
			{
				var client = new GoogleAi(_apiKey);
				var model = client.CreateGenerativeModel("gemini-2.0-flash-exp");

				var response = new VirtualTryOnResponse();
				using var httpClient = new HttpClient();

				// Tạo ảnh cho mỗi tổ hợp environment và clothing type
				foreach (var environment in request.Environments)
				{
					foreach (var clothingType in request.ClothingTypes)
					{
						// Chọn 2-3 màu ngẫu nhiên từ danh sách gợi ý
						var selectedColors = request.SuggestedColorHexCodes
							.OrderBy(x => Guid.NewGuid())
							.Take(Math.Min(3, request.SuggestedColorHexCodes.Count))
							.ToList();

						foreach (var colorHex in selectedColors)
						{
							var prompt = BuildVirtualTryOnPrompt(environment, clothingType, colorHex);

							try
							{
								var parts = new List<GenerativeAI.Types.Part>
								{
									new GenerativeAI.Types.Part { Text = prompt }
								};

								// Thêm ảnh người dùng
								var userImageBytes = await httpClient.GetByteArrayAsync(request.UserImageUrl);
								parts.Add(new GenerativeAI.Types.Part
								{
									InlineData = new GenerativeAI.Types.Blob
									{
										MimeType = "image/jpeg",
										Data = Convert.ToBase64String(userImageBytes)
									}
								});

								var geminiResponse = await model.GenerateContentAsync(parts);

								// Note: Gemini 2.0 có thể không trả về ảnh trực tiếp
								// Bạn có thể cần sử dụng Imagen hoặc API khác để generate ảnh
								// Đây là placeholder logic
								response.GeneratedImages.Add(new Application.Models.AiTest.GeneratedImage
								{
									ImageUrl = $"generated_{Guid.NewGuid()}.jpg", // Placeholder
									Environment = environment,
									ClothingType = clothingType,
									ColorHex = colorHex,
									Prompt = prompt
								});

								_logger.LogInformation("Generated virtual try-on for {Environment}, {ClothingType}, {Color}",
									environment, clothingType, colorHex);
							}
							catch (Exception ex)
							{
								_logger.LogError(ex, "Failed to generate image for {Environment}, {ClothingType}, {Color}",
									environment, clothingType, colorHex);
							}
						}
					}
				}

				return response;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating virtual try-on images");
				throw new Exception("Failed to generate virtual try-on images", ex);
			}
		}

		private string BuildVirtualTryOnPrompt(string environment, string clothingType, string colorHex)
		{
			var environmentDescriptions = new Dictionary<string, string>
			{
				{ "indoor", "in a well-lit indoor setting with natural window light" },
				{ "outdoor_sunny", "outdoors on a sunny day with bright natural sunlight" },
				{ "outdoor_cloudy", "outdoors on a cloudy day with soft, diffused lighting" },
				{ "evening", "in an evening setting with warm artificial lighting" }
			};

			var clothingDescriptions = new Dictionary<string, string>
			{
				{ "shirt", "a stylish casual shirt" },
				{ "dress", "an elegant dress" },
				{ "sweater", "a cozy sweater" },
				{ "blouse", "a professional blouse" },
				{ "jacket", "a fashionable jacket" }
			};

			return $@"
Generate a photorealistic image of a person wearing {clothingDescriptions.GetValueOrDefault(clothingType, clothingType)} 
in color {colorHex} {environmentDescriptions.GetValueOrDefault(environment, environment)}.

The image should:
- Show the person from waist up
- Maintain the person's facial features and skin tone from the reference image
- Display the clothing item prominently
- Have realistic lighting that matches the {environment} environment
- Look natural and professionally photographed
- Show how the color {colorHex} complements the person's features

Style: Fashion photography, high quality, professional";
		}
	}
}