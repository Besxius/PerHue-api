using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.AiTest;
using PerHue.Application.Models.CapsulePalette;
using PerHue.Application.Models.Color;
using PerHue.Application.Models.ExpertTestResult;
using PerHue.Application.Models.ManualTest;
using PerHue.Application.Models.TestRequest;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.AI;
using PerHue.Infrastructure.Utils;
using System;
using System.Linq;

namespace PerHue.Infrastructure.Services
{
	public class TestResultService : ITestResultService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly GeminiService _gemini;
		ILogger<TestResultService> _logger;
		private readonly IImageUploadService _imageUploadService;
		private readonly IVirtualTryOnService _virtualTryOnService; // Renamed from _imageGenerationService for clarity
		private readonly IHttpClientFactory _httpClientFactory;

		public TestResultService(IUnitOfWork unitOfWork, IMapper mapper, GeminiService gemini, ILogger<TestResultService> logger, IImageUploadService imageUploadService, IVirtualTryOnService virtualTryOnService,
			IHttpClientFactory httpClientFactory)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_gemini = gemini;
			_logger = logger;
			_imageUploadService = imageUploadService;
			_virtualTryOnService = virtualTryOnService;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var testResult = await _unitOfWork.TestResultRepository.GetByIdAsync(id);
			return await _unitOfWork.TestResultRepository.RemoveAsync(testResult);
		}

		public async Task<IEnumerable<TestResultModel>> GetAllAsync()
		{
			var testResults = await _unitOfWork.TestResultRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<TestResultModel>>(testResults);
		}

		public async Task<IEnumerable<TestResultModel>> GetAllAsyncByUserId(int userId)
		{
			var testResults = await _unitOfWork.TestResultRepository.GetAllByUserIdAsync(userId);
			var resultList = new List<TestResultModel>();

			foreach (var testResult in testResults)
			{
				var result = _mapper.Map<TestResultModel>(testResult);

				resultList.Add(result);
			}

			return resultList;
		}

		public async Task<TestResultModel> GetByTestResultIdAsync(int id)
		{
			var testResult = await _unitOfWork.TestResultRepository.GetByTestResultIdAsync(id);
			if (testResult == null)
			{
				return null;
			}

			var result = _mapper.Map<TestResultModel>(testResult);

			var capsulePalettes = await _unitOfWork.CapsulePaletteRepository.GetByColorTypeIdAsync(testResult.ColorTypeId);
			var colors = await _unitOfWork.ColorRepository.GetByColorTypeIdAsync(testResult.ColorTypeId);

			result.CapsulePalettes = _mapper.Map<List<CapsulePaletteModel>>(capsulePalettes);
			result.Colors = _mapper.Map<List<ColorModel>>(colors);

			return result;
		}

		public async Task<TestResultModel> GetByIdAsync(int id)
		{
			var testResult = await _unitOfWork.TestResultRepository.GetByIdAsync(id);
			return _mapper.Map<TestResultModel>(testResult);
		}

		//public async Task<TestResultModel> GetNormalTestSimpleColorResult(CreateManualTestResultModel model)
		//{
		//	var capsulePalettes = await _unitOfWork.CapsulePaletteRepository.GetRelativeCapsulePalettes(model.SelectedColors);

		//	var entity = new TestResult
		//	{
		//		UserId = model.UserId,
		//		User = await _unitOfWork.UserRepository.GetByIdAsync(model.UserId),
		//		Picture = "",
		//	};
		//	// code fix

		//	return _mapper.Map<TestResultModel>(entity);
		//}

		public async Task<TestResultModel> GetNormalTestSimpleColorResult(CreateManualTestResultModel model)
		{
			//Validate input colors
			var validatedColors = model.SelectedColors
				.Where(c => !string.IsNullOrEmpty(c) && c.StartsWith("#") && c.Length == 7)
				.Select(c => ColorCalculationHelper.NormalizeHexCode(c))
				.ToList();

			if (!validatedColors.Any())
			{
				throw new ArgumentException("No valid hex colors provided");
			}

			//Tìm màu tương tự trong database với scoring
			var colorMatches = new List<(string InputHex, Color DbColor, double Score)>();

			foreach (var inputHex in validatedColors)
			{
				var inputRgb = ColorCalculationHelper.HexToRgb(inputHex);
				if (!inputRgb.HasValue) continue;

				var allColors = await _unitOfWork.ColorRepository.GetAllAsync();

				foreach (var dbColor in allColors)
				{
					var dbRgb = ColorCalculationHelper.HexToRgb(dbColor.HexCode);
					if (!dbRgb.HasValue) continue;

					var deltaE = ColorCalculationHelper.CalculateDeltaE(inputRgb.Value, dbRgb.Value);

					// Chỉ lấy màu có Delta E <= 30 (tương đối gần)
					if (deltaE <= 30)
					{
						var relevanceScore = ColorCalculationHelper.CalculateRelevanceScore(
							inputRgb.Value,
							dbRgb.Value
						);

						colorMatches.Add((inputHex, dbColor, relevanceScore));
					}
				}
			}

			//Lấy top matches cho mỗi input color
			var bestMatches = colorMatches
				.GroupBy(m => m.InputHex)
				.SelectMany(g => g.OrderByDescending(m => m.Score))
				.ToList();

			if (!bestMatches.Any())
			{
				throw new InvalidOperationException("No matching colors found in database");
			}

			var matchedColorIds = bestMatches.Select(m => m.DbColor.Id).Distinct().ToList();

			//Tìm Capsule Palettes liên quan
			var capsulePalettes = await _unitOfWork.CapsulePaletteRepository.GetListByColorsIdAsync(matchedColorIds);
			var capsulePalettesList = capsulePalettes.ToList();

			//Xác định Color Type phổ biến nhất  (Chỉ 1 ColorType)
			var colorTypeGroups = capsulePalettesList
				.GroupBy(cp => cp.ColorTypeId)
				.Select(g => new
				{
					ColorTypeId = g.Key,
					Count = g.Count(),
					AverageScore = bestMatches
						.Where(m => g.Any(p => p.Colors.Any(c => c.Id == m.DbColor.Id)))
						.Average(m => m.Score)
				})
				.OrderByDescending(g => g.Count)
				.ThenByDescending(g => g.AverageScore)
				.ToList();

			var dominantColorType = colorTypeGroups.FirstOrDefault();
			if (dominantColorType == null)
			{
				throw new InvalidOperationException("Could not determine color type");
			}

			var colorType = await _unitOfWork.ColorTypeRepository.GetByIdAsync(dominantColorType.ColorTypeId);
			var colorsForColorType = await _unitOfWork.ColorRepository.GetByColorTypeIdAsync(dominantColorType.ColorTypeId);
			var colorsList = colorsForColorType.ToList();

			//Lọc ra các màu matched thuộc ColorType này
			var matchedColorsInColorType = bestMatches
				.Where(m => colorsList.Any(c => c.Id == m.DbColor.Id))
				.Select(m => m.DbColor)
				.DistinctBy(c => c.Id)
				.ToList();

			//Lấy top 5 capsule palettes của ColorType
			var filteredPalettes = capsulePalettesList
				.Where(cp => cp.ColorTypeId == dominantColorType.ColorTypeId)
				.DistinctBy(cp => cp.Id)
				.ToList();

			//Format ChosenColor và SuggestedColor
			var chosenColorString = string.Join(",", validatedColors);

			//SuggestedColor: Lấy top màu matched + thêm một số màu nổi bật từ ColorType
			var suggestedColors = matchedColorsInColorType
				.Select(c => c.HexCode)
				.ToList();

			// Thêm màu từ palettes nếu chưa đủ 15 màu
			//if (suggestedColors.Count < 15)
			//{
			//	var additionalColors = filteredPalettes
			//		.SelectMany(cp => cp.Colors)
			//		.DistinctBy(c => c.Id)
			//		.Where(c => !suggestedColors.Contains(c.HexCode))
			//		.Take(15 - suggestedColors.Count)
			//		.Select(c => c.HexCode);

			//	suggestedColors.AddRange(additionalColors);
			//}

			var suggestedColorString = string.Join(",", suggestedColors);
			
			var entity = new TestResult
			{
				UserId = model.UserId,
				CreatedDate = DateTime.Now,
				ChosenColor = chosenColorString,
				SuggestedColor = suggestedColorString,
				ColorTypeId = dominantColorType.ColorTypeId
			};

			//Check uploaded picture
			if (model.Picture != null)
			{
				try
				{
					var modelPictureLink = await _imageUploadService.UploadImageAsync(model.Picture);
					_logger.LogInformation($"Image uploaded successfully: {modelPictureLink}");
					entity.Picture = modelPictureLink; // Gán URL sau khi upload
				}
				catch (Exception ex)
				{
					_logger.LogError($"Failed to upload image: {ex.Message}");
					// Quyết định: throw exception hoặc tiếp tục với Picture = ""
				}
			}
			else
			{
				_logger.LogWarning("No picture provided or picture is empty");
			}

			await _unitOfWork.TestResultRepository.CreateAsync(entity);

			await _unitOfWork.SaveChangesWithTransactionAsync();

			var savedEntity = await _unitOfWork.TestResultRepository.GetTestResultDetailByIdWithUserAndColorTypeAsync(entity.Id);

			var result = _mapper.Map<TestResultModel>(savedEntity);
			result.CapsulePalettes = _mapper.Map<List<CapsulePaletteModel>>(filteredPalettes);
			result.Colors = _mapper.Map<List<ColorModel>>(matchedColorsInColorType);

			return result;
		}

		public async Task<TestResultModel> GetNormalTestCapsulePaletteResult(CreateManualTestResultModel model)
		{
			var capsulePalettes = await _unitOfWork.CapsulePaletteRepository.GetRelativeCapsulePalettes(model.SelectedColors, model.ColorType);

			// code fix
			var entity = new TestResult
			{
				UserId = model.UserId,
				User = await _unitOfWork.UserRepository.GetByIdAsync(model.UserId),
				Picture = "",
			};

			return _mapper.Map<TestResultModel>(entity);
		}

		public async Task<string> GetAiTestUploadImageResult(AiTestUploadImageModel model)
		{
			var result = await _gemini.GeneratePromptWithImageFromUrl(model.ImageUrl);
			return result;
		}

		/*public async Task<TestRequestModel> CreateExpertTestRequestAsync(int userId, string imageUrl)
		{
			// 1. Get user to link
			var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
			if (user == null)
			{
				throw new Exception("User not found");
			}

			// 2. Use AI to pick color info and generate scenario photos (Nodes 18 & 24)
			// This requires a different prompt than the one in GeminiService.
			// For now, we'll simulate this by calling the *existing* AI.
			// A "real" implementation would need a new Gemini prompt.
			_logger.LogWarning("Simulating 'Pick color info' and 'Generate scenarios'. Using standard AI test as placeholder.");

			var aiJsonResult = await _gemini.GeneratePromptWithImageFromUrl(imageUrl);
			var aiModel = JsonSerializer.Deserialize<AiTestResultModel>(aiJsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			// 3. Create the TestRequest (Node 22)
			var testRequest = new TestRequest
			{
				// We'll use the AI's "suggestion" as the "skin/hair" color for this example
				SkinColor = aiModel.SuggestedColor.FirstOrDefault(),
				HairColor = aiModel.SuggestedColor.LastOrDefault(),
				Status = "Pending", // Pending expert review
				CreatedDate = DateTime.Now,
				TypeOfTest = "Expert",
				UserAccountId = userId
			};

			// 4. Add the "scenario photos"
			// We will add the original image and simulated "scenario" images
			testRequest.AiPictures.Add(new AiPicture { Source = imageUrl, Note = "Original Photo" });
			foreach (var color in aiModel.SuggestedColor)
			{
				// In a real app, you'd call an image generation AI here.
				// We'll just add the hex code as a note.
				testRequest.AiPictures.Add(new AiPicture { Source = imageUrl, Note = $"Simulated scenario for {color}" });
			}

			await _unitOfWork.TestRequestRepository.CreateAsync(testRequest);
			await _unitOfWork.SaveChangesWithTransactionAsync(); // Save to get TestRequest.Id

			// 5. Send to Experts (Node 44)
			// Find 3 experts to send this to.
			var allExperts = await _unitOfWork.ExpertRepository.GetAllAsync();
			var expertsToRequest = allExperts.Take(3); // Simple logic, needs improvement (e.g., random, least busy)

			foreach (var expert in expertsToRequest)
			{
				var expertRequest = new ExpertTestRequest
				{
					ExpertId = expert.Id,
					TestRequestId = testRequest.Id,
					Status = "Pending",
					CreatedDate = DateTime.Now
				};
				await _unitOfWork.ExpertTestRequestRepository.CreateAsync(expertRequest);
			}

			await _unitOfWork.SaveChangesWithTransactionAsync();

			return _mapper.Map<TestRequestModel>(testRequest);
		}*/
		public async Task<TestRequestModel> CreateExpertTestRequestAsync(ExpertTestCreationParameters parameters)
		{
			// 1. CHECK SUBSCRIPTION (EXPERT TYPE ONLY)
			var activeSubscription = await _unitOfWork.UserSubscriptionRepository
				.GetActiveSubscriptionByTypeAsync(parameters.UserId, ServicePackageTypeEnum.Expert.ToString());

			if (activeSubscription == null)
			{
				throw new InvalidOperationException("You do not have an active EXPERT subscription or have run out of uses. Please purchase an Expert package.");
			}

			_logger.LogInformation($"Creating expert test request for user {parameters.UserId} with image {parameters.ImageUrl}");

			// 2. Create the TestRequest
			var testRequest = new TestRequest
			{
				SkinColor = parameters.SkinColor,
				HairColor = parameters.HairColor,
				EyesColor = parameters.EyesColor,
				LipsColor = parameters.LipsColor,
				Status = TestRequestStatus.Pending.ToString(),
				CreatedDate = DateTime.Now,
				TypeOfTest = "Expert",
				UserAccountId = parameters.UserId
			};

			// 3. Add the user's uploaded picture
			testRequest.AiPictures.Add(new AiPicture { Source = parameters.ImageUrl, Note = "Original Photo" });
			testRequest.Pictures.Add(new Picture { Source = parameters.ImageUrl });

			/*try
			{
				// STEP A: Prepare the Image File
				var httpClient = _httpClientFactory.CreateClient();
				var imageBytes = await httpClient.GetByteArrayAsync(parameters.ImageUrl);

				using var stream = new MemoryStream(imageBytes);
				var userImageFile = new FormFile(stream, 0, imageBytes.Length, "image", "temp_input.jpg")
				{
					Headers = new HeaderDictionary(),
					ContentType = "image/jpeg"
				};

				// STEP B: Prepare the Request Object
				// FIX: Since parameters doesn't have SuggestedColorHexCodes, 
				// we use a default set of neutral colors for the initial try-on.
				var defaultColors = new List<string> { "#FFFFFF", "#000000", "#808080" }; // White, Black, Grey

				var aiRequest = new VirtualTryOnRequest
				{
					UserImage = userImageFile,
					SuggestedColorHexCodes = defaultColors,
					Environments = new List<string> { "outdoor_sunny" }
				};

				// STEP C: Call the Service
				// Make sure _virtualTryOnService is injected in the constructor!
				var aiResponse = await _virtualTryOnService.GenerateVirtualTryOnImagesAsync(aiRequest);

				// STEP D: Add result to list
				if (aiResponse.GeneratedImages != null && aiResponse.GeneratedImages.Any())
				{
					var resultImage = aiResponse.GeneratedImages.First();
					testRequest.AiPictures.Add(new AiPicture
					{
						Source = resultImage.ImageUrl,
						Note = "AI Generated (Outdoor Sunny)"
					});
				}
			}
			catch (Exception ex)
			{
				// Optional: Handle generation failure without breaking the whole request
				_logger.LogError(ex, "Failed to generate outdoor_sunny AI image.");
			}*/
			await _unitOfWork.TestRequestRepository.CreateAsync(testRequest);

			// 4. DEDUCT USAGE FROM SUBSCRIPTION
			activeSubscription.RemainingUses--;
			if (activeSubscription.RemainingUses <= 0)
			{
				activeSubscription.RemainingUses = 0;
				activeSubscription.Status = false; // Deactivate if no uses left
			}
			await _unitOfWork.UserSubscriptionRepository.UpdateAsync(activeSubscription);

			// 5. Save Changes (Transactional)
			// Both the new request and the subscription update will be saved together
			await _unitOfWork.SaveChangesWithTransactionAsync();

			_logger.LogInformation($"Created TestRequest with ID {testRequest.Id}. User {parameters.UserId} remaining uses: {activeSubscription.RemainingUses}");

			// 6. Send to Experts (Randomly)
			var random = new Random();
			var allExperts = await _unitOfWork.ExpertRepository.GetAllAsync();
			var expertsToRequest = allExperts.Where(e => e.Id != parameters.UserId)
											 .OrderBy(e => random.Next())
											 .Take(3);

			foreach (var expert in expertsToRequest)
			{
				var expertRequest = new ExpertTestRequest
				{
					ExpertId = expert.Id,
					TestRequestId = testRequest.Id,
					Status = ExpertTestRequestStatus.Pending.ToString(),
					CreatedDate = DateTime.Now
				};
				await _unitOfWork.ExpertTestRequestRepository.CreateAsync(expertRequest);

				_logger.LogInformation($"Assigned request {testRequest.Id} to expert {expert.Id}");
				var notification = new Notification
				{
					Title = "New Test Request",
					Content = "You have received a new color analysis request.",
					Receiver = expert.Id, // Expert ID corresponds to UserAccountId
					TestRequestId = testRequest.Id,
					ReceivedTime = DateTime.Now,
					IsRead = false,
					Type = "TestRequest"
				};
				await _unitOfWork.NotificationRepository.CreateAsync(notification);
			}
			await _unitOfWork.SaveChangesWithTransactionAsync();

			return _mapper.Map<TestRequestModel>(testRequest);
		}
		public async Task RequestReviewAsync(int testRequestId, int userId)
		{
			// 1. Validate Request
			var testRequest = await _unitOfWork.TestRequestRepository.GetByIdWithDetailsAsync(testRequestId);
			if (testRequest == null) throw new Exception("Test request not found.");
			if (testRequest.UserAccountId != userId) throw new UnauthorizedAccessException("Unauthorized.");
			if (testRequest.Status != "Completed") throw new Exception("Can only review completed tests.");

			// 2. Check if review already requested (limit 1 review)
			var existingReviewResponse = testRequest.TestResponses.Any(tr => tr.Type == ResponseTypeEnum.Review.ToString());
			if (existingReviewResponse) throw new InvalidOperationException("A review has already been completed for this test.");

			var expertRequests = await _unitOfWork.ExpertTestRequestRepository.GetRequestsByTestIdAsync(testRequestId);
			// Filter out EXPIRED requests before counting. 
			// Counting valid (Pending, Completed, Reviewing) requests.
			// -----------------------------------------------------------------------
			var validExpertRequests = expertRequests
				.Where(er => er.Status != ExpertTestRequestStatus.Expired.ToString())
				.ToList();

			// Check if 4th valid request exists (3 original + 1 review)
			if (validExpertRequests.Count >= 4)
			{
				throw new InvalidOperationException("A review request has already been submitted for this test.");
			}

			// 3. Find a 4th Random Expert (who hasn't participated)
			var usedExpertIds = expertRequests.Select(er => er.ExpertId).ToList();
			var allExperts = await _unitOfWork.ExpertRepository.GetAllAsync();

			var availableExperts = allExperts.Where(e => !usedExpertIds.Contains(e.Id) && e.Id != userId).ToList();

			if (!availableExperts.Any()) throw new Exception("No additional experts available to review this request.");

			var random = new Random();
			var selectedExpert = availableExperts[random.Next(availableExperts.Count)];

			// 4. Create new ExpertTestRequest
			var newRequest = new ExpertTestRequest
			{
				ExpertId = selectedExpert.Id,
				TestRequestId = testRequestId,
				Status = ExpertTestRequestStatus.PendingReview.ToString(),
				CreatedDate = DateTime.Now
			};

			await _unitOfWork.ExpertTestRequestRepository.CreateAsync(newRequest);

			// --- STATUS CHANGE: Mark main request as Reviewing ---
			testRequest.Status = TestRequestStatus.Reviewing.ToString();
			await _unitOfWork.TestRequestRepository.UpdateAsync(testRequest);

			// --- NOTIFICATION: For the 4th Expert ---
			var notification = new Notification
			{
				Title = "Review Request",
				Content = "You have been selected to review and vote on a completed test.",
				Receiver = selectedExpert.Id,
				TestRequestId = testRequestId,
				ReceivedTime = DateTime.Now,
				IsRead = false,
				Type = "ReviewRequest"
			};
			await _unitOfWork.NotificationRepository.CreateAsync(notification);

			await _unitOfWork.SaveChangesWithTransactionAsync();
		}

		public async Task<TestResultModel> CreateNormalTestSimpleColorResult(TestResultModel model)
		{
			var entity = _mapper.Map<TestResult>(model);

			// code fix

			return _mapper.Map<TestResultModel>(entity);
		}

		public async Task<TestResultModel> CreateNormalTestCapsulePaletteResult(TestResultModel model)
		{
			var entity = _mapper.Map<TestResult>(model);

			// code fix

			return _mapper.Map<TestResultModel>(entity);
		}
	}
}
