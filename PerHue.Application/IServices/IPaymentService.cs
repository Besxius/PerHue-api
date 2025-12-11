using PerHue.Application.Basic;
using PerHue.Application.Models;
using PerHue.Application.Models.Payment;
using PerHue.Application.Models.Payment.An;

namespace PerHue.Application.IServices
{
	public interface IPaymentService : IGenericService<PerHue.Application.Models.Payment.PaymentModel>
	{
		Task<IEnumerable<PerHue.Application.Models.Payment.PaymentModel>> GetAllAsync(int userId);
		Task<IEnumerable<PerHue.Application.Models.Payment.PaymentModel>> GetAllAsync();
		Task<PerHue.Application.Models.Payment.PaymentModel> GetByIdAsync(int id);
		Task<string> CreateAsync(PayOSRequestModel model);

		Task<int> CreateSuccessPaymentInDbAsync(PerHue.Application.Models.Payment.An.CreatePaymentModel model);

		/// Lấy tất cả payments của user với phân trang
		/// </summary>
		Task<PaginatedResultV2<PerHue.Application.Models.Payment.An.PaymentDetailModel>> GetPaymentHistoryByUserIdAsync(
			int userId,
			int pageIndex,
			int pageSize);

		/// <summary>
		/// Lấy chi tiết payment theo id với PaymentLogs
		/// </summary>
		Task<PerHue.Application.Models.Payment.An.PaymentDetailModel?> GetPaymentDetailByIdAsync(int paymentId, int userId);

		/// <summary>
		/// Lấy tất cả payments của user (không phân trang)
		/// </summary>
		Task<List<PerHue.Application.Models.Payment.An.PaymentDetailModel>> GetAllPaymentsByUserIdAsync(int userId);

		/// <summary>
		/// [ADMIN] Lấy tất cả payments kèm thông tin user (phân trang, lọc user)
		/// </summary>
		Task<PaginatedResultV2<PaymentDetailModel>> GetPaymentsForAdminAsync(AdminPaymentSearchModel searchModel);

		/// <summary>
		/// [ADMIN] Lấy chi tiết payment kèm logs (bỏ qua kiểm tra ownership)
		/// </summary>
		Task<PaymentDetailModel?> GetPaymentDetailForAdminAsync(int paymentId);
	}
}
