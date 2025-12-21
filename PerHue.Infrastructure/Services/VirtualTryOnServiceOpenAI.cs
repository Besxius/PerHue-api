using GenerativeAI;
using GenerativeAI.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PerHue.Application.IServices;
using PerHue.Application.Models.AiTest;
using PerHue.Infrastructure.AI;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace PerHue.Infrastructure.Services
{
	public class VirtualTryOnServiceOpenAI : IVirtualTryOnService
	{
		private readonly string _apiKey;
		private readonly ILogger<VirtualTryOnService> _logger;
		private readonly IImageUploadService _imageUploadService;
		private readonly IHttpClientFactory _httpClientFactory;

		public VirtualTryOnServiceOpenAI(
			IConfiguration configuration,
			ILogger<VirtualTryOnService> logger,
			IImageUploadService imageUploadService,
			IHttpClientFactory httpClientFactory)
		{
			_apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini API Key not found");
			_logger = logger;
			_imageUploadService = imageUploadService;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<VirtualTryOnResponse> GenerateVirtualTryOnImagesAsync(VirtualTryOnRequest request)
		{
			// PRINT THE LAST 4 CHARS OF THE KEY TO CONSOLE
			var keyLast4 = _apiKey.Length > 4 ? _apiKey.Substring(_apiKey.Length - 4) : "INVALID";
			_logger.LogWarning($"[DEBUG] Using API Key ending in: ...{keyLast4}");
			try
			{
				// ✅ SỬ DỤNG MODEL HỖ TRỢ IMAGE GENERATION

				var response = new VirtualTryOnResponse();

				// ✅ LẤY 3-4 MÀU ĐỂ TẠO 1 ẢNH DUY NHẤT
				var selectedColors = request.SuggestedColorHexCodes
					.OrderBy(x => Guid.NewGuid())
					.Take(Math.Min(4, request.SuggestedColorHexCodes.Count))
					.ToList();

				// Chọn ngẫu nhiên 1 environment
				var environment = request.Environments.OrderBy(x => Guid.NewGuid()).FirstOrDefault() ?? "outdoor_sunny";

				_logger.LogInformation("Generating ONE virtual try-on image with {Count} colors in {Environment} environment",
					selectedColors.Count, environment);

				// ✅ TẠO PROMPT CHO 1 ẢNH VỚI NHIỀU MÀU
				var prompt = BuildVirtualTryOnPromptWithMultipleColors(selectedColors);

				try
				{
					string generatedImageUrl = string.Empty;

					var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://litellm.perhue.dpdns.org/v1/images/edits");
					var formData = new MultipartFormDataContent();
					AppendScalar(formData, "prompt", prompt);
					AppendScalar(formData, "model", "openai/gpt-image-1.5");
					//AppendScalar(formData, "size", "1024x1024");
					//AppendScalar(formData, "quality", "low");
					AppendImageFiles(formData, [request.UserImage]);

					httpRequest.Content = formData;
					HttpClient client = _httpClientFactory.CreateClient();
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "sk-HV9mO1l3CVtjFc-u7-KE7A");
					using var responseData = await client.SendAsync(httpRequest);

					var payload = await responseData.Content.ReadFromJsonAsync<ApiResponse>();
					if (payload?.Data != null)
					{
						// Convert base64 to stream và upload lên Cloudinary
						var imageBytes = Convert.FromBase64String(payload.Data.FirstOrDefault().B64Json);
						using var imageStream = new MemoryStream(imageBytes);

						// Tạo IFormFile từ stream để upload
						var colorsList = string.Join("_", selectedColors.Select(c => c.Replace("#", "")));
						var formFile = new FormFile(
							imageStream,
							0,
							imageBytes.Length,
							"image",
							$"virtual_tryon_{colorsList}_{Guid.NewGuid()}.jpg")
						{
							Headers = new HeaderDictionary(),
							ContentType = payload.OutputFormat ?? "image/jpeg"
						};

						generatedImageUrl = await _imageUploadService.UploadImageAsync(formFile);
						_logger.LogInformation("Successfully uploaded generated image to Cloudinary: {Url}", generatedImageUrl);

						// ✅ THÊM VÀO RESPONSE (CHỈ 1 ẢNH)
						response.GeneratedImages.Add(new Application.Models.AiTest.GeneratedImage
						{
							ImageUrl = generatedImageUrl,
							Environment = environment,
							ClothingType = "complete_outfit", // Outfit hoàn chỉnh với nhiều items
							ColorHex = string.Join(", ", selectedColors), // Danh sách màu
							Prompt = prompt
						});

						_logger.LogInformation("Successfully generated virtual try-on image with colors: {Colors} in {Environment}",
							string.Join(", ", selectedColors), environment);
					}
					else
					{
						throw new Exception("No image data found in Gemini response");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to generate virtual try-on image");
					throw;
				}

				if (response.GeneratedImages.Count == 0)
				{
					throw new Exception("Failed to generate virtual try-on image");
				}

				_logger.LogInformation("Virtual try-on generation completed successfully");
				return response;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating virtual try-on images");
				throw new Exception("Failed to generate virtual try-on images", ex);
			}
		}

		private string BuildVirtualTryOnPromptWithMultipleColors(List<string> colorHexCodes)
		{
			// 1. Kiểm tra đầu vào
			if (colorHexCodes == null || colorHexCodes.Count == 0)
			{
				throw new ArgumentException("Danh sách màu không được để trống.", nameof(colorHexCodes));
			}

			// 2. Chuẩn bị dữ liệu để chèn vào prompt
			// Tạo chuỗi danh sách màu ngăn cách bởi dấu phẩy (VD: #1C1C1C, #F5F5F0)
			string allColors = string.Join(", ", colorHexCodes);

			// Tạo danh sách gạch đầu dòng cho phần OUTFIT COLOR SCHEME
			// VD: 
			// - #1C1C1C
			// - #F5F5F0
			string colorDescriptionList = string.Join(System.Environment.NewLine, colorHexCodes.Select(c => $"- {c}"));

			int colorCount = colorHexCodes.Count;

			// 3. Tạo prompt sử dụng Verbatim String Interpolation ($@"...")
			// Lưu ý: Trong C#, nếu muốn in dấu " bên trong chuỗi $@"...", bạn phải dùng 2 dấu "" liên tiếp.
			string prompt = $@"OUTPUT FORMAT REQUIREMENT: Generate exactly ONE image. Do not provide any text, descriptions, or explanations. Just the raw image.

Using the provided reference image of the person, create a photorealistic virtual try-on fashion image showing them wearing a complete coordinated outfit in an urban street environment during golden hour.

CRITICAL REQUIREMENTS - MUST MAINTAIN FROM REFERENCE IMAGE:
- Keep the person's EXACT facial features, face shape, and facial structure
- Maintain their EXACT skin tone and complexion
- Preserve their hairstyle and hair color
- Keep their body proportions and body type
- Maintain their natural pose and posture
- Keep the same person identity - this is very important!

OUTFIT COLOR SCHEME (use ALL these colors in the outfit):
{colorDescriptionList}

The outfit should incorporate ALL {colorCount} colors ({allColors}) in a stylish, coordinated way across different clothing pieces and accessories.

STYLE REQUIREMENTS:
1. Create a fashionable, modern outfit that naturally combines all {colorCount} colors
2. The colors should be distributed across: top/shirt, bottom/pants or skirt, footwear, and accessories (bag, belt, or jewelry)
3. Show the person from head to toe (full body shot)
4. The environment should be an urban street setting with soft golden-hour sunlight
5. Professional fashion photography lighting that enhances the outfit colors
6. Natural, confident pose showing off the coordinated outfit
7. Make sure each color ({allColors}) is clearly visible and prominent in the outfit
8. The outfit should look cohesive, elegant, and high-fashion, not random or mismatched

PHOTOGRAPHY STYLE:
- High-quality fashion photography
- Professional lighting suitable for an outdoor urban environment
- Sharp focus on both the person and outfit details
- Natural, realistic rendering
- Colors should be vibrant and accurately match the hex codes: {allColors}
- Background appropriate for an urban street but subtly blurred so it does not distract from the outfit

IMPORTANT: This is a virtual try-on — maintain the EXACT same person from the reference image, only change their outfit to incorporate the specified colors.";

			return prompt;
		}


		//=============================================================


		private static void AppendImageFiles(MultipartFormDataContent formData, IList<IFormFile> images)
		{
			if (images == null || images.Count == 0)
			{
				return;
			}

			foreach (var image in images)
			{
				if (image == null)
				{
					continue;
				}

				var streamContent = new StreamContent(image.OpenReadStream());
				if (!string.IsNullOrWhiteSpace(image.ContentType))
				{
					streamContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
				}

				formData.Add(streamContent, "image", image.FileName);
			}
		}

		private static void AppendMaskFile(MultipartFormDataContent formData, IFormFile? mask)
		{
			if (mask == null)
			{
				return;
			}

			var streamContent = new StreamContent(mask.OpenReadStream());
			if (!string.IsNullOrWhiteSpace(mask.ContentType))
			{
				streamContent.Headers.ContentType = new MediaTypeHeaderValue(mask.ContentType);
			}

			formData.Add(streamContent, "mask", mask.FileName);
		}

		private static void AppendScalar(MultipartFormDataContent formData, string fieldName, string? value)
		{
			if (string.IsNullOrWhiteSpace(fieldName) || string.IsNullOrWhiteSpace(value))
			{
				return;
			}

			formData.Add(new StringContent(value), fieldName);
		}

		private class ApiResponse
		{
			[JsonPropertyName("created")]
			public long Created { get; set; }

			/// <summary>
			/// Raw unix timestamp (seconds). Use CreatedAt for a DateTime view.
			/// </summary>
			[JsonIgnore]
			public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(Created).UtcDateTime;

			[JsonPropertyName("background")]
			public string? Background { get; set; }

			[JsonPropertyName("data")]
			public List<DataItem>? Data { get; set; }

			[JsonPropertyName("output_format")]
			public string? OutputFormat { get; set; }

			[JsonPropertyName("quality")]
			public string? Quality { get; set; }

			[JsonPropertyName("size")]
			public string? Size { get; set; }

			[JsonPropertyName("usage")]
			public Usage? Usage { get; set; }
		}

		private class DataItem
		{
			[JsonPropertyName("b64_json")]
			public string? B64Json { get; set; }

			[JsonPropertyName("revised_prompt")]
			public string? RevisedPrompt { get; set; }

			[JsonPropertyName("url")]
			public string? Url { get; set; }
		}

		private class Usage
		{
			[JsonPropertyName("total_tokens")]
			public int TotalTokens { get; set; }

			[JsonPropertyName("input_tokens")]
			public int InputTokens { get; set; }

			[JsonPropertyName("input_tokens_details")]
			public InputTokensDetails? InputTokensDetails { get; set; }

			[JsonPropertyName("output_tokens")]
			public int OutputTokens { get; set; }
		}

		private class InputTokensDetails
		{
			[JsonPropertyName("image_tokens")]
			public int ImageTokens { get; set; }

			[JsonPropertyName("text_tokens")]
			public int TextTokens { get; set; }
		}
	}
}