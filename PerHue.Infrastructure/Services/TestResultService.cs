using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Services
{
	internal class TestResultService : ITestResultService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public TestResultService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
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

		public async Task<TestResultModel> CreateNormalTestSimpleColorResult(CreateNormalTestResultModel model)
		{
			var capsulePalettes = await _unitOfWork.CapsulePaletteRepository.GetRelativeCapsulePalettes(model.SelectedColors);

			var entity = new TestResult
			{
				UserId = model.UserId,
				User = await _unitOfWork.UserRepository.GetByIdAsync(model.UserId),
				Picture = "",
				Type = TestTypeEnum.NormalTestSimpleColor.ToString(),
				ColorTypeId = capsulePalettes.First().ColorTypeId,
				CapsulePalettes = capsulePalettes.ToList(),
			};

			await _unitOfWork.TestResultRepository.CreateAsync(entity);

			var simpleColorList = new List<SimpleColor>();
			foreach (var color in model.SelectedColors)
			{
				var simpleColor = new SimpleColor
				{
					Hexcode = color,
					TestResultId = entity.Id,
					TestResult = entity
				};
				await _unitOfWork.SimpleColorRepository.CreateAsync(simpleColor);
				simpleColorList.Add(simpleColor);
			}

			entity.SimpleColors = simpleColorList;

			return _mapper.Map<TestResultModel>(entity);
		}
		public async Task<TestResultModel> CreateNormalTestCapsulePaletteResult(CreateNormalTestResultModel model)
		{
			var capsulePalettes = await _unitOfWork.CapsulePaletteRepository.GetRelativeCapsulePalettes(model.SelectedColors);

			var entity = new TestResult
			{
				UserId = model.UserId,
				User = await _unitOfWork.UserRepository.GetByIdAsync(model.UserId),
				Picture = "",
				Type = TestTypeEnum.NormalTestCapsulePalette.ToString(),
				ColorTypeId = capsulePalettes.First().ColorTypeId,
				CapsulePalettes = capsulePalettes.ToList(),
			};

			await _unitOfWork.TestResultRepository.CreateAsync(entity);

			var simpleColorList = new List<SimpleColor>();
			foreach (var color in model.SelectedColors)
			{
				var simpleColor = new SimpleColor
				{
					Hexcode = color,
					TestResultId = entity.Id,
					TestResult = entity
				};
				await _unitOfWork.SimpleColorRepository.CreateAsync(simpleColor);
				simpleColorList.Add(simpleColor);
			}

			entity.SimpleColors = simpleColorList;

			return _mapper.Map<TestResultModel>(entity);
		}
	}
}
