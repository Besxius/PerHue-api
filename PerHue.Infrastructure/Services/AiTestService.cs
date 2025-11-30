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
using PerHue.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PerHue.Application.Models.AiTestModel;
using PerHue.Infrastructure.Utils;
using AutoMapper;



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
		private readonly IUserSubscriptionService _subscriptionService;
		private readonly IMapper _mapper;
		private readonly IUserService _userService;

		public AiTestService(
			IAiTestResultRepository aiTestRepository,
			IAIImageAnalysisService geminiService,
			IImageUploadService imageUploadService,
			ILogger<AiTestService> logger,
			IColorMatchingService colorMatchingService,
			IVirtualTryOnService virtualTryOnService,
			ITestRequestRepository testRequestRepository,
			IUserSubscriptionService subscriptionService,
			IUserService userService)
		{
			_aiTestRepository = aiTestRepository;
			_geminiService = geminiService;
			_imageUploadService = imageUploadService;
			_logger = logger;
			_colorMatchingService = colorMatchingService;
			_virtualTryOnService = virtualTryOnService;
			_testRequestRepository = testRequestRepository;
			_subscriptionService = subscriptionService;
			_userService = userService;
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
			var user = await _userService.GetByIdAsync(userId);

			if (testRequest == null || testRequest.UserAccountId != userId)
			{
				return null;
			}

			var response = new AiTestModel.AiTestResponseModel
			{
				TestRequestId = testRequest.Id,
				Status = testRequest.Status ?? "Unknown",
				CreatedDate = testRequest.CreatedDate ?? DateTime.UtcNow,
				UserAccountId = testRequest.UserAccountId,
				Fullname = user.Fullname,
				TypeOfTest = testRequest.TypeOfTest
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
			var user = await _userService.GetByIdAsync(userId);

			return testRequests.Select(testRequest => new AiTestModel.AiTestResponseModel
			{
				TestRequestId = testRequest.Id,
				Status = testRequest.Status ?? "Unknown",
				CreatedDate = testRequest.CreatedDate ?? DateTime.UtcNow,
				UserAccountId = testRequest.UserAccountId,
				Fullname = user.Fullname,
				TypeOfTest = testRequest.TypeOfTest,
				Result = testRequest.AiTestResult != null ? new AiTestResultModel
				{
					ColorType = testRequest.AiTestResult.ColorType.Name,
					ColorTypeId = testRequest.AiTestResult.ColorTypeId,
					SuggestedColor = testRequest.AiTestResult.SuggestedColor.Split(", ").ToList(),
					AvoidedColor = testRequest.AiTestResult.AvoidedColor.Split(", ").ToList()
				} : null
			}).ToList();
		}

		public async Task<PaginatedResultV2<AiTestModel.AiTestResponseModel>> GetAiTestsWithFilterAsync(AiTestSearchModel searchModel)
		{
			var (testRequests, totalCount) = await _aiTestRepository.GetFilteredTestRequestsAsync(
				searchModel.PageIndex,
				searchModel.PageSize,
				searchModel.UserId,
				searchModel.Status,
				searchModel.TypeOfTest,
				searchModel.Fullname,
				searchModel.StartDate,
				searchModel.EndDate,
				(int?)searchModel.SortBy,
				(int?)searchModel.SortOrder
			);

			var aiTestResponses = testRequests.Select(t => new AiTestModel.AiTestResponseModel
			{
				TestRequestId = t.Id,
				Status = t.Status ?? "Unknown",
				CreatedDate = t.CreatedDate ?? DateTime.UtcNow,
				Fullname = t.UserAccount.Fullname,
				UserAccountId = t.UserAccount.Id,
				TypeOfTest = t.TypeOfTest,
				Result = t.AiTestResult != null ? new AiTestResultModel
				{
					ColorType = t.AiTestResult.ColorType.Name,
					ColorTypeId = t.AiTestResult.ColorTypeId,
					SuggestedColor = [.. t.AiTestResult.SuggestedColor.Split(", ")],
					AvoidedColor = [.. t.AiTestResult.AvoidedColor.Split(", ")]
				} : null
			}).ToList();

			return new PaginatedResultV2<AiTestModel.AiTestResponseModel>
			{
				List = aiTestResponses,
				Total = totalCount,
				Current = searchModel.PageIndex
			};
		}

		public async Task<bool> MarkTestAsCompletedAsync(int testId)
		{
			var testRequest = await _aiTestRepository.GetTestRequestByIdAsync(testId) ?? throw new Exception("Test request not found");
			if (testRequest.Status == "completed")
			{
				return false; // Already completed
			}

			testRequest.Status = "completed";
			await _aiTestRepository.UpdateTestRequestAsync(testRequest);
			return true;
		}

		//================================================================

		public async Task<AiTestResultResponseModel> ProcessAiTestAsync2(int userId, AiTestCompleteRequest request)
		{
			try
			{
				// Validate images TRƯỚC KHI TRỪ LƯỢT
				if (request.FaceImages == null || request.FaceImages.Count == 0)
				{
					throw new ArgumentException("At least one face image is required");
				}

				if (request.FaceImages.Count > 1)
				{
					throw new ArgumentException("Only one face image is allowed");
				}

				_logger.LogInformation("Starting AI Test creation for UserId: {UserId}", userId);

				// KIỂM TRA VÀ TRỪ LƯỢT NGAY TẠI ĐÂY - TRƯỚC KHI BẮT ĐẦU QUY TRÌNH
				var hasRemaining = await _subscriptionService.HasRemainingUsageAsync(userId);
				if (!hasRemaining)
				{
					var remaining = await _subscriptionService.GetRemainingUsageAsync(userId);
					_logger.LogWarning($"User {userId} has insufficient remaining usage. Current: {remaining}");
					throw new InvalidOperationException($"You have no remaining AI test usage (Current: {remaining}). Please upgrade your subscription.");
				}

				var deducted = await _subscriptionService.DeductUsageAsync(userId, isFromExpertTest: false);
				if (!deducted)
				{
					_logger.LogError("Failed to deduct usage for user {UserId}", userId);
					throw new InvalidOperationException("Failed to deduct usage. Please try again.");
				}
				var remainingAfterDeduct = await _subscriptionService.GetRemainingUsageAsync(userId);
				_logger.LogInformation($"Successfully deducted 1 usage for user {userId}. Remaining: {remainingAfterDeduct}", userId, remainingAfterDeduct);

				// Tạo TestRequest mới
				var testRequest = new TestRequest
				{
					HairColor = request.HairColor,
					EyesColor = request.EyesColor,
					LipsColor = request.LipsColor,
					SkinColor = request.SkinColor,
					Status = TestStatus.Processing.ToString(),
					CreatedDate = DateTime.UtcNow,
					TypeOfTest = "AI Test",
					UserAccountId = userId
				};

				testRequest = await _aiTestRepository.CreateTestRequestAsync(testRequest);
				_logger.LogInformation("Created TestRequest with Id: {TestRequestId}", testRequest.Id);

				// Upload user images và lưu vào bảng Picture
				var imageUrls = new List<string>();
				var pictures = new List<Picture>();

				foreach (var image in request.FaceImages)
				{
					var imageUrl = await _imageUploadService.UploadImageAsync(image);
					imageUrls.Add(imageUrl);
					_logger.LogInformation("Uploaded user image: {ImageUrl}", imageUrl);

					pictures.Add(new Picture
					{
						Source = imageUrl,
						TestRequestId = testRequest.Id
					});
				}

				// Lưu ảnh người dùng vào bảng Picture
				await _aiTestRepository.CreatePicturesAsync(pictures);
				_logger.LogInformation("Saved {Count} user images to Picture table", pictures.Count);

				// Xử lý AI analysis
				try
				{
					// Analyze colors với Gemini
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

					// Match colors
					var matchedSuggestedColors = await _colorMatchingService
						.MatchColorsFromHexCodesAsync(colorAnalysis.SuggestedColorHexCodes);

					var matchedAvoidedColors = await _colorMatchingService
						.MatchColorsFromHexCodesAsync(colorAnalysis.AvoidedColorHexCodes);

					_logger.LogInformation("Color matching completed. Suggested: {Count1}, Avoided: {Count2}",
						matchedSuggestedColors.Count, matchedAvoidedColors.Count);

					// Generate virtual try-on với IFormFile TRỰC TIẾP
					VirtualTryOnResponse? virtualTryOnResults = null;
					if (request.FaceImages.Count > 0)
					{
						// SỬ DỤNG IFormFile TRỰC TIẾP
						var tryOnRequest = new VirtualTryOnRequest
						{
							UserImage = request.FaceImages[0],
							SuggestedColorHexCodes = colorAnalysis.SuggestedColorHexCodes
						};

						virtualTryOnResults = await _virtualTryOnService.GenerateVirtualTryOnImagesAsync(tryOnRequest);
						_logger.LogInformation("Virtual try-on generation completed: {Count} images",
							virtualTryOnResults.GeneratedImages.Count);

						// Lưu ảnh AI tạo ra vào bảng AiPicture
						if (virtualTryOnResults.GeneratedImages.Count > 0)
						{
							var aiPictures = virtualTryOnResults.GeneratedImages.Select(img => new AiPicture
							{
								Source = img.ImageUrl,
								Note = $"{PictureNotes.AiGeneratedImage} - {img.Environment} - {img.ClothingType} - Colors: {img.ColorHex}",
								TestRequestId = testRequest.Id
							}).ToList();

							await _aiTestRepository.CreateAiPicturesAsync(aiPictures);
							_logger.LogInformation("Saved {Count} AI-generated images to AiPicture table", aiPictures.Count);
						}
					}

					// Lưu kết quả test vào AiTestResult
					var aiTestResult = new AiTestResult
					{
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

					var result = await _aiTestRepository.CreateAiTestResultAsync(aiTestResult);

					// Update status thành Completed
					testRequest.Status = TestStatus.Completed.ToString();
					await _aiTestRepository.UpdateTestRequestAsync(testRequest);

					var response = _mapper.Map<AiTestResultResponseModel>(result);

					_logger.LogInformation("AI Test processing completed successfully for TestRequestId: {TestRequestId}",
						testRequest.Id);

					return response;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error processing AI test for request {TestRequestId}", testRequest.Id);

					// Update status thành Failed nếu có lỗi
					testRequest.Status = TestStatus.Failed.ToString();
					await _aiTestRepository.UpdateTestRequestAsync(testRequest);

					var refunded = await _subscriptionService.RefundUsageAsync(userId);
					if (refunded)
					{
						_logger.LogInformation($"Refunded 1 usage for user {userId} due to processing error");
					}

					throw;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating AI Test for UserId: {UserId}", userId);
				var refunded = await _subscriptionService.RefundUsageAsync(userId);
				if (refunded)
				{
					_logger.LogInformation($"Refunded 1 usage for user {userId} due to processing error");
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
