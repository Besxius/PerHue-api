using PerHue.Application.Basic;
using PerHue.Application.Models.Payment;

namespace PerHue.Application.IServices
{
	public interface IPaymentService : IGenericService<PaymentModel>
	{
		Task<IEnumerable<PaymentModel>> GetAllAsync(int userId);
		Task<IEnumerable<PaymentModel>> GetAllAsync();
		Task<PaymentModel> GetByIdAsync(int id);
		Task<string> CreateAsync(PayOSRequestModel model);

		Task<int> CreateSuccessPaymentInDbAsync(PerHue.Application.Models.Payment.An.CreatePaymentModel model);
	}
}
