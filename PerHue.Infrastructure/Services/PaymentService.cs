using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Services
{
	internal class PaymentService : IPaymentService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly PayOSPaymentService _payOSPaymentService;
		private readonly IMapper _mapper;

		public PaymentService(IUnitOfWork unitOfWork, PayOSPaymentService payOSPaymentService, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_payOSPaymentService = payOSPaymentService;
			_mapper = mapper;
		}

		public async Task<string> CreatePaymentAsync(PayOSRequestModel model)
		{
			return await _payOSPaymentService.CreatePaymentAsync(model);
		}

		public async Task<IEnumerable<PaymentModel>> GetAllPaymentsAsync(int userId)
		{
			var entities = await _unitOfWork.PaymentRepository.GetAllPaymentsByUserIdAsync(userId);
			return _mapper.Map<IEnumerable<PaymentModel>>(entities);
		}

		public async Task<IEnumerable<PaymentModel>> GetAllPaymentsAsync()
		{
			var entities = await _unitOfWork.PaymentRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<PaymentModel>>(entities);
		}

		public async Task<PaymentModel> GetPaymentByIdAsync(int id)
		{
			var entity = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
			return _mapper.Map<PaymentModel>(entity);
		}
	}
}
