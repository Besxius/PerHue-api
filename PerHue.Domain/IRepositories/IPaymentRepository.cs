using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IPaymentRepository : IGenericRepository<Payment>
	{
		Task<IEnumerable<Payment>> GetAllPaymentsByUserIdAsync(int userId);
	}
}
