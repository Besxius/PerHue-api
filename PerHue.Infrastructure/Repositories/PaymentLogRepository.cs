using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class PaymentLogRepository : GenericRepository<PaymentLog>, IPaymentLogRepository
	{
		public PaymentLogRepository(PerHueDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<PaymentLog>> GetAllAsync()
		{
			return await _context.PaymentLogs
				.Include(pl => pl.Payment)
				.ToListAsync();
		}
		public async Task<IEnumerable<PaymentLog>> GetAllByUserIdAsync(int userId)
		{
			return await _context.PaymentLogs
				.Include(pl => pl.Payment)
				.Where(pl => pl.Payment.UserId == userId)
				.ToListAsync();
		}
		public async Task<PaymentLog> GetByIdAsync(int id)
		{
			return await _context.PaymentLogs
				.Include(pl => pl.Payment)
				.FirstOrDefaultAsync(pl => pl.Id == id);
		}
	}
}
