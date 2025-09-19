using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly PerHueDbContext _context;

        public NotificationRepository(PerHueDbContext context)
        {
            _context = context;
        }

        public async Task<Notification> GetByIdAsync(int id)
        {
            return await _context.Notifications
                .Include(n => n.ReceiverNavigation)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .Include(n => n.ReceiverNavigation)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetByReceiverAsync(int receiverId)
        {
            return await _context.Notifications
                .Where(n => n.Receiver == receiverId)
                .OrderByDescending(n => n.Time)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadByReceiverAsync(int receiverId)
        {
            return await _context.Notifications
                .Where(n => n.Receiver == receiverId && !n.IsRead)
                .OrderByDescending(n => n.Time)
                .ToListAsync();
        }

        public async Task CreateAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Entry(notification).State = EntityState.Modified;
        }

        public async Task MarkAsReadAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.Entry(notification).State = EntityState.Modified;
            }
        }

        public async Task MarkAllAsReadAsync(int receiverId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.Receiver == receiverId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                _context.Entry(notification).State = EntityState.Modified;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return false;

            _context.Notifications.Remove(notification);
            return true;
        }
    }
}