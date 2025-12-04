using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.Report;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;

namespace PerHue.Infrastructure.Services
{
	internal class ReportService : IReportService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ReportModel> CreateReportAsync(int userId, CreateReportModel model)
		{
			var entity = new Report
			{
				Content = model.Content,
				Type = model.Type,
				Status = "Pending",
				UserAccountId = userId
			};

			await _unitOfWork.ReportRepository.CreateAsync(entity);

			var createdReport = await _unitOfWork.ReportRepository
				.FindAsync(r => r.Id == entity.Id && r.UserAccountId == userId);

			return MapToReportModel(createdReport.FirstOrDefault())!;
		}
		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _unitOfWork.ReportRepository.GetByIdAsync(id);
			if (entity == null)
				return false;

			return await _unitOfWork.ReportRepository.RemoveAsync(entity);
		}

		public async Task<IEnumerable<ReportModel>> GetAllAsync()
		{
			var reports = await _unitOfWork.ReportRepository.GetAllAsync();
			return reports.Select(MapToReportModel).Where(r => r != null).Cast<ReportModel>();
		}

		public async Task<PaginatedResultV2<ReportModel>> GetAllReportsAsync(ReportSearchModel searchModel)
		{
			var query = _unitOfWork.ReportRepository.GetQueryable()
				.Include(r => r.UserAccount)
				.AsQueryable();

			// Apply filters
			if (!string.IsNullOrEmpty(searchModel.Type))
			{
				query = query.Where(r => r.Type == searchModel.Type);
			}

			if (!string.IsNullOrEmpty(searchModel.Status))
			{
				query = query.Where(r => r.Status == searchModel.Status);
			}

			if (!string.IsNullOrEmpty(searchModel.SearchKeyword))
			{
				query = query.Where(r => (r.Content != null && r.Content.Contains(searchModel.SearchKeyword)) ||
										 (r.UserAccount != null && r.UserAccount.Email.Contains(searchModel.SearchKeyword)) ||
										 (r.UserAccount != null && r.UserAccount.Username.Contains(searchModel.SearchKeyword)));
			}
			if (searchModel.FromDate.HasValue)
			{
				query = query.Where(r => r.Id >= 0); // Placeholder for CreatedAt filter if it exists
			}

			if (searchModel.ToDate.HasValue)
			{
				query = query.Where(r => r.Id >= 0); // Placeholder for CreatedAt filter if it exists
			}

			// Apply sorting
			query = searchModel.SortOrder == 0
				? query.OrderByDescending(r => r.Id)
				: query.OrderBy(r => r.Id);

			// Get total count
			var totalCount = await query.CountAsync();


			// Apply pagination
			var reports = await query
				.Skip((searchModel.PageIndex - 1) * searchModel.PageSize)
				.Take(searchModel.PageSize)
				.ToListAsync();

			return new PaginatedResultV2<ReportModel>
			{
				List = reports.Select(MapToReportModel).Where(r => r != null).Cast<ReportModel>(),
				Total = totalCount,
				Current = searchModel.PageIndex
			};
		}
		public async Task<ReportModel> GetByIdAsync(int id)
		{
			var entity = await _unitOfWork.ReportRepository
				.FindAsync(r => r.Id == id);

			return MapToReportModel(entity.FirstOrDefault())!;
		}

		public async Task<IEnumerable<ReportModel>> GetReportsByUserIdAsync(int userId)
		{
			var reports = await _unitOfWork.ReportRepository.GetReportsByUserIdAsync(userId);
			return reports.Select(MapToReportModel).Where(r => r != null).Cast<ReportModel>();
		}

		public async Task<bool> UpdateReportAsync(int reportId, UpdateReportModel model)
		{
			var entity = await _unitOfWork.ReportRepository.GetByIdAsync(reportId);
			if (entity == null)
				return false;

			if (!string.IsNullOrEmpty(model.Content))
				entity.Content = model.Content;

			if (!string.IsNullOrEmpty(model.Type))
				entity.Type = model.Type;

			if (!string.IsNullOrEmpty(model.Status))
				entity.Status = model.Status;

			if (!string.IsNullOrEmpty(model.Notice))
				entity.Notice = model.Notice;

			await _unitOfWork.ReportRepository.UpdateAsync(entity);
			return true;
		}

		public async Task<bool> UpdateReportStatusAsync(int reportId, string status, string? notice = null)
		{
			return await _unitOfWork.ReportRepository.UpdateReportStatusAsync(reportId, status, notice);
		}

		private ReportModel? MapToReportModel(Report? entity)
		{
			if (entity == null)
				return null;

			return new ReportModel
			{
				Id = entity.Id,
				Content = entity.Content,
				Type = entity.Type,
				Status = entity.Status,
				Notice = entity.Notice,
				UserAccountId = entity.UserAccountId,
				UserEmail = entity.UserAccount?.Email,
				Username = entity.UserAccount?.Username
			};
		}
	}
}
