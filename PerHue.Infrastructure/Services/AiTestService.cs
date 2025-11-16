using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PerHue.Application.IServices;
using PerHue.Application.Models;
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

		public AiTestService(
			IAiTestResultRepository aiTestRepository,
			IAIImageAnalysisService geminiService,
			IImageUploadService imageUploadService,
			ILogger<AiTestService> logger)
		{
			_aiTestRepository = aiTestRepository;
			_geminiService = geminiService;
			_imageUploadService = imageUploadService;
			_logger = logger;
		}

		public async Task<AiTestResponseModel> CreateAiTestRequestAsync(int userId, CreateAiTestRequestModel model)
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
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error processing AI test for request {testRequest.Id}");
				testRequest.Status = "Failed";
				await _aiTestRepository.UpdateTestRequestAsync(testRequest);
			}

			return new AiTestResponseModel
			{
				TestRequestId = testRequest.Id,
				Status = testRequest.Status,
				CreatedDate = testRequest.CreatedDate.Value
			};
		}

		public async Task<AiTestResponseModel> ProcessAiTestAsync(int testRequestId)
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
				var analysisRequest = new GeminiAnalysisRequest
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
					SuggestedColor = string.Join(", ", analysisResult.SuggestedColor),
					AvoidedColor = string.Join(", ", analysisResult.AvoidedColor),
					ColorTypeId = analysisResult.ColorTypeId,
					Note = "Analysis completed by AI"
				};

				await _aiTestRepository.CreateAiTestResultAsync(aiTestResult);

				// Update status
				testRequest.Status = "Completed";
				await _aiTestRepository.UpdateTestRequestAsync(testRequest);

				return new AiTestResponseModel
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

		public async Task<AiTestResponseModel?> GetAiTestResultAsync(int testRequestId, int userId)
		{
			var testRequest = await _aiTestRepository.GetTestRequestByIdAsync(testRequestId);

			if (testRequest == null || testRequest.UserAccountId != userId)
			{
				return null;
			}

			var response = new AiTestResponseModel
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

		public async Task<List<AiTestResponseModel>> GetUserAiTestsAsync(int userId)
		{
			var testRequests = await _aiTestRepository.GetTestRequestsByUserIdAsync(userId);

			return testRequests.Select(t => new AiTestResponseModel
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
	}
}
