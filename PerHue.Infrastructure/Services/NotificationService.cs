using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models.Notification;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
		private readonly IDateTimeService _dateTimeService;

		public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeService dateTimeService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_dateTimeService = dateTimeService;
		}

		public async Task<NotificationModel> GetByIdAsync(int id)
        {
            var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(id);
            return _mapper.Map<NotificationModel>(notification);
        }

        public async Task<IEnumerable<NotificationModel>> GetAllAsync()
        {
            var notifications = await _unitOfWork.NotificationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<NotificationModel>>(notifications);
        }

        public async Task<IEnumerable<NotificationModel>> GetByReceiverAsync(int receiverId)
        {
            var notifications = await _unitOfWork.NotificationRepository.GetByReceiverAsync(receiverId);
            return _mapper.Map<IEnumerable<NotificationModel>>(notifications);
        }

        public async Task<IEnumerable<NotificationModel>> GetUnreadByReceiverAsync(int receiverId)
        {
            var notifications = await _unitOfWork.NotificationRepository.GetUnreadByReceiverAsync(receiverId);
            return _mapper.Map<IEnumerable<NotificationModel>>(notifications);
        }

        public async Task CreateAsync(CreateNotificationModel model)
        {
            var notification = new Notification
            {
                Title = model.Title,
                Content = model.Content,
                Receiver = model.Receiver,
                ReceivedTime = _dateTimeService.GetCurrentTime(),
                IsRead = false
            };

            await _unitOfWork.NotificationRepository.CreateAsync(notification);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task MarkAsReadAsync(int id)
        {
            await _unitOfWork.NotificationRepository.MarkAsReadAsync(id);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task MarkAllAsReadAsync(int receiverId)
        {
            await _unitOfWork.NotificationRepository.MarkAllAsReadAsync(receiverId);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.NotificationRepository.DeleteAsync(id);
            if (result)
            {
                await _unitOfWork.SaveChangesWithTransactionAsync();
                return true;
            }
            return false;
        }
    }
}