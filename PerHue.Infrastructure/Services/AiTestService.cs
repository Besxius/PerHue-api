using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.AiTest;
using PerHue.Application.Models.TestRequest;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.AI;
using PerHue.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PerHue.Application.Models.AiTestModel;


namespace PerHue.Infrastructure.Services
{
	public class AiTestService : IAiTestService
	{
		private readonly IAiTestResultRepository _aiTestRepository;
		private readonly IAIImageAnalysisService _geminiService;
		private readonly IImageUploadService _imageUploadService;
		private readonly ILogger<AiTestService> _logger;

		private readonly IColorMatchingService _colorMatchingService;
		private readonly IVirtualTryOnService _virtualTryOnService;
		private readonly ITestRequestRepository _testRequestRepository;

		public AiTestService(
			IAiTestResultRepository aiTestRepository,
			IAIImageAnalysisService geminiService,
			IImageUploadService imageUploadService,
			ILogger<AiTestService> logger,
			IColorMatchingService colorMatchingService,
			IVirtualTryOnService virtualTryOnService,
			ITestRequestRepository testRequestRepository)
		{
			_aiTestRepository = aiTestRepository;
			_geminiService = geminiService;
			_imageUploadService = imageUploadService;
			_logger = logger;
			_colorMatchingService = colorMatchingService;
			_virtualTryOnService = virtualTryOnService;
			_testRequestRepository = testRequestRepository;
		}

