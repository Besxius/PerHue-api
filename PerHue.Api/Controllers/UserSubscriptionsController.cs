using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.Payment;
using PerHue.Application.Models.PaymentLog;
using PerHue.Application.Models.UserSubscription;
using PerHue.Infrastructure.Utils;
using System.Security.Claims;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserSubscriptionsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		private readonly PayOSPaymentService _payOSPaymentService;
		public UserSubscriptionsController(IServicesProvider servicesProvider, PayOSPaymentService payOSPaymentService)
		{
			_servicesProvider = servicesProvider;
			_payOSPaymentService = payOSPaymentService;
		}
		[HttpGet]
		[Authorize]
		public async Task<IEnumerable<UserSubscriptionModel>> Gets()
		{
			return await _servicesProvider.UserSubscriptionService.GetAllAsync();
		}

		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<UserSubscriptionModel>> Get(int id)
		{
			var model = await _servicesProvider.UserSubscriptionService.GetByIdAsync(id);
			if (model == null)
				return NotFound();
			return Ok(model);
		}
		[HttpGet]
		[Route("user/current/{userId}")]
		public async Task<ActionResult<UserSubscriptionModel>> GetCurrent(int userId)
		{
			var model = await _servicesProvider.UserSubscriptionService.GetCurrentUserSubscriptionByUserIdAsync(userId);
			if (model == null)
				return NotFound();
			return Ok(model);
		}
		[HttpGet]
		[Route("user/all/{userId}")]
		public async Task<IEnumerable<UserSubscriptionModel>> GetsByUserId(int userId)
		{
			return await _servicesProvider.UserSubscriptionService.GetHistoryUserSubscriptionsByUserIdAsync(userId);
		}
		[HttpPost]
		[Route("subscription/{packageId}")]
		//[Authorize(Roles = "User")]
		public async Task<string> Post([FromRoute] int packageId, string returnUrl, string cancelUrl)
		{
			if (User.FindFirst(ClaimTypes.NameIdentifier) == null)
			{
				throw new UnauthorizedAccessException("User is not authenticated.");
			}

			var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

			var model = new CreateUserSubscriptionModel
			{
				//UserId = 2,
				UserId = userId,
				ServicePackageId = packageId,
			};

			var package = await _servicesProvider.ServicePackageService.GetByIdAsync(packageId);
			var description = CreateDateTimeStringNoSeparator(DateTime.Now) + $"U{model.UserId}P{model.ServicePackageId}";

			var paymentModel = new PayOSRequestModel
			{
				Amount = package.Price,
				Description = description,
				ReturnUrl = returnUrl,
				CancelUrl = cancelUrl
			};
			var paymentUrl = await _servicesProvider.PaymentService.CreateAsync(paymentModel);
			return paymentUrl;
		}

		/// <summary>
		/// Payment callback - xử lý cả success và cancel
		/// </summary>
		[HttpGet("payment-callback")]
		public async Task<IActionResult> PaymentCallback(
			[FromQuery] string code,
			[FromQuery] string id,
			[FromQuery] bool cancel,
			[FromQuery] string status,
			[FromQuery] string orderCode,
			[FromQuery] int servicePackageId)
		{
			try
			{

				var paymentInfo = await _payOSPaymentService.GetPaymentRequestInformationAsync(long.Parse(orderCode));

				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var description = CreateDateTimeStringNoSeparator(DateTime.Now) + $"U{userId}P{servicePackageId}";

				if (!cancel)
				{
					// Tạo subscription với status tương ứng
					var model = new CreateUserSubscriptionModel
					{
						UserId = userId,
						ServicePackageId = servicePackageId,
						Status = !cancel
					};

					var subscriptionId = await _servicesProvider.UserSubscriptionService.CreateAsync(model);

					//Tạo payment ở dưới db
					//var paymentDb = new PerHue.Application.Models.Payment.An.CreatePaymentModel
					//{
					//	UserId = userId,
					//	Amount = paymentInfo.amount,
					//	Description = description,
					//	OrderCode = orderCode,
					//};
					//int paymentId = await _servicesProvider.PaymentService.CreateSuccessPaymentInDbAsync(paymentDb);

					////Tạo payment log ở dưới db
					//var paymentLogDb = new CreatePaymentLogModel
					//{
					//	PaymentId = paymentId,
					//	Mesage = cancel ? "Payment cancelled by user." : "Payment completed successfully.",
					//	CreatedAt = DateTime.Now,
					//	OldStatus = "Pending",
					//	NewStatus = !cancel ? "Cancelled" : "Success", // cancel = false => success = true
					//	Metadata = $"UserId: {userId} , OrderCode: {orderCode} , Id: {id} , Code: {code} , Status: {status}"
					//};
					//await _servicesProvider.PaymentLogService.CreatePaymentLogAsync(paymentLogDb);
					// THANH TOÁN THÀNH CÔNG
					return Ok(new
					{
						success = true,
						message = "Payment successful",
						data = new
						{
							subscriptionId,
							userId,
							packageId = servicePackageId,
						}
					});
				}
				else
				{
					// THANH TOÁN THẤT BẠI
					return Ok(new
					{
						success = false,
						message = "Payment cancelled",
					});
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		[HttpGet("subscription/success")]
		public async Task<IActionResult> SubscriptionSuccess(
			[FromQuery] string code,
			[FromQuery] string id,
			[FromQuery] bool cancel,
			[FromQuery] string status,
			[FromQuery] string orderCode,
			int servicePackageId
			)
		{
			var paymentInfo = await _payOSPaymentService.GetPaymentRequestInformationAsync(long.Parse(orderCode));
			var servicePackage = await _servicesProvider.ServicePackageService.GetByIdAsync(servicePackageId);

			var model = new CreateUserSubscriptionModel
			{
				//UserId = 2,
				UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value),
				ServicePackageId = servicePackage.Id,
				Status = true,
			};

			var subscriptionId = await _servicesProvider.UserSubscriptionService.CreateAsync(model);

			return Ok(new
			{
				Message = "Payment process successful!",
			});
		}
		[HttpGet("subscription/cancel")]
		public async Task<IActionResult> SubscriptionCancelAsync(
			[FromQuery] string code,
			[FromQuery] string id,
			[FromQuery] bool cancel,
			[FromQuery] string status,
			[FromQuery] string orderCode,
			int servicePackageId
			)
		{
			var paymentInfo = await _payOSPaymentService.GetPaymentRequestInformationAsync(long.Parse(orderCode));
			var servicePackage = await _servicesProvider.ServicePackageService.GetByIdAsync(servicePackageId);

			var model = new CreateUserSubscriptionModel
			{
				//UserId = 2,
				UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value),
				ServicePackageId = servicePackage.Id,
				Status = false,
			};

			var subscriptionId = await _servicesProvider.UserSubscriptionService.CreateAsync(model);

			return Ok(new
			{
				Message = "Payment process failed!",
			});
		}

		private string CreateDateTimeStringNoSeparator(DateTime dateTime)
		{
			return dateTime.ToString("yyyyMMddHHmmss");
		}

		// ============== CÁC API MỚI BỔ SUNG ==============


		/// <summary>
		/// Lấy số lượt sử dụng còn lại của user hiện tại
		/// </summary>
		[HttpGet("remaining-usage-by-userId")]
		[Authorize]
		public async Task<ActionResult<int>> GetRemainingUsage()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var remaining = await _servicesProvider.UserSubscriptionService.GetRemainingUsageAsync(userId);

				return Ok(new
				{
					success = true,
					data = remaining
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy thông tin subscription đang active
		/// </summary>
		[HttpGet("active-subscription-by-userId")]
		[Authorize]
		public async Task<ActionResult<UserSubscriptionModel>> GetActiveSubscription()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var subscription = await _servicesProvider.UserSubscriptionService.GetActiveSubscriptionAsync(userId);

				if (subscription == null)
				{
					return NotFound(new
					{
						success = false,
						message = "No active subscription found"
					});
				}

				return Ok(new
				{
					success = true,
					data = subscription
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy TỔNG số lượt sử dụng còn lại (tất cả packages)
		/// </summary>
		[HttpGet("total-remaining-usage-by-userId")]
		[Authorize]
		public async Task<ActionResult<int>> GetTotalRemainingUsage()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var totalRemaining = await _servicesProvider.UserSubscriptionService.GetAllActiveRemainingUsageByUserIdAsync(userId);

				return Ok(new
				{
					success = true,
					data = totalRemaining
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy lượt sử dụng còn lại THEO TỪNG PACKAGE
		/// </summary>
		[HttpGet("user-remaining-usage-by-package")]
		[Authorize]
		public async Task<ActionResult<Dictionary<int, PackageUsageInfo>>> GetRemainingUsageByPackage()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var usageByPackage = await _servicesProvider.UserSubscriptionService.GetRemainingUsageByPackageAsync(userId);

				return Ok(new
				{
					success = true,
					data = usageByPackage
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy thông tin tổng hợp sử dụng theo package (chi tiết)
		/// </summary>
		[HttpGet("usage-summary-by-userId")]
		[Authorize]
		public async Task<ActionResult<List<PackageUsageSummary>>> GetUsageSummary()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var summary = await _servicesProvider.UserSubscriptionService.GetUsageSummaryAsync(userId);

				return Ok(new
				{
					success = true,
					data = summary
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		// ============== API lấy danh sách subscription ==============

		/// <summary>
		/// Lấy những subscription đang sử dụng tính đến thời điểm hiện tại
		/// (Status = true, còn trong thời hạn, còn lượt sử dụng)
		/// </summary>
		[HttpGet("user/currently-active")]
		[Authorize]
		public async Task<ActionResult<List<UserSubscriptionModel>>> GetCurrentlyActiveSubscriptions()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var subscriptions = await _servicesProvider.UserSubscriptionService
					.GetCurrentlyActiveSubscriptionsByUserIdAsync(userId);

				return Ok(new
				{
					success = true,
					message = "Successfully retrieved currently active subscriptions",
					data = subscriptions,
					count = subscriptions.Count
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy tất cả subscriptions active (Status = true) của user
		/// </summary>
		[HttpGet("user/subscriptions/active")]
		[Authorize]
		public async Task<ActionResult<List<UserSubscriptionModel>>> GetActiveSubscriptions()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var subscriptions = await _servicesProvider.UserSubscriptionService
					.GetAllActiveSubscriptionsForUserAsync(userId);

				return Ok(subscriptions);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy tất cả subscriptions inactive (Status = false) của user
		/// </summary>
		[HttpGet("user/subscriptions/inactive")]
		[Authorize]
		public async Task<ActionResult<List<UserSubscriptionModel>>> GetInactiveSubscriptions()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var subscriptions = await _servicesProvider.UserSubscriptionService
					.GetAllInactiveSubscriptionsForUserAsync(userId);

				return Ok(new
				{
					success = true,
					message = "Successfully retrieved inactive subscriptions",
					data = subscriptions,
					count = subscriptions.Count
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy tất cả subscriptions đã đăng ký (cả active và inactive) của user
		/// </summary>
		[HttpGet("user/subscriptions/all")]
		[Authorize]
		public async Task<ActionResult<List<UserSubscriptionModel>>> GetAllRegisteredSubscriptions()
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var subscriptions = await _servicesProvider.UserSubscriptionService
					.GetAllRegisteredSubscriptionsForUserAsync(userId);

				return Ok(subscriptions);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy subscriptions với phân trang và filter theo status
		/// </summary>
		/// <param name="pageIndex">Trang hiện tại (mặc định: 1)</param>
		/// <param name="pageSize">Số items mỗi trang (mặc định: 10)</param>
		/// <param name="status">Filter theo status: true (active), false (inactive), null (tất cả)</param>
		[HttpGet("user/subscriptions/paginated")]
		[Authorize]
		public async Task<ActionResult<PerHue.Application.Models.PaginatedResultV2<UserSubscriptionModel>>> GetSubscriptionsWithPagination(
			[FromQuery] int pageIndex = 1,
			[FromQuery] int pageSize = 10,
			[FromQuery] bool? status = null)
		{
			try
			{
				var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
				var result = await _servicesProvider.UserSubscriptionService
					.GetUserSubscriptionsWithFilterAsync(userId, pageIndex, pageSize, status);

				return Ok(new
				{
					success = true,
					message = "Successfully retrieved subscriptions with pagination",
					data = result
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = ex.Message });
			}
		}



	}
}
