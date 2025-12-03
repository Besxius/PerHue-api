using GenerativeAI;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PerHue.Application.IServices;
using PerHue.Application.Models.AiTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Services
{
	public class VirtualTryOnService : IVirtualTryOnService
	{
		private readonly string _apiKey;
		private readonly ILogger<VirtualTryOnService> _logger;
		private readonly IImageUploadService _imageUploadService;

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
        try
        {
            // ✅ SỬ DỤNG MODEL HỖ TRỢ IMAGE GENERATION
            var client = new GoogleAi(_apiKey);
            //var model = client.CreateGenerativeModel("gemini-2.5-flash-image");
			var model = client.CreateGenerativeModel("gemini-3-pro-image-preview");
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
                // Chuẩn bị parts cho Gemini request
                var parts = new List<Part>();

                // ✅ THÊM TEXT PROMPT TRƯỚC
                parts.Add(new Part { Text = prompt });

                // ✅ THÊM USER IMAGE TỪ IFormFile
                byte[] userImageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await request.UserImage.CopyToAsync(memoryStream);
                    userImageBytes = memoryStream.ToArray();
                }

                // Xác định MIME type từ file
                var mimeType = request.UserImage.ContentType ?? "image/jpeg";
                
                parts.Add(new Part
                {
                    InlineData = new Blob
                    {
                        MimeType = mimeType,
						//Data = Convert.ToBase64String(userImageBytes)
						Data = userImageBytes
					}
                });

                _logger.LogInformation("User image loaded from IFormFile: {FileName}, Size: {Size} bytes, MimeType: {MimeType}", 
                    request.UserImage.FileName, userImageBytes.Length, mimeType);

                // ✅ QUAN TRỌNG: PHẢI CÓ GenerationConfig VỚI responseModalities
                var generationConfig = new GenerationConfig
                {
					ResponseModalities = new List<Modality> { Modality.TEXT, Modality.IMAGE }
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
								Parts = parts.Select(p => new GenerativeAI.Types.Part
                                {
                                    Text = p.Text,
                                    InlineData = p.InlineData == null ? null : new GenerativeAI.Types.Blob
									{
										MimeType = p.InlineData.MimeType,
										// FIX: Convert byte[] to base64 string for Data property
										Data = p.InlineData.Data != null ? Convert.ToBase64String(p.InlineData.Data) : null
									}
									// Copy other relevant properties if needed
								}).ToList()
							}
						},
					}
                );

                // Extract image từ Gemini response
                string generatedImageUrl;

                if (geminiResponse?.Candidates?.Count() > 0 &&
                    geminiResponse.Candidates[0].Content?.Parts?.Count > 0)
                {
                    var imagePart = geminiResponse.Candidates[0].Content.Parts
                        .FirstOrDefault(p => p.InlineData != null);

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

        return $@"Using the provided reference image of the person, create a photorealistic virtual try-on fashion image showing them wearing a complete coordinated outfit {environmentDesc}.

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
    }

		// Giữ lại method cũ để backward compatibility (nếu cần)
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

			return $@"Generate a photorealistic image of a person wearing {clothingDescriptions.GetValueOrDefault(clothingType, clothingType)} 
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