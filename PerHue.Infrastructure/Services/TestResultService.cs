using AutoMapper;
using Microsoft.Extensions.Logging;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.ExpertTestResult;
using PerHue.Application.Models.ManualTest;
using PerHue.Application.Models.TestRequest;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.AI;
using Microsoft.EntityFrameworkCore;

namespace PerHue.Infrastructure.Services
{
	internal class TestResultService : ITestResultService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly GeminiService _gemini;
		ILogger<TestResultService> _logger;
		public TestResultService(IUnitOfWork unitOfWork, IMapper mapper, GeminiService gemini, ILogger<TestResultService> logger)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_gemini = gemini;
			_logger = logger;
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

		public async Task<TestResultModel> GetByIdAsync(int id)
		{
			var testResult = await _unitOfWork.TestResultRepository.GetByIdAsync(id);
			return _mapper.Map<TestResultModel>(testResult);
		}

		public async Task<TestResultModel> GetNormalTestSimpleColorResult(CreateManualTestResultModel model)
		{
			var capsulePalettes = await _unitOfWork.CapsulePaletteRepository.GetRelativeCapsulePalettes(model.SelectedColors);

			var entity = new TestResult
			{
				UserId = model.UserId,
				User = await _unitOfWork.UserRepository.GetByIdAsync(model.UserId),
				Picture = "",
			};
			// code fix

			return _mapper.Map<TestResultModel>(entity);
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
			_logger.LogInformation($"Creating expert test request for user {parameters.UserId} with image {parameters.ImageUrl}");

			// 1. Create the TestRequest from the parameters object
			var testRequest = new TestRequest
			{
				SkinColor = parameters.SkinColor,
				HairColor = parameters.HairColor,
				EyesColor = parameters.EyesColor,
				LipsColor = parameters.LipsColor,
				Status = "Pending",
				CreatedDate = DateTime.Now,
				TypeOfTest = "Expert",
				UserAccountId = parameters.UserId
			};

			// 2. Add the user's uploaded picture
			testRequest.AiPictures.Add(new AiPicture { Source = parameters.ImageUrl, Note = "Original Photo" });
			testRequest.Pictures.Add(new Picture { Source = parameters.ImageUrl });

			await _unitOfWork.TestRequestRepository.CreateAsync(testRequest);
			await _unitOfWork.SaveChangesWithTransactionAsync();
			_logger.LogInformation($"Created TestRequest with ID {testRequest.Id}");

			// 3. Send to Experts
			var allExperts = await _unitOfWork.ExpertRepository.GetAllAsync();
			var expertsToRequest = allExperts.Where(e => e.Id != parameters.UserId).Take(3);

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
				_logger.LogInformation($"Assigned request {testRequest.Id} to expert {expert.Id}");
			}

			await _unitOfWork.SaveChangesWithTransactionAsync();

			return _mapper.Map<TestRequestModel>(testRequest);
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
