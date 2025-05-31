using PerHue.Application.Models;

namespace PerHue.Application.IServices
{
	public interface IPaymentService
	{
		Task<IEnumerable<PaymentModel>> GetAllPaymentsAsync(int userId);
		Task<IEnumerable<PaymentModel>> GetAllPaymentsAsync();
		Task<PaymentModel> GetPaymentByIdAsync(int id);
		Task<string> CreatePaymentAsync(PayOSRequestModel model);
	}
}
