using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.Expert;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;

namespace PerHue.Infrastructure.Services
{
	internal class AdminExpertService : IAdminExpertService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IExpertService _expertService;

		public AdminExpertService(IUnitOfWork unitOfWork, IMapper mapper, IExpertService expertService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_expertService = expertService;
		}

		public async Task<IEnumerable<ExpertModel>> GetAllAsync()
		{
			var experts = await _unitOfWork.ExpertRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<ExpertModel>>(experts);
		}

		public async Task<PaginatedResultV2<ExpertModel>> GetAllAsync(ExpertSearchModel searchModel)
		{
			var baseQuery = _unitOfWork.ExpertRepository.GetQueryable();
			IQueryable<Expert> query = baseQuery.Include(e => e.IdNavigation);

			// Apply search filters
			if (!string.IsNullOrEmpty(searchModel.SearchTerm))
			{
				switch (searchModel.SearchBy?.ToLower())
				{
					case "email":
						query = query.Where(e => e.IdNavigation.Email.Contains(searchModel.SearchTerm));
						break;
					case "username":
						query = query.Where(e => e.IdNavigation.Username.Contains(searchModel.SearchTerm));
						break;
					case "nickname":
						query = query.Where(e => e.Nickname != null && e.Nickname.Contains(searchModel.SearchTerm));
						break;
					case "specialization":
						query = query.Where(e => e.Specialization.Contains(searchModel.SearchTerm));
						break;
					default:
						query = query.Where(e => 
							e.IdNavigation.Email.Contains(searchModel.SearchTerm) || 
							e.IdNavigation.Username.Contains(searchModel.SearchTerm) ||
							(e.Nickname != null && e.Nickname.Contains(searchModel.SearchTerm)) ||
							e.Specialization.Contains(searchModel.SearchTerm));
						break;
				}
			}

			// Apply sorting
			IOrderedQueryable<Expert> orderedQuery;
			switch (searchModel.SortBy?.ToLower())
			{
				case "yearsofexperience":
					orderedQuery = searchModel.SortOrder?.ToLower() == "asc"
						? query.OrderBy(e => e.YearsOfExperience)
						: query.OrderByDescending(e => e.YearsOfExperience);
					break;
				case "id":
					orderedQuery = searchModel.SortOrder?.ToLower() == "asc"
						? query.OrderBy(e => e.Id)
						: query.OrderByDescending(e => e.Id);
					break;
				case "rating":
				default:
					orderedQuery = searchModel.SortOrder?.ToLower() == "asc"
						? query.OrderBy(e => e.Rating ?? 0)
						: query.OrderByDescending(e => e.Rating ?? 0);
					break;
			}

			var totalCount = await query.CountAsync();

			var experts = await orderedQuery
				.Skip((searchModel.PageIndex - 1) * searchModel.PageSize)
				.Take(searchModel.PageSize)
				.ToListAsync();

			var expertModels = _mapper.Map<IEnumerable<ExpertModel>>(experts);

			return new PaginatedResultV2<ExpertModel>
			{
				List = expertModels,
				Total = totalCount,
				Current = searchModel.PageIndex
			};
		}

		public async Task<PaginatedResultV2<ExpertSalaryModel>> GetAllSalaryReportsAsync(ExpertSalarySearchModel searchModel)
		{
			var baseQuery = _unitOfWork.ExpertRepository.GetQueryable();
			var query = baseQuery.AsQueryable();

			// Get all experts
			var experts = await query.ToListAsync();

			// Calculate salary for each expert
			var salaryReports = new List<ExpertSalaryModel>();
			foreach (var expert in experts)
			{
				var salaryReport = await _expertService.CalculateSalaryAsync(expert.Id, searchModel.StartDate, searchModel.EndDate);
				salaryReports.Add(salaryReport);
			}

			// Apply search filter
			if (!string.IsNullOrEmpty(searchModel.SearchTerm))
			{
				if (int.TryParse(searchModel.SearchTerm, out int expertId))
				{
					salaryReports = salaryReports.Where(s => s.ExpertId == expertId).ToList();
				}
			}

			// Apply sorting
			switch (searchModel.SortBy?.ToLower())
			{
				case "totalrequests":
					salaryReports = searchModel.SortOrder?.ToLower() == "asc"
						? salaryReports.OrderBy(s => s.TotalRequests).ToList()
						: salaryReports.OrderByDescending(s => s.TotalRequests).ToList();
					break;
				case "averagerating":
					salaryReports = searchModel.SortOrder?.ToLower() == "asc"
						? salaryReports.OrderBy(s => s.AverageRating).ToList()
						: salaryReports.OrderByDescending(s => s.AverageRating).ToList();
					break;
				case "expertid":
					salaryReports = searchModel.SortOrder?.ToLower() == "asc"
						? salaryReports.OrderBy(s => s.ExpertId).ToList()
						: salaryReports.OrderByDescending(s => s.ExpertId).ToList();
					break;
				case "totalsalary":
				default:
					salaryReports = searchModel.SortOrder?.ToLower() == "asc"
						? salaryReports.OrderBy(s => s.TotalSalary).ToList()
						: salaryReports.OrderByDescending(s => s.TotalSalary).ToList();
					break;
			}

			var totalCount = salaryReports.Count;

			// Apply pagination
			var paginatedReports = salaryReports
				.Skip((searchModel.PageIndex - 1) * searchModel.PageSize)
				.Take(searchModel.PageSize)
				.ToList();

			return new PaginatedResultV2<ExpertSalaryModel>
			{
				List = paginatedReports,
				Total = totalCount,
				Current = searchModel.PageIndex
			};
		}

		public Task<bool> CreateAsync(ExpertModel entity)
		{
			throw new NotImplementedException();
		}

		public Task<bool> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<ExpertModel> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<bool> UpdateAsync(ExpertModel entity)
		{
			throw new NotImplementedException();
		}
	}
}
