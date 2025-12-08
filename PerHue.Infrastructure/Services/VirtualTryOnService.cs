using GenerativeAI;
using GenerativeAI.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PerHue.Application.IServices;
using PerHue.Application.Models.AiTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Services
{
	public class VirtualTryOnService : IVirtualTryOnService
	{
		private readonly string _apiKey;
		private readonly ILogger<VirtualTryOnService> _logger;
		private readonly IImageUploadService _imageUploadService;
		private readonly HttpClient _httpClient;


		// Các model có thể sử dụng
		
		private const string MODEL_FLUX = "black-forest-labs/FLUX.1-schnell";


		public VirtualTryOnService(
			IConfiguration configuration,
			ILogger<VirtualTryOnService> logger,
			IImageUploadService imageUploadService)
		{
			_apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini API Key not found");
			_logger = logger;
			_imageUploadService = imageUploadService;

			_apiKey = configuration["HuggingFace:ApiKey"]
				?? throw new ArgumentNullException("HuggingFace API Key not found");
			_httpClient = new HttpClient();
			_httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", _apiKey);
			_httpClient.Timeout = TimeSpan.FromMinutes(5); // Tăng timeout vì model cần load
		}

		public async Task<VirtualTryOnResponse> GenerateVirtualTryOnImagesAsync(VirtualTryOnRequest request)
		{
			// PRINT THE LAST 4 CHARS OF THE KEY TO CONSOLE
			var keyLast4 = _apiKey.Length > 4 ? _apiKey.Substring(_apiKey.Length - 4) : "INVALID";
			_logger.LogWarning($"[DEBUG] Using API Key ending in: ...{keyLast4}");
			try
			{
				// ✅ SỬ DỤNG MODEL HỖ TRỢ IMAGE GENERATION
				var client = new GoogleAi(_apiKey);
				var model = client.CreateGenerativeModel("gemini-2.5-flash");
				//var model = client.CreateGenerativeModel("gemini-3-pro-image-preview");
				//var model = client.CreateGenerativeModel("gemini-2.0-flash-exp");

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
					// ✅ THÊM USER IMAGE TỪ IFormFile
					byte[] userImageBytes;
					using (var memoryStream = new MemoryStream())
					{
						await request.UserImage.CopyToAsync(memoryStream);
						userImageBytes = memoryStream.ToArray();
					}

					// Xác định MIME type từ file
					var mimeType = request.UserImage.ContentType ?? "image/jpeg";

					_logger.LogInformation("User image loaded from IFormFile: {FileName}, Size: {Size} bytes, MimeType: {MimeType}",
						request.UserImage.FileName, userImageBytes.Length, mimeType);

					// ✅ CHUẨN BỊ PARTS CHO GEMINI REQUEST (SỬ DỤNG ĐÚNG TYPES)
					var parts = new List<GenerativeAI.Types.Part>
					{
						new GenerativeAI.Types.Part { Text = prompt },
						new GenerativeAI.Types.Part
						{
							InlineData = new GenerativeAI.Types.Blob
							{
								MimeType = mimeType,
								Data = Convert.ToBase64String(userImageBytes)
							}
						}
					};


					// ✅ QUAN TRỌNG: GenerationConfig VỚI responseModalities
					//var generationConfig = new GenerationConfig
					//{
					//	ResponseModalities = new List<Modality> { Modality.TEXT, Modality.IMAGE }
					//};

					var generationConfig = new GenerationConfig
					{
						// Force the model to generate exactly ONE option (saves quota)
						CandidateCount = 1,

						// Explicitly ask for IMAGE only. 
						// (Note: Some models might default to adding text anyway, but this sets the preference)
						ResponseModalities = new List<Modality> { Modality.IMAGE },

						// Lower temperature slightly for better prompt adherence (less random creativity)
						Temperature = 0.6f
					};

					// ✅ GỌI API VỚI CONFIG ĐÚNG
					var geminiResponse = await model.GenerateContentAsync(
					request: new GenerativeAI.Types.GenerateContentRequest
					{
						Contents = new List<GenerativeAI.Types.Content>
						{
							new GenerativeAI.Types.Content
							{
								Role = "user",
								Parts = parts // Use parts directly without re-mapping
							}
						},
						GenerationConfig = generationConfig
					}
				);

					// Extract image từ Gemini response
					string generatedImageUrl = null;
					_logger.LogInformation("Gemini Response received. Candidates count: {Count}",
	geminiResponse?.Candidates?.Count() ?? 0);

					if (geminiResponse?.Candidates?.Count() > 0 &&
						geminiResponse.Candidates[0].Content?.Parts?.Count > 0)
					{
						var candidate = geminiResponse?.Candidates?[0];

						// Check if the model stopped because of safety or quota issues
						if (candidate.FinishReason != FinishReason.STOP)
						{
							_logger.LogWarning($"Model did not finish cleanly. Reason: {candidate.FinishReason}");
						}

						// Proceed to extract image
						var imagePart = candidate.Content.Parts.FirstOrDefault(p => p.InlineData != null);

						if (imagePart?.InlineData != null)
						{
							// Convert base64 to stream và upload lên Cloudinary
							var imageBytes = Convert.FromBase64String(imagePart.InlineData.Data);
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
								ContentType = imagePart.InlineData.MimeType ?? "image/jpeg"
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
					else
					{
						throw new Exception("Invalid Gemini response structure");
					}
					if (geminiResponse?.Candidates?.Any() == true)
					{
						var candidate = geminiResponse.Candidates.First();

						_logger.LogInformation("Candidate FinishReason: {Reason}", candidate.FinishReason);

						// Check if the model stopped because of safety or quota issues
						if (candidate.FinishReason != FinishReason.STOP)
						{
							_logger.LogWarning("Model did not finish cleanly. Reason: {Reason}", candidate.FinishReason);

							// Log safety ratings if available
							if (candidate.SafetyRatings?.Any() == true)
							{
								foreach (var rating in candidate.SafetyRatings)
								{
									_logger.LogWarning("Safety Rating - Category: {Category}, Probability: {Probability}",
										rating.Category, rating.Probability);
								}
							}
						}

						_logger.LogInformation("Parts count in response: {Count}",
							candidate.Content?.Parts?.Count ?? 0);

						if (candidate.Content?.Parts?.Any() == true)
						{
							// Log what types of parts we received
							foreach (var part in candidate.Content.Parts)
							{
								if (part.Text != null)
								{
									_logger.LogInformation("Received TEXT part: {Text}",
										part.Text.Substring(0, Math.Min(100, part.Text.Length)));
								}
								if (part.InlineData != null)
								{
									_logger.LogInformation("Received IMAGE part: MimeType={MimeType}, DataLength={Length}",
										part.InlineData.MimeType, part.InlineData.Data?.Length ?? 0);
								}
							}

							// Find the image part
							var imagePart = candidate.Content.Parts.FirstOrDefault(p => p.InlineData != null);

							if (imagePart?.InlineData != null)
							{
								_logger.LogInformation("Found image data. MimeType: {MimeType}, Data length: {Length}",
									imagePart.InlineData.MimeType, imagePart.InlineData.Data?.Length ?? 0);

								try
								{
									// ✅ FIXED: Proper Base64 decoding
									// The Data property should already be a base64 string
									var base64Data = imagePart.InlineData.Data;

									// Remove any whitespace or line breaks that might cause issues
									base64Data = base64Data?.Replace("\n", "").Replace("\r", "").Trim();

									if (string.IsNullOrEmpty(base64Data))
									{
										throw new Exception("Image data is null or empty");
									}

									_logger.LogInformation("Decoding base64 data of length: {Length}", base64Data.Length);

									var imageBytes = Convert.FromBase64String(base64Data);

									_logger.LogInformation("Successfully decoded {ByteCount} bytes", imageBytes.Length);

									using var imageStream = new MemoryStream(imageBytes);

									// Create IFormFile for upload
									var colorsList = string.Join("_", selectedColors.Select(c => c.Replace("#", "")));
									var fileName = $"virtual_tryon_{colorsList}_{Guid.NewGuid()}.jpg";

									var formFile = new FormFile(
										imageStream,
										0,
										imageBytes.Length,
										"image",
										fileName)
									{
										Headers = new HeaderDictionary(),
										ContentType = imagePart.InlineData.MimeType ?? "image/jpeg"
									};

									_logger.LogInformation("Uploading image to Cloudinary: {FileName}", fileName);

									generatedImageUrl = await _imageUploadService.UploadImageAsync(formFile);

									_logger.LogInformation("Successfully uploaded to Cloudinary: {Url}", generatedImageUrl);

									// Add to response
									response.GeneratedImages.Add(new Application.Models.AiTest.GeneratedImage
									{
										ImageUrl = generatedImageUrl,
										Environment = environment,
										ClothingType = "complete_outfit",
										ColorHex = string.Join(", ", selectedColors),
										Prompt = prompt
									});

									_logger.LogInformation("Successfully generated virtual try-on with colors: {Colors}",
										string.Join(", ", selectedColors));
								}
								catch (FormatException ex)
								{
									_logger.LogError(ex, "Invalid base64 format. Data preview: {Preview}",
										imagePart.InlineData.Data?.Substring(0, Math.Min(100, imagePart.InlineData.Data?.Length ?? 0)));
									throw new Exception("Failed to decode image data from Gemini response", ex);
								}
								catch (Exception ex)
								{
									_logger.LogError(ex, "Failed to process generated image");
									throw;
								}
							}
							else
							{
								_logger.LogError("No image part found in response. Available parts: {PartTypes}",
									string.Join(", ", candidate.Content.Parts.Select(p =>
										p.Text != null ? "text" :
										p.InlineData != null ? "image" : "unknown")));
								throw new Exception("No image data found in Gemini response");
							}
						}
						else
						{
							_logger.LogError("Candidate has no parts in Content");
							throw new Exception("Gemini response has no content parts");
						}
					}
					else
					{
						_logger.LogError("No candidates in Gemini response");
						throw new Exception("Invalid Gemini response structure - no candidates");
					}

					if (string.IsNullOrEmpty(generatedImageUrl))
					{
						throw new Exception("Failed to generate and upload virtual try-on image");
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

/*		private string BuildVirtualTryOnPromptWithMultipleColors(string environment, List<string> colorHexCodes)
		{
			var environmentDescriptions = new Dictionary<string, string>
		{
			{ "indoor", "in a well-lit, modern indoor setting with natural window light" },
			{ "outdoor_sunny", "outdoors on a beautiful sunny day with bright natural sunlight" },
			{ "outdoor_cloudy", "outdoors on a pleasant cloudy day with soft, diffused natural lighting" },
			{ "evening", "in a stylish evening setting with warm, atmospheric lighting" }
		};

			var environmentDesc = environmentDescriptions.GetValueOrDefault(environment, "in a natural outdoor setting");

			// Tạo mô tả cho từng màu với clothing item cụ thể
			var colorAssignments = new List<string>();
			var clothingItems = new List<string> { "shirt/top", "pants/skirt", "shoes", "accessories/hat" };

			for (int i = 0; i < colorHexCodes.Count && i < clothingItems.Count; i++)
			{
				colorAssignments.Add($"- {clothingItems[i]} in color {colorHexCodes[i]}");
			}

			var colorDescriptions = string.Join("\n", colorAssignments);
			var allColors = string.Join(", ", colorHexCodes);

			return $@"
OUTPUT FORMAT REQUIREMENT: Generate exactly ONE image. Do not provide any text, descriptions, or explanations. Just the raw image.

Using the provided reference image of the person, create a photorealistic virtual try-on fashion image showing them wearing a complete coordinated outfit {environmentDesc}.

CRITICAL REQUIREMENTS - MUST MAINTAIN FROM REFERENCE IMAGE:
- Keep the person's EXACT facial features, face shape, and facial structure
- Maintain their EXACT skin tone and complexion
- Preserve their hairstyle and hair color
- Keep their body proportions and body type
- Maintain their natural pose and posture
- Keep the same person identity - this is very important!

OUTFIT COLOR SCHEME (use ALL these colors in the outfit):
{colorDescriptions}

The outfit should incorporate ALL {colorHexCodes.Count} colors ({allColors}) in a stylish, coordinated way across different clothing pieces and accessories.

STYLE REQUIREMENTS:
1. Create a fashionable, modern outfit that naturally combines all {colorHexCodes.Count} colors
2. The colors should be distributed across: top/shirt, bottom/pants or skirt, footwear, and accessories (hat, bag, or jewelry)
3. Show the person from head to toe (full body shot) or at least from waist up
4. The environment should be {environmentDesc}
5. Professional fashion photography lighting that enhances the outfit colors
6. Natural, confident pose showing off the coordinated outfit
7. Make sure each color {allColors} is clearly visible and prominent in the outfit
8. The outfit should look cohesive and stylish, not random or mismatched

PHOTOGRAPHY STYLE:
- High-quality fashion photography
- Professional lighting matching {environment} environment
- Sharp focus on both the person and outfit details
- Natural, realistic rendering
- Colors should be vibrant and accurately match the hex codes: {allColors}
- Background appropriate for {environment} but should not distract from the outfit

IMPORTANT: This is a virtual try-on - maintain the EXACT same person from the reference image, only change their outfit to incorporate the specified colors.";
		}*/

		//=============================================================

		public async Task<HuggingFaceModel.HFVirtualTryOnResponse> HFGenerateVirtualTryOnImagesAsync(VirtualTryOnRequest request)
		{
			var keyLast4 = _apiKey.Length > 4 ? _apiKey.Substring(_apiKey.Length - 4) : "INVALID";
			_logger.LogWarning($"[DEBUG] Using API Key ending in: ...{keyLast4}");

			try
			{
				// Lấy màu và environment
				var selectedColors = request.SuggestedColorHexCodes
					.OrderBy(x => Guid.NewGuid())
					.Take(Math.Min(4, request.SuggestedColorHexCodes.Count))
					.ToList();

				var environment = request.Environments
					.OrderBy(x => Guid.NewGuid())
					.FirstOrDefault() ?? "outdoor_sunny";

				_logger.LogInformation("Generating virtual try-on with {Count} colors", selectedColors.Count);

				// Đọc user image
				byte[] userImageBytes;
				using (var memoryStream = new MemoryStream())
				{
					await request.UserImage.CopyToAsync(memoryStream);
					userImageBytes = memoryStream.ToArray();
				}

				// Tạo prompt
				var prompt = BuildVirtualTryOnPromptWithMultipleColors(environment, selectedColors);

				// Generate image với retry
				var imageResult = await GenerateImageWithRetryAsync(prompt, userImageBytes, maxRetries: 3);

				var response = new HuggingFaceModel.HFVirtualTryOnResponse
				{
					GeneratedImages = new HuggingFaceModel.HFGeneratedImage
					{
						ImageUrl = imageResult.ImageUrl,           // URL nếu có
						ImageBase64 = imageResult.ImageBase64,     // Base64 nếu có
						ImageBytes = imageResult.ImageBytes,       // Raw bytes
						Environment = environment,
						ClothingType = "complete_outfit",
						ColorHex = string.Join(", ", selectedColors),
						Prompt = prompt
					}
				};

				_logger.LogInformation("Virtual try-on completed successfully");
				return response;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating virtual try-on images");
				throw new Exception("Failed to generate virtual try-on images", ex);
			}
		}

		private async Task<ImageResult> GenerateImageWithRetryAsync(
			string prompt,
			byte[] userImageBytes,
			int maxRetries = 3)
		{
			Exception lastException = null;

			for (int attempt = 1; attempt <= maxRetries; attempt++)
			{
				try
				{
					_logger.LogInformation("Attempt {Attempt} of {MaxRetries}", attempt, maxRetries);
					return await GenerateImageAsync(prompt, userImageBytes);
				}
				catch (Exception ex)
				{
					lastException = ex;
					_logger.LogWarning(ex, "Attempt {Attempt} failed", attempt);

					if (attempt < maxRetries)
					{
						var delaySeconds = Math.Pow(2, attempt) * 5;
						_logger.LogInformation("Waiting {Delay}s before retry...", delaySeconds);
						await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
					}
				}
			}

			throw new Exception($"Failed after {maxRetries} attempts", lastException);
		}

		private async Task<ImageResult> GenerateImageAsync(string prompt, byte[] userImageBytes)
		{
			var modelId = MODEL_FLUX; // Hoặc MODEL_SDXL
			var apiUrl = $"https://api-inference.huggingface.co/models/{modelId}";

			try
			{
				// Convert image to base64
				var base64Image = Convert.ToBase64String(userImageBytes);

				// Payload cho text+image-to-image
				var payload = new
				{
					inputs = new
					{
						prompt = prompt,
						image = base64Image,
						negative_prompt = BuildNegativePrompt()
					},
					parameters = new
					{
						num_inference_steps = 25,
						guidance_scale = 3.5,
						strength = 0.75,
						width = 1024,
						height = 1024
					},
					options = new
					{
						wait_for_model = true,
						use_cache = false
					}
				};

				var jsonContent = JsonConvert.SerializeObject(payload);
				var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

				_logger.LogInformation("Calling Hugging Face API: {Model}", modelId);

				var response = await _httpClient.PostAsync(apiUrl, content);

				// Xử lý model loading
				if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogWarning("Model is loading: {Error}", errorContent);

					try
					{
						var errorObj = JsonConvert.DeserializeObject<HuggingFaceError>(errorContent);
						if (errorObj?.EstimatedTime > 0)
						{
							_logger.LogInformation("Waiting {Time}s for model...", errorObj.EstimatedTime);
							await Task.Delay(TimeSpan.FromSeconds(errorObj.EstimatedTime + 5));
							return await GenerateImageAsync(prompt, userImageBytes);
						}
					}
					catch { }

					await Task.Delay(20000);
					throw new Exception("Model is loading, please retry");
				}

				if (!response.IsSuccessStatusCode)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogError("API Error: {StatusCode} - {Error}", response.StatusCode, errorContent);
					throw new Exception($"API Error {response.StatusCode}: {errorContent}");
				}

				// Đọc raw image bytes
				var imageBytes = await response.Content.ReadAsByteArrayAsync();
				_logger.LogInformation("Received image: {Size} bytes", imageBytes.Length);

				if (imageBytes.Length < 1000)
				{
					throw new Exception("Invalid image data received");
				}

				// Trả về cả 3 formats: bytes, base64, và có thể URL nếu cần
				return new ImageResult
				{
					ImageBytes = imageBytes,
					ImageBase64 = Convert.ToBase64String(imageBytes),
					ImageUrl = null // Không có URL, chỉ có raw data
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to generate image");
				throw;
			}
		}

		private string BuildVirtualTryOnPromptWithMultipleColors(string environment, List<string> colorHexCodes)
		{
			var environmentDescriptions = new Dictionary<string, string>
			{
				{ "indoor", "modern indoor studio with professional lighting" },
				{ "outdoor_sunny", "sunny outdoor location with natural daylight" },
				{ "outdoor_cloudy", "outdoor setting with soft diffused lighting" },
				{ "evening", "evening atmosphere with warm golden hour lighting" }
			};

			var environmentDesc = environmentDescriptions.GetValueOrDefault(environment, "natural outdoor setting");

			var colorDescriptions = new List<string>();
			var clothingItems = new List<string> { "fashionable top", "stylish pants", "trendy shoes", "accessories" };

			for (int i = 0; i < colorHexCodes.Count && i < clothingItems.Count; i++)
			{
				colorDescriptions.Add($"{clothingItems[i]} in {colorHexCodes[i]} color");
			}

			var colorDesc = string.Join(", ", colorDescriptions);

			return $@"professional fashion photography, full body portrait, person wearing coordinated outfit with {colorDesc}, {environmentDesc}, high quality, photorealistic, 8k resolution, sharp focus, detailed clothing texture, vibrant colors, modern fashion style, confident pose, natural expression, fashion magazine quality, professional model lighting";
		}

		private string BuildNegativePrompt()
		{
			return @"low quality, blurry, distorted, deformed, ugly, bad anatomy, bad proportions, disfigured, poorly drawn face, mutation, mutated, extra limbs, gross proportions, missing arms, missing legs, extra arms, extra legs, fused fingers, too many fingers, long neck, cross-eyed, mutated hands, bad hands, bad feet, cropped, worst quality, low resolution, jpeg artifacts, watermark, signature, username, text, cartoon, anime, illustration, painting, drawing, 3d render";
		}

	}

	public class ImageResult
	{
		public byte[] ImageBytes { get; set; }      // Raw image data
		public string ImageBase64 { get; set; }     // Base64 string
		public string ImageUrl { get; set; }        // URL (null nếu không upload)
	}

	public class HuggingFaceError
	{
		[JsonProperty("error")]
		public string Error { get; set; }

		[JsonProperty("estimated_time")]
		public double EstimatedTime { get; set; }
	}
}