using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.AiTest;
using PerHue.Application.Models.CapsulePalette;
using PerHue.Application.Models.Color;
using PerHue.Application.Models.ColorType;
using PerHue.Application.Models.TestRequest;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.AI;
using PerHue.Infrastructure.Policies;
using PerHue.Infrastructure.Repositories;
using PerHue.Infrastructure.Utils;
using Polly.Retry;
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
		private readonly IUserSubscriptionService _subscriptionService;
		private readonly IMapper _mapper;
		private readonly IUserService _userService;
		private readonly ICapsulePaletteService _capsulePaletteService;
		private readonly AsyncRetryPolicy _retryPolicy;

		public AiTestService(
			IAiTestResultRepository aiTestRepository,
			IAIImageAnalysisService geminiService,
			IImageUploadService imageUploadService,
			ILogger<AiTestService> logger,
			IColorMatchingService colorMatchingService,
			IVirtualTryOnService virtualTryOnService,
			ITestRequestRepository testRequestRepository,
			IUserSubscriptionService subscriptionService,
			IUserService userService, IMapper mapper,
			ICapsulePaletteService capsulePaletteService)
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
			_mapper = mapper;
			_capsulePaletteService = capsulePaletteService;
			// Khởi tạo retry policy với 3 lần thử
			_retryPolicy = RetryPolicies.CreateAiServiceRetryPolicy(logger, maxRetryAttempts: 3);
		}

		public async Task<AiTestModel.AiTestResponseModel?> GetAiTestResultAsync(int testRequestId, int userId)
		{
			var testRequest = await _aiTestRepository.GetTestRequestByIdAsync(testRequestId);
			var user = await _userService.GetByIdAsync(userId);

			if (testRequest == null || testRequest.UserAccountId != userId)
			{
				return null;
			}

			var pictureUrl = await _aiTestRepository.GetPictureUrlByTestRequestIdAsync(testRequest.Id);

			//var picture = await _imageUploadService.DownloadImageAsFormFileAsync(pictureUrl);

			var response = new AiTestModel.AiTestResponseModel
			{
				TestRequestId = testRequest.Id,
				Status = testRequest.Status ?? "Unknown",
				CreatedDate = testRequest.CreatedDate ?? DateTime.Now,
				UserAccountId = testRequest.UserAccountId,
				Fullname = user.Fullname,
				TypeOfTest = testRequest.TypeOfTest,
				ImageUrl = pictureUrl
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

			var pictureUrlTasks = testRequests.Select(async tr => new
			{
				TestRequest = tr,
				PictureUrl = await _aiTestRepository.GetPictureUrlByTestRequestIdAsync(tr.Id)
			});

			var testRequestsWithUrls = await Task.WhenAll(pictureUrlTasks);

			return testRequestsWithUrls.Select(item => new AiTestModel.AiTestResponseModel
			{
				TestRequestId = item.TestRequest.Id,
				Status = item.TestRequest.Status ?? "Unknown",
				CreatedDate = item.TestRequest.CreatedDate ?? DateTime.Now,
				UserAccountId = item.TestRequest.UserAccountId,
				Fullname = user.Fullname,
				TypeOfTest = item.TestRequest.TypeOfTest,
				ImageUrl = item.PictureUrl, // Trả về URL thay vì IFormFile
				Result = item.TestRequest.AiTestResult != null ? new AiTestResultModel
				{
					ColorType = item.TestRequest.AiTestResult.ColorType.Name,
					ColorTypeId = item.TestRequest.AiTestResult.ColorTypeId,
					SuggestedColor = item.TestRequest.AiTestResult.SuggestedColor.Split(", ").ToList(),
					AvoidedColor = item.TestRequest.AiTestResult.AvoidedColor.Split(", ").ToList()
				} : null
			}).ToList();
		}

		public async Task<List<NewTestRequestReponseModel>> GetListTestRequestByTypeAiAsync(int userId)
		{
			var userAccount = await _userService.GetByIdAsync(userId);
			if (userAccount == null)
			{
				throw new Exception("User not found");
			}

			var testRequestList = await _aiTestRepository.GetTestRequestsByUserIdAsync(userId);
			if (testRequestList == null || testRequestList.Count == 0)
			{
				return new List<NewTestRequestReponseModel>();
			}
			var result = testRequestList
				.OrderByDescending(tr => tr.CreatedDate)
				.Select(testRequest => new NewTestRequestReponseModel
				{
					Id = testRequest.Id,
					CreatedDate = testRequest.CreatedDate ?? DateTime.Now,
					ImageUrl = testRequest.Pictures?.FirstOrDefault()?.Source,
					ColorTypeId = testRequest.AiTestResult?.ColorTypeId ?? 0,
					ColorTypeName = testRequest.AiTestResult?.ColorType?.Name ?? string.Empty
				})
				.ToList();
			return result;
		}

		public async Task<NewTestRequestReponseModel> GetDetailTestRequestByTypeAiAsync(int testRequestId, int userId)
		{
			var userAccount = await _userService.GetByIdAsync(userId);
			if (userAccount == null)
			{
				throw new Exception("User not found");
			}

			var testRequest = await _aiTestRepository.GetTestRequestByIdAsync(testRequestId);
			if (testRequest == null)
			{
				throw new Exception("Test request not found");
			}

			if (testRequest.UserAccountId != userId)
			{
				throw new UnauthorizedAccessException("This test request does not belong to the specified user");
			}

			var pictureUrl = testRequest.Pictures?.FirstOrDefault()?.Source;

			var response = new NewTestRequestReponseModel
			{
				Id = testRequest.Id,
				HairColor = testRequest.HairColor,
				EyesColor = testRequest.EyesColor,
				LipsColor = testRequest.LipsColor,
				SkinColor = testRequest.SkinColor,
				Status = testRequest.Status ?? "Unknown",
				CreatedDate = testRequest.CreatedDate,
				TypeOfTest = testRequest.TypeOfTest,
				Fullname = userAccount.Fullname,
				ImageUrl = pictureUrl
			};

			if (testRequest.AiTestResult != null)
			{
				var aiTestResult = testRequest.AiTestResult;

				_logger.LogInformation("Processing AI test result for TestRequestId: {TestRequestId}", testRequestId);

				var suggestedHexCodes = aiTestResult.SuggestedColor
					.Split(", ", StringSplitOptions.RemoveEmptyEntries)
					.ToList();

				var matchedSuggestedColors = await _colorMatchingService
					.MatchColorsFromHexCodesAsync(suggestedHexCodes);

				// GET SYSTEM HEX CODES FOR PALETTE MATCHING
				var uniqueSystemColors = matchedSuggestedColors
					.Where(c => c.MatchedColor != null)
					.GroupBy(c => c.MatchedColor!.Id)
					.Select(g => g.First().MatchedColor!)
					.Select(c => new ColorModel
					{
						Id = c.Id,
						Name = c.Name,
						HexCode = c.HexCode
					})
					.ToList();

				// GET ONE RELATED CAPSULE PALETTE

				var capsulePalettes = new List<CapsulePaletteModel>();
				if (uniqueSystemColors.Any())
				{
					var systemHexCodes = uniqueSystemColors.Select(c => c.HexCode).ToList();
					var palettes = await _capsulePaletteService
						.GetRelativeCapsulePalettes(systemHexCodes);

					capsulePalettes = palettes.ToList();

					_logger.LogInformation(
						"Found {Count} related capsule palettes for TestRequestId {TestRequestId}",
						capsulePalettes.Count, testRequestId);
				}

				if (aiTestResult.ColorType != null)
				{
					response.ColorTypeId = aiTestResult.ColorType.Id;
					response.ColorTypeName = aiTestResult.ColorType.Name;
				}

				// BUILD COMPLETE AI TEST RESULT MODEL
				response.newAiTestResultResponseModel = new NewAiTestResultResponseModel
				{
					Id = aiTestResult.Id,
					Note = aiTestResult.Note,
					ColorTypeId = aiTestResult.ColorTypeId,
					SuggestedColor = aiTestResult.SuggestedColor,
					AvoidedColor = aiTestResult.AvoidedColor,
					SuggestedColorsBySystem = uniqueSystemColors,
					SuggestedCapsulePalletesBySystem = capsulePalettes
				};
			}
			else
			{
				// EMPTY RESULT IF NO AI TEST RESULT EXISTS
				_logger.LogWarning("No AI test result found for TestRequestId: {TestRequestId}", testRequestId);

				response.newAiTestResultResponseModel = new NewAiTestResultResponseModel();
			}

			_logger.LogInformation(
				"Successfully retrieved detailed test request {TestRequestId} for user {UserId}",
				testRequestId, userId);

			return response;
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
				CreatedDate = t.CreatedDate ?? DateTime.Now,
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
			var checkQuotaAndRateLimit = await _aiTestRepository.CountAsync(
				tr => tr.Date.HasValue &&
					  tr.Date.Value.Date == DateTime.UtcNow.Date);
			if (checkQuotaAndRateLimit == 20) // Giới hạn 20 requests/ngày
			{
				_logger.LogWarning("Daily AI test quota exceeded !!!");
				throw new InvalidOperationException("Sorry for this inconvenience! Please try again tomorrow");
			}

			_logger.LogInformation("Starting AI Test creation for UserId: {UserId}", userId);

			// KIỂM TRA VÀ TRỪ LƯỢT NGAY TẠI ĐÂY - TRƯỚC KHI BẮT ĐẦU QUY TRÌNH
			var userSubscription = await _subscriptionService.GetCurrentUserSubscriptionByUserIdAsync(userId);
			var hasRemaining = await _subscriptionService.HasRemainingUsageAsync(userId);
			if (!hasRemaining)
			{
				var remaining = await _subscriptionService.GetRemainingUsageAsync(userId);
				_logger.LogWarning($"User {userId} has insufficient remaining usage. Current: {remaining}");
				throw new InvalidOperationException($"You have no remaining AI test usage (Current: {remaining}). Please upgrade your subscription.");
			}

			var packageId = userSubscription.ServicePackageId;
			var packageType = userSubscription.ServicePackage.Type;

			var deducted = await _subscriptionService.DeductUsageAsync(userId, packageId, packageType);
			if (!deducted)
			{
				_logger.LogError("Failed to deduct usage for user {UserId}", userId);
				throw new InvalidOperationException("Failed to deduct usage. Please try again.");
			}

			try
			{
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
					CreatedDate = DateTime.Now,
					TypeOfTest = "AI Test",
					UserAccountId = userId
				};

				testRequest = await _aiTestRepository.CreateTestRequestAsync(testRequest);
				_logger.LogInformation("Created TestRequest with Id: {TestRequestId}", testRequest.Id);

				// Upload user images và lưu vào bảng Picture
				var imageUrls = new List<string>();
				var pictures = new List<Picture>();

				if (request.FaceImages != null)
				{
					var imageUrl = await _imageUploadService.UploadImageAsync(request.FaceImages);
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

					// ✅ LẤY CAPSULE PALETTES LIÊN QUAN DỰA VÀO SUGGESTED COLORS
					var relatedPalettes = await _capsulePaletteService
						.GetRelativeCapsulePalettes(colorAnalysis.SuggestedColorHexCodes);


					// Generate virtual try-on với IFormFile TRỰC TIẾP
					VirtualTryOnResponse? virtualTryOnResults = null;
					if (request.FaceImages != null)
					{
						_logger.LogInformation("Starting virtual try-on with retry policy for TestRequestId: {TestRequestId}", testRequest.Id);

						// SỬ DỤNG IFormFile TRỰC TIẾP
						var tryOnRequest = new VirtualTryOnRequest
						{
							UserImage = request.FaceImages,
							SuggestedColorHexCodes = colorAnalysis.SuggestedColorHexCodes
						};

						virtualTryOnResults = await _retryPolicy.ExecuteAsync(async () =>
							await _virtualTryOnService.GenerateVirtualTryOnImagesAsync(tryOnRequest)
						);
						_logger.LogInformation("Virtual try-on generation completed: {Count} images", virtualTryOnResults.GeneratedImages.Count);

						// Lưu ảnh AI tạo ra vào bảng AiPicture
						if (virtualTryOnResults.GeneratedImages.Count > 0)
						{
							var aiPictures = virtualTryOnResults.GeneratedImages.Select(img => new AiPicture
							{
								Source = img.ImageUrl,
								Note = $"{PictureNotes.AiGeneratedImage} - Colors: {img.ColorHex}",
								TestRequestId = testRequest.Id
							}).ToList();

							await _aiTestRepository.CreateAiPicturesAsync(aiPictures);
							_logger.LogInformation("Saved {Count} AI-generated images to AiPicture table", aiPictures.Count);
						}
					}

					// Lưu kết quả test vào AiTestResult
					var aiTestResult = new AiTestResult
					{
						Date = DateTime.Now,
						ColorTypeId = colorAnalysis.ColorTypeId,
						SuggestedColor = string.Join(", ", colorAnalysis.SuggestedColorHexCodes),
						AvoidedColor = string.Join(", ", colorAnalysis.AvoidedColorHexCodes),
						Note = $"Analysis completed by AI. Raw hex codes",
						IdNavigation = testRequest
					};

					var result = await _aiTestRepository.CreateAiTestResultAsync(aiTestResult);

					// Update status thành Completed
					testRequest.Status = TestStatus.Completed.ToString();
					await _aiTestRepository.UpdateTestRequestAsync(testRequest);

					// 1. Lấy hex codes từ matched colors (để tìm palettes)
					var suggestedHexCodes = matchedSuggestedColors
						.Where(c => c.MatchedColor != null)
						.Select(c => c.MatchedColor!.HexCode)
						.ToList();

					_logger.LogInformation("Finding related palettes for {Count} suggested colors: {Colors}",
						suggestedHexCodes.Count, string.Join(", ", suggestedHexCodes));

					// 2. Tìm capsule palettes dựa trên matched colors
					var relatedPalettesByColors = await _capsulePaletteService
						.GetRelativeCapsulePalettes(suggestedHexCodes);

					var relatedPalettesList = relatedPalettesByColors.ToList();

					_logger.LogInformation("Found {Count} related capsule palettes matching suggested colors",
						relatedPalettesList.Count);

					// 3. Map response
					var response = _mapper.Map<AiTestResultResponseModel>(result);

					response.ColorTypeName = result.ColorType.Name;

					response.SuggestedColorsBySystem = matchedSuggestedColors
						.Where(c => c.MatchedColor != null)
						.Select(c => new ColorModel
						{
							Id = c.MatchedColor!.Id,
							Name = c.MatchedColor.Name,
							HexCode = c.MatchedColor.HexCode
						})
						.ToList();

					// ✅ ASSIGN THE FIRST RELATED CAPSULE PALETTE TO THE RESPONSE
					response.SuggestedCapsulePalleteBySystem = relatedPalettesList.FirstOrDefault() ?? new CapsulePaletteModel();

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

					var refunded = await _subscriptionService.RefundUsageAsync(userId, packageId, packageType);
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

				var refunded = await _subscriptionService.RefundUsageAsync(userId, packageId, packageType);
				if (refunded)
				{
					_logger.LogInformation($"Refunded 1 usage for user {userId} due to processing error");
				}
				throw;
			}
		}

		public async Task<GeminiColorAnalysisResponse> AnalyzeColorsOnlyAsync(Application.Models.AiTest.GeminiAnalysisRequest request)
		{
			try
			{
				var colorAnalysis = await _geminiService.AnalyzeColorTypeAsync2(request);

				_logger.LogInformation("Color analysis completed: {ColorType}", colorAnalysis.ColorType);

				return colorAnalysis;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error analyzing colors");
				throw;
			}
		}

		public async Task<VirtualTryOnResponse> GenerateVirtualTryOnAsync(VirtualTryOnRequest request)
		{
			try
			{
				_logger.LogInformation("Generating virtual try-on");

				var result = await _retryPolicy.ExecuteAsync(async () =>
							await _virtualTryOnService.GenerateVirtualTryOnImagesAsync(request)
						);

				_logger.LogInformation("Virtual try-on generation completed: {Count} images", result.GeneratedImages);

				return result;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating virtual try-on");
				throw;
			}
		}
	}
}
