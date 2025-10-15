using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.AI;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Services
{
	internal class TestResultService : ITestResultService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly GeminiService _gemini;
		public TestResultService(IUnitOfWork unitOfWork, IMapper mapper, GeminiService gemini)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_gemini = gemini;
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

		public async Task<TestResultModel> GetNormalTestSimpleColorResult(CreateNormalTestResultModel model)
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
		public async Task<TestResultModel> GetNormalTestCapsulePaletteResult(CreateNormalTestResultModel model)
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
