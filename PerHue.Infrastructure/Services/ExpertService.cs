using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models.Expert;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PerHue.Infrastructure.Services
{
	public class ExpertService : IExpertService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IConfiguration _config;

		public ExpertService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_config = config;
		}

		public async Task<ExpertModel> GetByIdAsync(int id)
		{
			var expert = await _unitOfWork.ExpertRepository.GetByIdAsync(id);
			return _mapper.Map<ExpertModel>(expert);
		}

		public async Task<IEnumerable<ExpertModel>> GetAllAsync()
		{
			var experts = await _unitOfWork.ExpertRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<ExpertModel>>(experts);
		}

		public async Task<bool> UpdateAsync(int id, UpdateExpertModel model)
		{
			var expert = await _unitOfWork.ExpertRepository.GetByIdAsync(id);
			if (expert == null)
				return false;

			// Update expert properties
			expert.Nickname = model.Nickname;
			expert.Specialization = model.Specialization;
			expert.Bio = model.Bio;
			expert.YearsOfExperience = model.YearsOfExperience;
			expert.Languages = model.Languages;
			expert.Certification = model.Certification;
			expert.Introduction = model.Introduction;
			expert.FacebookAccount = model.FacebookAccount;
			expert.LinkedInAccount = model.LinkedInAccount;
			expert.InstagramAccount = model.InstagramAccount;

			await _unitOfWork.ExpertRepository.UpdateAsync(expert);
			await _unitOfWork.SaveChangesWithTransactionAsync();
			return true;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var result = await _unitOfWork.ExpertRepository.DeleteAsync(id);
			if (result)
			{
				await _unitOfWork.SaveChangesWithTransactionAsync();
				return true;
			}
			return false;
		}

		public async Task<bool> ExistsAsync(int id)
		{
			return await _unitOfWork.ExpertRepository.ExistsAsync(id);
		}
		public async Task<IEnumerable<ExpertModel>> GetAllByRatingDescendingAsync()
		{
			var experts = await _unitOfWork.ExpertRepository.GetAllByRatingDescendingAsync();
			return _mapper.Map<IEnumerable<ExpertModel>>(experts);
		}

		public async Task<ExpertSalaryModel> CalculateSalaryAsync(int expertId, DateTime? startDate, DateTime? endDate)
		{
			// 1. Get Configuration Values (and apply multiplier for thousands if needed)
			// Default to 0 if config is missing to avoid crashes, but ideally config should exist.
			decimal baseSc1 = _config.GetValue<decimal>("Salary:sc1");
			decimal baseSc2 = _config.GetValue<decimal>("Salary:sc2");
			decimal baseSc3 = _config.GetValue<decimal>("Salary:sc3");

			// Assuming config values like 50, 100, 150 represent thousands (50k, 100k, 150k)
			decimal salaryBelow4Stars = baseSc1 * 1000;
			decimal salary4Stars = baseSc2 * 1000;
			decimal salary5Stars = baseSc3 * 1000;

			// 2. Get all rated responses
			var ratedResponses = await _unitOfWork.TestResponseRepository.GetRatedResponsesByExpertIdAsync(expertId, startDate, endDate);

			var salaryModel = new ExpertSalaryModel
			{
				ExpertId = expertId,
				FromDate = startDate,
				ToDate = endDate,
				TotalRequests = ratedResponses.Count(),
				AverageRating = ratedResponses.Any() ? ratedResponses.Average(r => r.Rating ?? 0) : 0
			};

			// 3. Calculate
			decimal totalSalary = 0;

			foreach (var response in ratedResponses)
			{
				decimal amount = 0;
				int rating = response.Rating ?? 0;

				if (rating == 5)
				{
					amount = salary5Stars;
				}
				else if (rating == 4)
				{
					amount = salary4Stars;
				}
				else
				{
					amount = salaryBelow4Stars;
				}

				totalSalary += amount;

				salaryModel.Details.Add(new ExpertSalaryDetail
				{
					TestResponseId = response.Id,
					CompletedDate = response.CreatedDate ?? DateTime.MinValue,
					Rating = rating,
					Amount = amount
				});
			}

			salaryModel.TotalSalary = totalSalary;

			return salaryModel;
		}
		public async Task<IEnumerable<ExpertSalaryModel>> CalculateAllExpertsSalaryAsync(DateTime? startDate, DateTime? endDate)
		{
			// 1. Get all experts (using the existing method logic or repository directly)
			// We use the repository directly to get entities since we just need IDs for calculation
			var experts = await _unitOfWork.ExpertRepository.GetAllAsync();

			var reports = new List<ExpertSalaryModel>();

			// 2. Loop through each expert and calculate their salary
			foreach (var expert in experts)
			{
				// Reuse the single calculation logic to avoid code duplication
				var salaryReport = await CalculateSalaryAsync(expert.Id, startDate, endDate);
				reports.Add(salaryReport);
			}

			return reports;
		}
	}
}