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
	}
}
