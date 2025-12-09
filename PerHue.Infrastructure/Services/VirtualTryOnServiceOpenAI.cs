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
				var prompt = BuildVirtualTryOnPromptWithMultipleColors(environment, selectedColors);

				try
				{
					string generatedImageUrl = string.Empty;

					var httpRequest = new HttpRequestMessage(HttpMethod.Post, "http://perhue.duckdns.org:8443/v1/images/edits");
					//var httpRequest = new HttpRequestMessage(HttpMethod.Post, "http://perhue.duckdns.org:443/v1/images/edits'");
					var formData = new MultipartFormDataContent();
					AppendScalar(formData, "prompt", prompt);
					AppendScalar(formData, "model", "openai/gpt-image-1-mini");
					AppendScalar(formData, "size", "1024x1024");
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

		private string BuildVirtualTryOnPromptWithMultipleColors(string environment, List<string> colorHexCodes)
		{
			var environmentDescriptions = new Dictionary<string, string>
	{
		{ "indoor", "in a well-lit, modern indoor setting with natural window light" },
		{ "outdoor_sunny", "outdoors on a beautiful sunny day with bright natural sunlight" },
		{ "outdoor_cloudy", "outdoors on a pleasant cloudy day with soft, diffused natural lighting" },
		{ "evening", "in a stylish evening setting with warm, atmospheric lighting" }
	};

			var environmentDesc = environmentDescriptions.GetValueOrDefault(
				environment,
				"in a natural outdoor setting"
			);

			// Clothing items for color distribution
			var clothingItems = new List<string>
	{
		"shirt/top",
		"pants/skirt",
		"shoes",
		"accessories (hat/bag)"
	};

			var colorAssignments = new List<string>();
			for (int i = 0; i < colorHexCodes.Count && i < clothingItems.Count; i++)
			{
				colorAssignments.Add($"- {clothingItems[i]} in color {colorHexCodes[i]}");
			}

			var colorDescriptions = string.Join("\n", colorAssignments);
			var allColors = string.Join(", ", colorHexCodes);

			return $@"
OUTPUT FORMAT: Generate exactly ONE image. Output only the image with no text.

Using the provided reference image of the person, generate a photorealistic virtual try-on image showing them wearing a coordinated outfit {environmentDesc}.

===========================
CRITICAL FRAMING REQUIREMENTS (HIGHEST PRIORITY - MUST FOLLOW)
===========================
CAMERA DISTANCE & FRAMING:
- Shot type: FULL BODY PORTRAIT - Extreme Long Shot
- Camera must be positioned FAR ENOUGH to capture the ENTIRE person from head to toe
- Person's height in frame: 165-170cm (5'5""-5'7"")
- The person should occupy approximately 70-80% of the image height (NOT 100%)
- Leave 10-15% empty space ABOVE the head
- Leave 10-15% empty space BELOW the feet
- Leave adequate space on left and right sides

MANDATORY VISIBILITY:
✓ COMPLETE head (including all hair/hat if present)
✓ FULL torso and arms
✓ ENTIRE legs from hip to ankle
✓ COMPLETE feet and shoes (both feet must be fully visible)
✓ All clothing items must be 100% visible

FORBIDDEN - NEVER DO THIS:
✗ DO NOT crop the head, hair, or top of hat
✗ DO NOT crop the feet or shoes at bottom
✗ DO NOT zoom in too close
✗ DO NOT cut off any body part at the frame edges
✗ DO NOT let the person touch the top or bottom frame borders

COMPOSITION:
- Subject perfectly centered in frame
- Vertical orientation (portrait mode)
- Natural standing pose showing full body
- Adequate breathing room on all four sides

===========================
IDENTITY PRESERVATION (MAINTAIN EXACT LIKENESS)
===========================
MUST PRESERVE FROM REFERENCE IMAGE:
- EXACT facial features: eyes, nose, mouth, eyebrows, face shape
- EXACT skin tone and complexion (do not lighten or darken)
- EXACT hairstyle, hair color, and hair texture
- Body proportions and physique type
- Natural facial expression
- Ethnicity and distinctive features

CRITICAL: The face must look like the SAME PERSON as in the reference image. Only the outfit should change.

===========================
COLOR ASSIGNMENT RULES
===========================
- Distribute the provided colors across different clothing items
- Each color should be used for ONE clothing item only
- DO NOT mix multiple provided colors on a single item
- Keep each piece clean with ONE dominant color

Assigned colors:
{colorDescriptions}

Use these exact color hex codes: {allColors}

===========================
OUTFIT GENERATION RULES
===========================
- Modern, stylish, coordinated outfit
- Top/shirt: ONE solid color from the list
- Pants/skirt: ONE solid color from the list  
- Shoes: ONE solid color from the list (MUST BE FULLY VISIBLE)
- If accessories: use remaining colors
- If more colors than items: ignore excess colors
- If fewer colors than items: use neutral colors (white/black/gray) for remaining items
- Ensure outfit is appropriate for the specified environment

===========================
PHOTOGRAPHY & TECHNICAL SPECS
===========================
- Professional fashion photography quality
- Sharp focus on entire person and outfit
- Proper lighting for {environment} setting
- Colors must accurately match the hex values provided
- High resolution and detail
- Natural, flattering pose
- Clean, uncluttered background appropriate for environment

FINAL CHECK BEFORE GENERATING:
□ Can you see the TOP of the head? (with margin above)
□ Can you see BOTH shoes completely? (with margin below)
□ Is the person centered with space around them?
□ Does the face match the reference image exactly?
□ Are all colors correctly applied to clothing?
";
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