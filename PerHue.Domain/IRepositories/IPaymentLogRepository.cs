using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IPaymentLogRepository : IGenericRepository<PaymentLog>
	{
		Task<IEnumerable<PaymentLog>> GetAllAsync();
		Task<IEnumerable<PaymentLog>> GetAllByUserIdAsync(int userId);
		Task<PaymentLog> GetByIdAsync(int id);
	}
}
