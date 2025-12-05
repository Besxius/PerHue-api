using AutoMapper;
using Net.payOS.Types;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.Payment;
using PerHue.Application.Models.Payment.An;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Services
{
	internal class PaymentService : IPaymentService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly PayOSPaymentService _payOSPaymentService;
		private readonly IMapper _mapper;
		private readonly IUserSubscriptionService _subscriptionService;
		private readonly IServicePackageService _packageService;

		public PaymentService(IUnitOfWork unitOfWork, PayOSPaymentService payOSPaymentService, IMapper mapper,
			IUserSubscriptionService subscriptionService, IServicePackageService packageService
			)
		{
			_unitOfWork = unitOfWork;
			_payOSPaymentService = payOSPaymentService;
			_mapper = mapper;
			_subscriptionService = subscriptionService;
			_packageService = packageService;
		}

		public async Task<string> CreateAsync(PayOSRequestModel model)
		{
			return await _payOSPaymentService.CreatePaymentAsync(model);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
			return await _unitOfWork.PaymentRepository.RemoveAsync(entity);
		}

		public async Task<IEnumerable<PerHue.Application.Models.Payment.PaymentModel>> GetAllAsync(int userId)
		{
			var entities = await _unitOfWork.PaymentRepository.GetAllByUserIdAsync(userId);
			return _mapper.Map<IEnumerable<PerHue.Application.Models.Payment.PaymentModel>>(entities);
		}

		public async Task<IEnumerable<PerHue.Application.Models.Payment.PaymentModel>> GetAllAsync()
		{
			var entities = await _unitOfWork.PaymentRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<PerHue.Application.Models.Payment.PaymentModel>>(entities);
		}

		public async Task<PerHue.Application.Models.Payment.PaymentModel> GetByIdAsync(int id)
		{
			var entity = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
			return _mapper.Map<PerHue.Application.Models.Payment.PaymentModel>(entity);
		}


		// ========= AN LÀM

		public async Task<int> CreateSuccessPaymentInDbAsync(PerHue.Application.Models.Payment.An.CreatePaymentModel model) {
			var payment = new Payment
			{
				Id = model.PaymentId,
				UserId = model.UserId,
				Amount = model.Amount,
				Description = model.Description,
				Status = PaymentStatusEnum.Success.ToString(),
				TransactionId = model.OrderCode,
				CreatedAt = DateTime.Now
			};
			var numberOfPaymentCreated = await _unitOfWork.PaymentRepository.CreateAsync(payment);

			return payment.Id;
		}

		/// <summary>
		/// Lấy tất cả payments của user với phân trang
		/// </summary>
		public async Task<PaginatedResultV2<PaymentDetailModel>> GetPaymentHistoryByUserIdAsync(
			int userId,
			int pageIndex,
			int pageSize)
		{
			var (payments, totalCount) = await _unitOfWork.PaymentRepository
				.GetPaymentsByUserIdWithPaginationAsync(userId, pageIndex, pageSize);

			var models = payments.Select(p => new PaymentDetailModel
			{
				Id = p.Id,
				UserId = p.UserId,
				UserFullname = p.User?.Fullname ?? string.Empty,
				UserEmail = p.User?.Email ?? string.Empty,
				Amount = p.Amount,
				Description = p.Description,
				OrderCode = p.TransactionId,
				CreatedAt = p.CreatedAt,
				PaymentLogs = p.PaymentLogs?.Select(pl => new PaymentLogDetailModel
				{
					Id = pl.Id,
					PaymentId = pl.PaymentId,
					Message = pl.Mesage,
					CreatedAt = pl.CreatedAt,
					OldStatus = pl.OldStatus,
					NewStatus = pl.NewStatus,
					Metadata = pl.Metadata
				}).OrderByDescending(pl => pl.CreatedAt).ToList() ?? new()
			}).ToList();

			return new PaginatedResultV2<PaymentDetailModel>
			{
				List = models,
				Total = totalCount,
				Current = pageIndex
			};
		}

		/// <summary>
		/// Lấy chi tiết payment theo id với PaymentLogs
		/// </summary>
		public async Task<PaymentDetailModel?> GetPaymentDetailByIdAsync(int paymentId, int userId)
		{
			var payment = await _unitOfWork.PaymentRepository.GetPaymentByIdWithLogsAsync(paymentId);

			if (payment == null)
				return null;

			// Kiểm tra ownership
			if (payment.UserId != userId)
				return null;

			return new PaymentDetailModel
			{
				Id = payment.Id,
				UserId = payment.UserId,
				UserFullname = payment.User?.Fullname ?? string.Empty,
				UserEmail = payment.User?.Email ?? string.Empty,
				Amount = payment.Amount,
				Description = payment.Description,
				OrderCode = payment.TransactionId,
				CreatedAt = payment.CreatedAt,
				PaymentLogs = payment.PaymentLogs?.Select(pl => new PaymentLogDetailModel
				{
					Id = pl.Id,
					PaymentId = pl.PaymentId,
					Message = pl.Mesage,
					CreatedAt = pl.CreatedAt,
					OldStatus = pl.OldStatus,
					NewStatus = pl.NewStatus,
					Metadata = pl.Metadata
				}).OrderByDescending(pl => pl.CreatedAt).ToList() ?? new()
			};
		}

		/// <summary>
		/// Lấy tất cả payments của user (không phân trang)
		/// </summary>
		public async Task<List<PaymentDetailModel>> GetAllPaymentsByUserIdAsync(int userId)
		{
			var payments = await _unitOfWork.PaymentRepository.GetAllPaymentsByUserIdAsync(userId);

			return payments.Select(p => new PaymentDetailModel
			{
				Id = p.Id,
				UserId = p.UserId,
				UserFullname = p.User?.Fullname ?? string.Empty,
				UserEmail = p.User?.Email ?? string.Empty,
				Amount = p.Amount,
				Description = p.Description,
				OrderCode = p.TransactionId,
				CreatedAt = p.CreatedAt,
				PaymentLogs = p.PaymentLogs?.Select(pl => new PaymentLogDetailModel
				{
					Id = pl.Id,
					PaymentId = pl.PaymentId,
					Message = pl.Mesage,
					CreatedAt = pl.CreatedAt,
					OldStatus = pl.OldStatus,
					NewStatus = pl.NewStatus,
					Metadata = pl.Metadata
				}).OrderByDescending(pl => pl.CreatedAt).ToList() ?? new()
			}).ToList();
		}

	}
}
