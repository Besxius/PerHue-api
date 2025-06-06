using PerHue.Application.Basic;
using PerHue.Application.Models;

namespace PerHue.Application.IServices
{
	public interface IPaymentService : IGenericService<PaymentModel>
	{
		Task<IEnumerable<PaymentModel>> GetAllAsync(int userId);
		Task<IEnumerable<PaymentModel>> GetAllAsync();
		Task<PaymentModel> GetByIdAsync(int id);
		Task<string> CreateAsync(PayOSRequestModel model);
	}
}
