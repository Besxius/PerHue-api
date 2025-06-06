using AutoMapper;
using Azure.Core;
using PerHue.Application.Basic;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;

namespace PerHue.Infrastructure.Services
{
	internal class PaymentLogService : IPaymentLogService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public PaymentLogService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _unitOfWork.PaymentRepository.GetByIdAsync(id);	
			return await _unitOfWork.PaymentRepository.RemoveAsync(entity);
		}

		public async Task<IEnumerable<PaymentLog>> GetAllAsync()
		{
			return await _unitOfWork.PaymentLogRepository.GetAllAsync();
		}

		public async Task<PaymentLog> GetByIdAsync(int id)
		{
			return await _unitOfWork.PaymentLogRepository.GetByIdAsync(id);
		}

		async Task<IEnumerable<PaymentLogModel>> IGenericService<PaymentLogModel>.GetAllAsync()
		{
			var entities = await _unitOfWork.PaymentLogRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<PaymentLogModel>>(entities);
		}

		async Task<PaymentLogModel> IGenericService<PaymentLogModel>.GetByIdAsync(int id)
		{
			var entity = await _unitOfWork.PaymentLogRepository.GetByIdAsync(id);
			return _mapper.Map<PaymentLogModel>(entity);
		}

		public async Task<int> CreateAsync(PaymentLogModel model)
		{
			var entity = _mapper.Map<PaymentLog>(model);
			await _unitOfWork.PaymentLogRepository.CreateAsync(entity);
			return entity.Id;
		}
	}
}
