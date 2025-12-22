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

				// ✅ TẠO PROMPT CHO 1 ẢNH VỚI NHIỀU MÀU
				var prompt = BuildVirtualTryOnPromptWithMultipleColors(selectedColors);

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
								ColorHex = string.Join(", ", selectedColors), // Danh sách màu
								Prompt = prompt
							});

							_logger.LogInformation("Successfully generated virtual try-on image with colors: {Colors}",
								string.Join(", ", selectedColors));
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

		private string BuildVirtualTryOnPromptWithMultipleColors(List<string> colorHexCodes)
		{
			var environmentDescriptions = new Dictionary<string, string>
		{
			{ "indoor", "in a well-lit, modern indoor setting with natural window light" },
			{ "outdoor_sunny", "outdoors on a beautiful sunny day with bright natural sunlight" },
			{ "outdoor_cloudy", "outdoors on a pleasant cloudy day with soft, diffused natural lighting" },
			{ "evening", "in a stylish evening setting with warm, atmospheric lighting" }
		};

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

Using the provided reference image of the person, create a photorealistic virtual try-on fashion image showing them wearing a complete coordinated outfit.

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
4. The environment should be
5. Professional fashion photography lighting that enhances the outfit colors
6. Natural, confident pose showing off the coordinated outfit
7. Make sure each color {allColors} is clearly visible and prominent in the outfit
8. The outfit should look cohesive and stylish, not random or mismatched

PHOTOGRAPHY STYLE:
- High-quality fashion photography
- Professional lighting matching environment
- Sharp focus on both the person and outfit details
- Natural, realistic rendering
- Colors should be vibrant and accurately match the hex codes: {allColors}
- Background appropriate for but should not distract from the outfit

IMPORTANT: This is a virtual try-on - maintain the EXACT same person from the reference image, only change their outfit to incorporate the specified colors.";
		}

		//=============================================================

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