		public async Task<AiTestModel.AiTestResponseModel> CreateAiTestRequestAsync(int userId, AiTestModel.CreateAiTestRequestModel model)
		{
			// Validate images
			if (model.Images == null || model.Images.Count == 0)
			{
				throw new ArgumentException("At least one image is required");
			}

			if (model.Images.Count > 5)
			{
				throw new ArgumentException("Maximum 5 images allowed");
			}

			// Tạo TestRequest
			var testRequest = new TestRequest
			{
				HairColor = model.HairColor,
				EyesColor = model.EyesColor,
				LipsColor = model.LipsColor,
				SkinColor = model.SkinColor,
				Status = "Processing",
				CreatedDate = DateTime.UtcNow,
				TypeOfTest = "AI Test",
				UserAccountId = userId
			};

			testRequest = await _aiTestRepository.CreateTestRequestAsync(testRequest);

			// Upload ảnh lên Cloudinary
			var imageUrls = new List<string>();
			var aiPictures = new List<AiPicture>();

			foreach (var image in model.Images)
			{
				var imageUrl = await _imageUploadService.UploadImageAsync(image);
				imageUrls.Add(imageUrl);

				aiPictures.Add(new AiPicture
				{
					Source = imageUrl,
					Note = "AI Test Image",
					TestRequestId = testRequest.Id
				});
			}

			// Lưu AiPictures
			await _aiTestRepository.CreateAiPicturesAsync(aiPictures);

			// Xử lý AI analysis trong background (hoặc gọi trực tiếp)
			try
			{
				await ProcessAiTestAsync(testRequest.Id);
				// Reload testRequest with all navigation properties
				testRequest = await _aiTestRepository.GetTestRequestByIdAsync(testRequest.Id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error processing AI test for request {testRequest.Id}");
				testRequest.Status = "Failed";
				await _aiTestRepository.UpdateTestRequestAsync(testRequest);
			}

			return new AiTestModel.AiTestResponseModel
			{
				TestRequestId = testRequest.Id,
				Status = testRequest.Status,
				CreatedDate = testRequest.CreatedDate.Value,
				Result = testRequest.AiTestResult != null ? new AiTestResultModel
				{
					ColorType = testRequest.AiTestResult.ColorType.Name,
					ColorTypeId = testRequest.AiTestResult.ColorTypeId,
					SuggestedColor = testRequest.AiTestResult.SuggestedColor.Split(", ").ToList(),
					AvoidedColor = testRequest.AiTestResult.AvoidedColor.Split(", ").ToList()
				} : null
			};
		}

		public async Task<AiTestModel.AiTestResponseModel> ProcessAiTestAsync(int testRequestId)
		{
			var testRequest = await _aiTestRepository.GetTestRequestByIdAsync(testRequestId);

			if (testRequest == null)
			{
				throw new Exception("Test request not found");
			}

			if (testRequest.AiTestResult != null)
			{
				throw new InvalidOperationException("Test already processed");
			}

			try
			{
				// Gọi Gemini để phân tích
				var analysisRequest = new AiTestModel.GeminiAnalysisRequest
				{
					ImageUrls = testRequest.AiPictures.Select(p => p.Source).ToList(),
					HairColor = testRequest.HairColor,
					EyesColor = testRequest.EyesColor,
					LipsColor = testRequest.LipsColor,
					SkinColor = testRequest.SkinColor
				};

				var analysisResult = await _geminiService.AnalyzeColorTypeAsync(analysisRequest);

				// Lưu kết quả
				var aiTestResult = new AiTestResult
				{
					Id = testRequest.Id,
					Date = DateTime.UtcNow,
					ColorTypeId = analysisResult.ColorTypeId,
					SuggestedColor = string.Join(", ", analysisResult.SuggestedColor),
					AvoidedColor = string.Join(", ", analysisResult.AvoidedColor),
					Note = "Analysis completed by AI"
				};

				await _aiTestRepository.CreateAiTestResultAsync(aiTestResult);

				// Update status
				testRequest.Status = "Completed";
				await _aiTestRepository.UpdateTestRequestAsync(testRequest);

				return new AiTestModel.AiTestResponseModel
				{
					TestRequestId = testRequest.Id,
					Status = testRequest.Status,
					CreatedDate = testRequest.CreatedDate.Value,
					Result = analysisResult
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error processing AI test {testRequestId}");
				testRequest.Status = "Failed";
				await _aiTestRepository.UpdateTestRequestAsync(testRequest);
				throw;
			}
		}

		public async Task<AiTestModel.AiTestResponseModel?> GetAiTestResultAsync(int testRequestId, int userId)
		{
			var testRequest = await _aiTestRepository.GetTestRequestByIdAsync(testRequestId);

			if (testRequest == null || testRequest.UserAccountId != userId)
			{
				return null;
			}

			var response = new AiTestModel.AiTestResponseModel
			{
				TestRequestId = testRequest.Id,
				Status = testRequest.Status ?? "Unknown",
				CreatedDate = testRequest.CreatedDate ?? DateTime.UtcNow
			};

			if (testRequest.AiTestResult != null)
			{
				response.Result = new AiTestResultModel
				{
					ColorType = testRequest.AiTestResult.ColorType.Name,
					ColorTypeId = testRequest.AiTestResult.ColorTypeId,
					SuggestedColor = testRequest.AiTestResult.SuggestedColor.Split(", ").ToList(),
					AvoidedColor = testRequest.AiTestResult.AvoidedColor.Split(", ").ToList()
				};
			}

			return response;
		}

		public async Task<List<AiTestModel.AiTestResponseModel>> GetUserAiTestsAsync(int userId)
		{
			var testRequests = await _aiTestRepository.GetTestRequestsByUserIdAsync(userId);

			return testRequests.Select(t => new AiTestModel.AiTestResponseModel
			{
				TestRequestId = t.Id,
				Status = t.Status ?? "Unknown",
				CreatedDate = t.CreatedDate ?? DateTime.UtcNow,
				Result = t.AiTestResult != null ? new AiTestResultModel
				{
					ColorType = t.AiTestResult.ColorType.Name,
					ColorTypeId = t.AiTestResult.ColorTypeId,
					SuggestedColor = t.AiTestResult.SuggestedColor.Split(", ").ToList(),
					AvoidedColor = t.AiTestResult.AvoidedColor.Split(", ").ToList()
				} : null
			}).ToList();
		}


		//================================================================

		public async Task<AiTestCompleteResponse> ProcessAiTestAsync2(AiTestCompleteRequest request)
		{
			try
			{
				_logger.LogInformation("Starting AI Test processing for TestRequestId: {TestRequestId}", request.TestRequestId);

				// 1. Verify test request exists and belongs to user
				var testRequest = await _testRequestRepository.GetByIdAsync(request.TestRequestId);
				if (testRequest == null)
				{
					throw new Exception($"Test request {request.TestRequestId} not found");
				}

				// 2. Validate images
				if (request.FaceImages == null || request.FaceImages.Count == 0)
				{
					throw new ArgumentException("At least one face image is required");
				}

				if (request.FaceImages.Count > 5)
				{
					throw new ArgumentException("Maximum 5 images allowed");
				}

				// 3. Upload images to Cloudinary
				var imageUrls = new List<string>();
				foreach (var image in request.FaceImages)
				{
					var imageUrl = await _imageUploadService.UploadImageAsync(image);
					imageUrls.Add(imageUrl);
					_logger.LogInformation("Uploaded image: {ImageUrl}", imageUrl);
				}

				// 4. Analyze colors from images using Gemini
				var analysisRequest = new Application.Models.AiTest.GeminiAnalysisRequest
				{
					ImageUrls = imageUrls,
					HairColor = request.HairColor,
					EyesColor = request.EyesColor,
					LipsColor = request.LipsColor,
					SkinColor = request.SkinColor
				};

				var colorAnalysis = await _geminiService.AnalyzeColorTypeAsync2(analysisRequest);
				_logger.LogInformation("Color analysis completed: {ColorType}", colorAnalysis.ColorType);

				// 5. Match hex codes to database colors
				var matchedSuggestedColors = await _colorMatchingService
					.MatchColorsFromHexCodesAsync(colorAnalysis.SuggestedColorHexCodes);

				var matchedAvoidedColors = await _colorMatchingService
					.MatchColorsFromHexCodesAsync(colorAnalysis.AvoidedColorHexCodes);

				_logger.LogInformation("Color matching completed. Suggested: {Count1}, Avoided: {Count2}",
					matchedSuggestedColors.Count, matchedAvoidedColors.Count);

				// 6. Generate virtual try-on images if requested
				VirtualTryOnResponse? virtualTryOnResults = null;
				if (request.GenerateVirtualTryOn && imageUrls.Count > 0)
				{
					var tryOnRequest = new VirtualTryOnRequest
					{
						UserImageUrl = imageUrls[0], // Use first uploaded image
						SuggestedColorHexCodes = colorAnalysis.SuggestedColorHexCodes
					};

					virtualTryOnResults = await _virtualTryOnService.GenerateVirtualTryOnImagesAsync(tryOnRequest);
					_logger.LogInformation("Virtual try-on generation completed: {Count} images",
						virtualTryOnResults.GeneratedImages.Count);
				}

				// 7. Save AiPictures to database
				var aiPictures = imageUrls.Select(url => new AiPicture
				{
					Source = url,
					Note = "AI Test Face Image",
					TestRequestId = request.TestRequestId
				}).ToList();

				await _aiTestRepository.CreateAiPicturesAsync(aiPictures);

				// 8. Save AI Test Result
				var aiTestResult = new AiTestResult
				{
					Id = request.TestRequestId,
					Date = DateTime.UtcNow,
					ColorTypeId = colorAnalysis.ColorTypeId,
					SuggestedColor = string.Join(", ", matchedSuggestedColors
						.Where(c => c.MatchedColor != null)
						.Select(c => c.MatchedColor!.Name)),
					AvoidedColor = string.Join(", ", matchedAvoidedColors
						.Where(c => c.MatchedColor != null)
						.Select(c => c.MatchedColor!.Name)),
					Note = $"Analysis completed by AI. Raw hex codes - Suggested: {string.Join(", ", colorAnalysis.SuggestedColorHexCodes)}, Avoided: {string.Join(", ", colorAnalysis.AvoidedColorHexCodes)}"
				};

				await _aiTestRepository.CreateAiTestResultAsync(aiTestResult);

				// 9. Update test request status
				testRequest.Status = "Completed";
				await _testRequestRepository.UpdateAsync(testRequest);

				// 10. Build response
				var response = new AiTestCompleteResponse
				{
					TestRequestId = request.TestRequestId,
					ColorAnalysis = colorAnalysis,
					MatchedSuggestedColors = matchedSuggestedColors,
					MatchedAvoidedColors = matchedAvoidedColors,
					VirtualTryOnResults = virtualTryOnResults,
					Status = "Completed"
				};

				_logger.LogInformation("AI Test processing completed successfully for TestRequestId: {TestRequestId}",
					request.TestRequestId);

				return response;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing AI Test for TestRequestId: {TestRequestId}", request.TestRequestId);

				// Update test request status to failed
				try
				{
					var testRequest = await _testRequestRepository.GetByIdAsync(request.TestRequestId);
					if (testRequest != null)
					{
						testRequest.Status = "Failed";
						await _testRequestRepository.UpdateAsync(testRequest);
					}
				}
				catch (Exception updateEx)
				{
					_logger.LogError(updateEx, "Failed to update test request status");
				}

				throw;
			}
		}

		public async Task<GeminiColorAnalysisResponse> AnalyzeColorsOnlyAsync(int testRequestId, Application.Models.AiTest.GeminiAnalysisRequest request)
		{
			try
			{
				_logger.LogInformation("Analyzing colors only for TestRequestId: {TestRequestId}", testRequestId);

				var colorAnalysis = await _geminiService.AnalyzeColorTypeAsync2(request);

				_logger.LogInformation("Color analysis completed: {ColorType}", colorAnalysis.ColorType);

				return colorAnalysis;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error analyzing colors for TestRequestId: {TestRequestId}", testRequestId);
				throw;
			}
		}

		public async Task<VirtualTryOnResponse> GenerateVirtualTryOnAsync(int testRequestId, VirtualTryOnRequest request)
		{
			try
			{
				_logger.LogInformation("Generating virtual try-on for TestRequestId: {TestRequestId}", testRequestId);

				var result = await _virtualTryOnService.GenerateVirtualTryOnImagesAsync(request);

				_logger.LogInformation("Virtual try-on generation completed: {Count} images", result.GeneratedImages.Count);

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating virtual try-on for TestRequestId: {TestRequestId}", testRequestId);
				throw;
			}
		}

	}
}
