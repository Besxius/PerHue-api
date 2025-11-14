using PerHue.Application.Models.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Application.IServices
{
    public interface INotificationService
    {
        Task<NotificationModel> GetByIdAsync(int id);
        Task<IEnumerable<NotificationModel>> GetAllAsync();
        Task<IEnumerable<NotificationModel>> GetByReceiverAsync(int receiverId);
        Task<IEnumerable<NotificationModel>> GetUnreadByReceiverAsync(int receiverId);
        Task CreateAsync(CreateNotificationModel model);
        Task MarkAsReadAsync(int id);
        Task MarkAllAsReadAsync(int receiverId);
        Task<bool> DeleteAsync(int id);
    }
}