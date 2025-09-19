using PerHue.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Domain.IRepositories
{
    public interface INotificationRepository
    {
        Task<Notification> GetByIdAsync(int id);
        Task<IEnumerable<Notification>> GetAllAsync();
        Task<IEnumerable<Notification>> GetByReceiverAsync(int receiverId);
        Task<IEnumerable<Notification>> GetUnreadByReceiverAsync(int receiverId);
        Task CreateAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task MarkAsReadAsync(int id);
        Task MarkAllAsReadAsync(int receiverId);
        Task<bool> DeleteAsync(int id);
    }
}