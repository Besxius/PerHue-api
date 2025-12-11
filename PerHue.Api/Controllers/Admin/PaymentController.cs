using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Payment;
using PerHue.Application.Models.Payment.An;
using PerHue.Application.Models.PaymentLog;
using System.Linq;

namespace PerHue.Api.Controllers.Admin
{
	[Route("api/admin/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin, Moderator")]
	public class PaymentController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public PaymentController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		/// <summary>
		/// Get all payments (optionally filter by user)
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<PaginatedResultV2<PaymentDetailModel>>> GetPayments([FromQuery] AdminPaymentSearchModel searchModel)
		{
			try
			{
				var payments = await _servicesProvider.PaymentService.GetPaymentsForAdminAsync(searchModel);
				return Ok(payments);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ServiceResponse<IEnumerable<PaymentDetailModel>>.Error($"An error occurred while retrieving payments: {ex.Message}"));
			}
		}

		/// <summary>
		/// Get payment detail with logs
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<ServiceResponse<PaymentDetailModel>>> GetPaymentDetail(int id)
		{
			try
			{
				var payment = await _servicesProvider.PaymentService.GetPaymentDetailForAdminAsync(id);
				if (payment == null)
				{
					return NotFound(ServiceResponse<PaymentDetailModel>.NotFound("Payment not found"));
				}

				return Ok(ServiceResponse<PaymentDetailModel>.Ok(payment, "Payment retrieved successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ServiceResponse<PaymentDetailModel>.Error($"An error occurred while retrieving payment detail: {ex.Message}"));
			}
		}
	}
}
