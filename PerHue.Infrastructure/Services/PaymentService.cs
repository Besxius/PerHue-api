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

		public async Task<string> CreateAsync(PayOSRequestModel model)
		{
			return await _payOSPaymentService.CreatePaymentAsync(model);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
			return await _unitOfWork.PaymentRepository.RemoveAsync(entity);
		}

		public async Task<IEnumerable<PaymentModel>> GetAllAsync(int userId)
		{
			var entities = await _unitOfWork.PaymentRepository.GetAllByUserIdAsync(userId);
			return _mapper.Map<IEnumerable<PaymentModel>>(entities);
		}

		public async Task<IEnumerable<PaymentModel>> GetAllAsync()
		{
			var entities = await _unitOfWork.PaymentRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<PaymentModel>>(entities);
		}

		public async Task<PaymentModel> GetByIdAsync(int id)
		{
			var entity = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
			return _mapper.Map<PaymentModel>(entity);
		}
	}
}
