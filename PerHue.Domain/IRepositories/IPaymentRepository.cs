using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IPaymentRepository : IGenericRepository<Payment>
	{
		Task<IEnumerable<Payment>> GetAllAsync();
		Task<IEnumerable<Payment>> GetAllByUserIdAsync(int userId);
		Task<Payment> GetByIdAsync(int id);
	}
}
