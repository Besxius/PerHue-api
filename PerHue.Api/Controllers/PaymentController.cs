using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Payment.An;
using System.Security.Claims;

namespace PerHue.Api.Controllers
{
	[Route("api/payment")]
	[ApiController]
	public class PaymentsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public PaymentsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		/// <summary>
		/// Lấy lịch sử giao dịch của user hiện tại với phân trang
		/// </summary>
		/// <param name="pageIndex">Trang hiện tại (mặc định: 1)</param>
		/// <param name="pageSize">Số items mỗi trang (mặc định: 10)</param>
		[HttpGet("user/payment-history")]
		[Authorize]
		public async Task<ActionResult<PaginatedResultV2<PaymentDetailModel>>> GetPaymentHistory(
			[FromQuery] int pageIndex = 1,
			[FromQuery] int pageSize = 10)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var result = await _servicesProvider.PaymentService
					.GetPaymentHistoryByUserIdAsync(userId, pageIndex, pageSize);

				return Ok(new
				{
					success = true,
					message = "Successfully retrieved payment history",
					data = result
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy tất cả lịch sử giao dịch của user hiện tại (không phân trang)
		/// </summary>
		[HttpGet("user/all-payments")]
		[Authorize]
		public async Task<ActionResult<List<PaymentDetailModel>>> GetAllPayments()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var payments = await _servicesProvider.PaymentService
					.GetAllPaymentsByUserIdAsync(userId);

				return Ok(new
				{
					success = true,
					message = "Successfully retrieved all payments",
					data = payments,
					count = payments.Count
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy chi tiết giao dịch theo id (kèm PaymentLogs)
		/// </summary>
		/// <param name="id">Payment ID</param>
		[HttpGet("user/payment/{id}")]
		[Authorize]
		public async Task<ActionResult<PaymentDetailModel>> GetPaymentDetail(int id)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var payment = await _servicesProvider.PaymentService
					.GetPaymentDetailByIdAsync(id, userId);

				if (payment == null)
				{
					return NotFound(new
					{
						success = false,
						message = "Payment not found or you don't have permission to view this payment"
					});
				}

				return Ok(new
				{
					success = true,
					message = "Successfully retrieved payment detail",
					data = payment
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// [ADMIN] Lấy chi tiết giao dịch theo id (không kiểm tra ownership)
		/// </summary>
		/// <param name="id">Payment ID</param>
		/*[HttpGet("admin/payment/{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<PaymentDetailModel>> GetPaymentDetailForAdmin(int id)
		{
			try
			{
				var payment = await _servicesProvider.PaymentService.GetPaymentDetailByIdAsync(id, 0); // Pass 0 để bỏ qua check ownership trong admin context

				var paymentEntity = await _servicesProvider.PaymentService.GetByIdAsync(id);

				if (paymentEntity == null)
				{
					return NotFound(new
					{
						success = false,
						message = "Payment not found"
					});
				}

				return Ok(new
				{
					success = true,
					message = "Successfully retrieved payment detail",
					data = paymentEntity
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}*/
	}
}
