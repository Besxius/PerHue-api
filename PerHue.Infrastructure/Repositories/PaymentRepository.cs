using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
	{
		public PaymentRepository(PerHueDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Payment>> GetAllByUserIdAsync(int userId)
		{
			return await _context.Payments
				.Include(p => p.User)
				.Where(p => p.UserId == userId).ToListAsync();
		}

		public async Task<IEnumerable<Payment>> GetAllAsync()
		{
			return await _context.Payments
				.Include(p => p.User)
				.ToListAsync();
		}

		public async Task<Payment> GetByIdAsync(int id)
		{
			return await _context.Payments
				.Include(p => p.User)
				.FirstOrDefaultAsync(p => p.Id == id);
		}

		/// <summary>
		/// Lấy tất cả payments của user với phân trang
		/// </summary>
		public async Task<(List<Payment> payments, int totalCount)> GetPaymentsByUserIdWithPaginationAsync(
			int userId,
			int pageIndex,
			int pageSize)
		{
			var query = _context.Payments
				.Include(p => p.User)
				.Include(p => p.PaymentLogs)
				.Where(p => p.UserId == userId);

			var totalCount = await query.CountAsync();

			var payments = await query
				.OrderByDescending(p => p.CreatedAt)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (payments, totalCount);
		}

		/// <summary>
		/// Lấy payment by id với PaymentLogs
		/// </summary>
		public async Task<Payment?> GetPaymentByIdWithLogsAsync(int paymentId)
		{
			return await _context.Payments
				.Include(p => p.User)
				.Include(p => p.PaymentLogs.OrderByDescending(pl => pl.CreatedAt))
				.FirstOrDefaultAsync(p => p.Id == paymentId);
		}

		/// <summary>
		/// Lấy tất cả payments của user (không phân trang)
		/// </summary>
		public async Task<List<Payment>> GetAllPaymentsByUserIdAsync(int userId)
		{
			return await _context.Payments
				.Include(p => p.User)
				.Include(p => p.PaymentLogs)
				.Where(p => p.UserId == userId)
				.OrderByDescending(p => p.CreatedAt)
				.ToListAsync();
		}
	}
}
