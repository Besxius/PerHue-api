using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Infrastructure.Utils;

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
		public async Task<IEnumerable<UserSubscriptionModel>> Gets()
		{
			return await _servicesProvider.UserSubscriptionService.GetAllAsync();
		}

		[HttpGet("{id}")]
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
			if (User.FindFirst("UserId") == null)
			{
				throw new UnauthorizedAccessException("User is not authenticated.");
			}
			var model = new CreateUserSubscriptionModel
			{
				//UserId = 2,
				UserId = int.Parse(User.FindFirst("UserId")!.Value),
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

		[HttpGet("subscription/success")]
		public async Task<IActionResult> SubscriptionSuccess(
			[FromQuery] string code,
			[FromQuery] string id,
			[FromQuery] bool cancel,
			[FromQuery] string status,
			[FromQuery] string orderCode
			)
		{
			var paymentInfo = await _payOSPaymentService.GetPaymentRequestInformationAsync(long.Parse(orderCode));
			var servicePackage = await _servicesProvider.ServicePackageService.GetByAmountAsync(paymentInfo.amount);

			var model = new CreateUserSubscriptionModel
			{
				//UserId = 2,
				UserId = int.Parse(User.FindFirst("UserId")!.Value),
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
			[FromQuery] string orderCode
			)
		{
			var paymentInfo = await _payOSPaymentService.GetPaymentRequestInformationAsync(long.Parse(orderCode));
			var servicePackage = await _servicesProvider.ServicePackageService.GetByAmountAsync(paymentInfo.amount);

			var model = new CreateUserSubscriptionModel
			{
				//UserId = 2,
				UserId = int.Parse(User.FindFirst("UserId")!.Value),
				ServicePackageId = servicePackage.Id,
				Status = true,
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

	}
}
