using AutoMapper;
using Azure.Core;
using PerHue.Application.Basic;
using PerHue.Application.IServices;
using PerHue.Application.Models.PaymentLog;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Services
{
	internal class PaymentLogService : IPaymentLogService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IDateTimeService _dateTimeService;
		public PaymentLogService(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeService dateTimeService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_dateTimeService = dateTimeService;
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

		public async Task CreatePaymentLogAsync(CreatePaymentLogModel model)
		{
			var entity = new PaymentLog
			{
				PaymentId = model.PaymentId,
				OldStatus = model.OldStatus,
				NewStatus = model.NewStatus,
				Mesage = model.Mesage,
				Metadata = model.Metadata,
				EventType = EventTypeEnum.StatusChanged.ToString(),
				CreatedAt = _dateTimeService.GetCurrentTime()
			};
			await _unitOfWork.PaymentLogRepository.CreateAsync(entity);
			await _unitOfWork.SaveChangesWithTransactionAsync();
		}
	}
}
