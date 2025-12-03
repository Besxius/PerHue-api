using AutoMapper;
using Net.payOS.Types;
using PerHue.Application.IServices;
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
				UserId = model.UserId,
				Amount = model.Amount,
				Description = model.Description,
				Status = PaymentStatusEnum.Success.ToString(),
				TransactionId = model.OrderCode,
				CreatedAt = DateTime.UtcNow
			};
			var createdPayment = await _unitOfWork.PaymentRepository.CreateAsync(payment);

			await _unitOfWork.SaveChangesWithTransactionAsync();

			return createdPayment;
		}

	}
}
