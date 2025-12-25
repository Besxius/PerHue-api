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
using Microsoft.EntityFrameworkCore;

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

		/// <summary>
		/// Calculates the expert's average rating at a specific point in time based on historical responses.
		/// </summary>
		private async Task<decimal> CalculateRatingAtSpecificTime(int expertId, DateTime timePoint)
		{
			// Get all rated responses for this expert created strictly before the specified timePoint
			// This represents the expert's "reputation" at the moment they performed the task.
			var ratings = await _unitOfWork.TestResponseRepository.GetQueryable()
				.Where(r => r.ExpertId == expertId && r.Rating != null && r.CreatedDate < timePoint)
				.Select(r => r.Rating)
				.ToListAsync();

			if (!ratings.Any())
			{
				// Default rating is 5.0 if no history exists (new expert or perfect start)
				return 5.0m;
			}

			// Calculate average
			return (decimal)ratings.Average(r => r.Value);
		}

		/// <summary>
		/// Calculates the salary for a single test response based on the expert's rating at the time.
		/// </summary>
		private async Task<decimal> CalculateSalaryOfTestResponse(int testResponseId)
		{
			// 1. Get the response to identify the timestamp and expert
			var response = await _unitOfWork.TestResponseRepository.GetByIdAsync(testResponseId);
			if (response == null) return 0;

			// 2. Calculate Expert's rating at the time this response was created
			var ratingAtTime = await CalculateRatingAtSpecificTime(response.ExpertId, response.CreatedDate ?? DateTime.Now);

			// 3. Apply Salary Logic
			// Default salary: 30,000 VND
			// If rating <= 3.0: 20,000 VND
			if (ratingAtTime <= 3.0m)
			{
				return 20000m;
			}

			return 30000m;
		}

		public async Task<ExpertSalaryModel> CalculateSalaryAsync(int expertId, DateTime? startDate, DateTime? endDate)
		{
			// 1. Get all responses for the expert within the date range
			// Note: We use GetQueryable to filter by date and expert ID. We include ALL responses, not just rated ones.
			var query = _unitOfWork.TestResponseRepository.GetQueryable()
				.Where(r => r.ExpertId == expertId);

			if (startDate.HasValue)
				query = query.Where(r => r.CreatedDate >= startDate.Value);

			if (endDate.HasValue)
				query = query.Where(r => r.CreatedDate <= endDate.Value);

			var responses = await query.OrderByDescending(r => r.CreatedDate).ToListAsync();

			// 2. Prepare Model
			// Calculate current average rating for the summary header (based on all-time data)
			var allRatedResponses = await _unitOfWork.TestResponseRepository.GetQueryable()
				.Where(r => r.ExpertId == expertId && r.Rating != null)
				.ToListAsync();

			var salaryModel = new ExpertSalaryModel
			{
				ExpertId = expertId,
				FromDate = startDate,
				ToDate = endDate,
				TotalRequests = responses.Count,
				AverageRating = allRatedResponses.Any() ? allRatedResponses.Average(r => r.Rating ?? 0) : 0
			};

			// 3. Calculate Salary for each response
			decimal totalSalary = 0;

			foreach (var response in responses)
			{
				// Compute amount for this specific response using the helper function
				decimal amount = await CalculateSalaryOfTestResponse(response.Id);

				totalSalary += amount;

				salaryModel.Details.Add(new ExpertSalaryDetail
				{
					TestRequestId = response.TestRequestId, // Returning TestRequestId as requested
					CompletedDate = response.CreatedDate ?? DateTime.MinValue,
					Rating = response.Rating, // Can be null now
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