using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IPaymentRepository : IGenericRepository<Payment>
	{
		Task<IEnumerable<Payment>> GetAllAsync();
		Task<IEnumerable<Payment>> GetAllByUserIdAsync(int userId);
		Task<Payment> GetByIdAsync(int id);

		Task<(List<Payment> payments, int totalCount)> GetPaymentsByUserIdWithPaginationAsync(
			int userId,
			int pageIndex,
			int pageSize);

		Task<Payment?> GetPaymentByIdWithLogsAsync(int paymentId);

		Task<List<Payment>> GetAllPaymentsByUserIdAsync(int userId);
	}
}
