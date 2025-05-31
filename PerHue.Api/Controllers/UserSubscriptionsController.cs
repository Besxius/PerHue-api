using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Infrastructure.Utils;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserSubscriptionsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		public UserSubscriptionsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}
		[HttpGet]
		public async Task<IEnumerable<UserSubscriptionModel>> Gets()
		{
			return await _servicesProvider.UserSubscriptionService.GetUserSubscriptionModels();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<UserSubscriptionModel>> Get(int id)
		{
			var model = await _servicesProvider.UserSubscriptionService.GetUserSubscriptionByIdAsync(id);
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
		public async Task<string> Post([FromRoute] int packageId)
		{
			var model = new CreateUserSubscriptionModel
			{
				UserId = 2,
				//UserId = int.Parse(User.FindFirst("UserId")!.Value),
				ServicePackageId = packageId,
			};
			var subscriptionId = await _servicesProvider.UserSubscriptionService.CreateUserSubscriptionAsync(model);

			var package = await _servicesProvider.ServicePackageService.GetServicePackageByIdAsync(packageId);
			var description = CreateDateTimeStringNoSeparator(DateTime.Now) + $"U{model.UserId}P{model.ServicePackageId}";

			var paymentModel = new PayOSRequestModel
			{
				Amount = package.Price,
				Description = description,
				UserSubscriptionId = subscriptionId
			};
			var paymentUrl = await _servicesProvider.PaymentService.CreatePaymentAsync(paymentModel);
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
			await _servicesProvider.UserSubscriptionService.UpdateUserSubscriptionAsync(int.Parse(orderCode), UserSubscriptionStatusEnum.Active.ToString());

			return Ok(new
			{
				Message = "Đã nhận dữ liệu thành công",
				ReceivedData = new { code, id, cancel, status, orderCode }
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
			await _servicesProvider.UserSubscriptionService.UpdateUserSubscriptionAsync(int.Parse(orderCode), UserSubscriptionStatusEnum.Cancelled.ToString());

			return Ok(new
			{
				Message = "Đã nhận dữ liệu thành công",
				ReceivedData = new { code, id, cancel, status, orderCode }
			});
		}

		private string CreateDateTimeStringNoSeparator(DateTime dateTime)
		{
			return dateTime.ToString("yyyyMMddHHmmss");
		}

	}
}
