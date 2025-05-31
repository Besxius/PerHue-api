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

		public async Task<IEnumerable<Payment>> GetAllPaymentsByUserIdAsync(int userId)
		{
			return await _context.Payments.Where(p => p.UserId == userId).ToListAsync();
		}
	}
}
