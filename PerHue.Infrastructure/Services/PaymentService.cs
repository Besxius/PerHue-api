using AutoMapper;
using Net.payOS.Types;
using PerHue.Application.IServices;
using PerHue.Application.Models.Payment;
using PerHue.Application.Models.Payment.An;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Services
{
	internal class PaymentService : IPaymentService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly PayOSPaymentService _payOSPaymentService;
		private readonly IMapper _mapper;
		private readonly IUserSubscriptionService _subscriptionService;
		private readonly IServicePackageService _packageService;

		public PaymentService(IUnitOfWork unitOfWork, PayOSPaymentService payOSPaymentService, IMapper mapper,
			IUserSubscriptionService subscriptionService, IServicePackageService packageService
			)
		{
			_unitOfWork = unitOfWork;
			_payOSPaymentService = payOSPaymentService;
			_mapper = mapper;
			_subscriptionService = subscriptionService;
			_packageService = packageService;
		}

		public async Task<string> CreateAsync(PayOSRequestModel model)
		{
			return await _payOSPaymentService.CreatePaymentAsync(model);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
			return await _unitOfWork.PaymentRepository.RemoveAsync(entity);
		}

		public async Task<IEnumerable<PerHue.Application.Models.Payment.PaymentModel>> GetAllAsync(int userId)
		{
			var entities = await _unitOfWork.PaymentRepository.GetAllByUserIdAsync(userId);
			return _mapper.Map<IEnumerable<PerHue.Application.Models.Payment.PaymentModel>>(entities);
		}

		public async Task<IEnumerable<PerHue.Application.Models.Payment.PaymentModel>> GetAllAsync()
		{
			var entities = await _unitOfWork.PaymentRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<PerHue.Application.Models.Payment.PaymentModel>>(entities);
		}

		public async Task<PerHue.Application.Models.Payment.PaymentModel> GetByIdAsync(int id)
		{
			var entity = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
			return _mapper.Map<PerHue.Application.Models.Payment.PaymentModel>(entity);
		}


		// ========= AN LÀM

		public async Task<int> CreateSuccessPaymentInDbAsync(PerHue.Application.Models.Payment.An.CreatePaymentModel model) {
			var payment = new Payment
			{
				UserId = model.UserId,
				Amount = model.Amount,
				Description = model.Description,
				Status = PaymentStatusEnum.Success.ToString(),
				TransactionId = model.OrderCode,
				CreatedAt = DateTime.UtcNow
			};
			var createdPayment = await _unitOfWork.PaymentRepository.CreateAsync(payment);

			await _unitOfWork.SaveChangesWithTransactionAsync();

			return createdPayment;
		}

		/*public async Task<PerHue.Application.Models.Payment.An.CreatePaymentResult> CreatePaymentAsync(
		PerHue.Application.Models.Payment.An.CreatePaymentModel model,
		string returnUrl,
		string cancelUrl)
		{
			// 1. Tạo orderCode unique
			var orderCode = DateTimeOffset.Now.ToString("ffffff");
			var transactionId = $"{DateTime.Now:yyyyMMddHHmmss}U{model.UserId}P{model.ServicePackageId}";

			// 2. Tạo Payment record với status = Pending
			var payment = new Payment
			{
				UserId = model.UserId,
				ServicePackageId = model.ServicePackageId, // LƯU ServicePackageId
				Amount = model.Amount,
				Description = model.Description,
				Status = PaymentStatusEnum.Pending.ToString(),
				TransactionId = transactionId,
				CreatedAt = DateTime.Now
			};

			await _unitOfWork.PaymentRepository.CreateAsync(payment);
			await _unitOfWork.SaveChangesWithTransactionAsync();

			// 3. Tạo PaymentLog - event Created
			await CreatePaymentLogAsync(payment.Id, EventTypeEnum.Created,
				null, PaymentStatusEnum.Pending.ToString(),
				"Payment created successfully");

			// 4. Tạo payment link từ PayOS
			var payOSRequest = new PayOSRequestModel
			{
				Amount = model.Amount,
				Description = transactionId, // Dùng transactionId làm description để map về
				ReturnUrl = returnUrl,
				CancelUrl = cancelUrl
			};

			try
			{
				var paymentUrl = await _payOSPaymentService.CreatePaymentAsync(payOSRequest);

				// 5. Log thành công
				await CreatePaymentLogAsync(payment.Id, EventTypeEnum.Info,
					null, null, "Payment link created",
					new { paymentUrl, orderCode });

				return new PerHue.Application.Models.Payment.An.CreatePaymentResult
				{
					PaymentId = payment.Id,
					PaymentUrl = paymentUrl,
					TransactionId = transactionId
				};
			}
			catch (Exception ex)
			{
				// 6. Nếu lỗi, update status và log
				payment.Status = PaymentStatusEnum.Failed.ToString();
				await _unitOfWork.PaymentRepository.UpdateAsync(payment);
				await _unitOfWork.SaveChangesWithTransactionAsync();

				await CreatePaymentLogAsync(payment.Id, EventTypeEnum.Error,
					PaymentStatusEnum.Pending.ToString(),
					PaymentStatusEnum.Failed.ToString(),
					$"Failed to create payment link: {ex.Message}");

				throw;
			}
		}

		public async Task<WebhookData> VerifyWebhookAsync(WebhookType webhookData)
		{
			// Verify signature với PayOS
			return _payOSPaymentService.VerifyWebhook(webhookData);
		}

		public async Task ProcessWebhookAsync(WebhookData webhookData)
		{
			// 1. Tìm payment theo description (chứa transactionId)
			var payment = await _unitOfWork.PaymentRepository
				.GetByTransactionIdAsync(webhookData.description);

			if (payment == null)
			{
				throw new Exception($"Payment not found for transaction: {webhookData.description}");
			}

			// 2. Log webhook received
			await CreatePaymentLogAsync(payment.Id, EventTypeEnum.WebhookReceived,
				null, null, "Webhook received from PayOS",
				new { orderCode = webhookData.orderCode, status = webhookData.status });

			var oldStatus = payment.Status;
			var newStatus = MapPayOSStatusToPaymentStatus(webhookData.status);

			// 3. Kiểm tra nếu status không thay đổi thì skip
			if (oldStatus == newStatus)
			{
				return;
			}

			// 4. Update payment status
			payment.Status = newStatus;
			payment.UpdatedAt = DateTime.Now;
			await _unitOfWork.PaymentRepository.UpdateAsync(payment);

			// 5. Log status changed
			await CreatePaymentLogAsync(payment.Id, EventTypeEnum.StatusChanged,
				oldStatus, newStatus, $"Payment status changed from {oldStatus} to {newStatus}");

			// 6. Nếu thanh toán thành công → Xử lý UserSubscription
			if (newStatus == PaymentStatusEnum.Success.ToString())
			{
				await ProcessSuccessfulPaymentAsync(payment);
			}
			// 7. Nếu bị cancel hoặc failed
			else if (newStatus == PaymentStatusEnum.Cancelled.ToString() ||
					 newStatus == PaymentStatusEnum.Failed.ToString())
			{
				await CreatePaymentLogAsync(payment.Id, EventTypeEnum.Info,
					null, null, $"Payment {newStatus.ToLower()}, no subscription created");
			}

			await _unitOfWork.SaveChangesWithTransactionAsync();
		}

		private async Task ProcessSuccessfulPaymentAsync(Payment payment)
		{
			try
			{
				// 1. Lấy ServicePackageId từ Payment (ĐÃ LƯU SẴN)
				var servicePackage = await _packageService.GetByIdAsync(payment.ServicePackageId);

				// 2. Kiểm tra UserSubscription cũ cùng loại và ServicePackageId
				var existingSubscription = await _unitOfWork.UserSubscriptionRepository
					.GetActiveSubscriptionByUserAndPackageAsync(payment.UserId, payment.ServicePackageId);

				if (existingSubscription != null)
				{
					// Disable gói cũ
					existingSubscription.Status = false;
					existingSubscription.EndDate = DateTime.Now;
					await _unitOfWork.UserSubscriptionRepository.UpdateAsync(existingSubscription);

					await CreatePaymentLogAsync(payment.Id, EventTypeEnum.Info,
						null, null,
						$"Disabled old subscription (ID: {existingSubscription.Id}) before creating new one");
				}

				// 3. Tạo UserSubscription mới
				var subscription = new UserSubscription
				{
					UserId = payment.UserId,
					ServicePackageId = payment.ServicePackageId,
					Status = true, // Active
					RemainingUses = servicePackage.Uses,
					CreateAt = DateTime.Now,
					StartDate = DateTime.Now,
					EndDate = DateTime.Now.AddDays(servicePackage.Duration ?? 0),
					Id = payment.Id // 1-1 relationship
				};

				await _unitOfWork.UserSubscriptionRepository.CreateAsync(subscription);

				// 4. Log thành công
				await CreatePaymentLogAsync(payment.Id, EventTypeEnum.Info,
					null, null,
					"Subscription created successfully",
					new
					{
						subscriptionId = subscription.Id,
						packageName = servicePackage.Name,
						remainingUses = subscription.RemainingUses,
						endDate = subscription.EndDate
					});
			}
			catch (Exception ex)
			{
				// Log error nhưng không throw để không block webhook
				await CreatePaymentLogAsync(payment.Id, EventTypeEnum.Error,
					null, null,
					$"Error creating subscription: {ex.Message}");
			}
		}

		private async Task CreatePaymentLogAsync(
			int paymentId,
			EventTypeEnum eventType,
			string? oldStatus,
			string? newStatus,
			string message,
			object? metadata = null)
		{
			var log = new PaymentLog
			{
				PaymentId = paymentId,
				EventType = eventType.ToString(),
				OldStatus = oldStatus,
				NewStatus = newStatus,
				Mesage = message,
				CreatedAt = DateTime.Now,
				Metadata = metadata != null ? System.Text.Json.JsonSerializer.Serialize(metadata) : null
			};

			await _unitOfWork.PaymentLogRepository.CreateAsync(log);
		}

		private string MapPayOSStatusToPaymentStatus(string payOSStatus)
		{
			return payOSStatus.ToUpper() switch
			{
				"PAID" => PaymentStatusEnum.Success.ToString(),
				"CANCELLED" => PaymentStatusEnum.Cancelled.ToString(),
				"PENDING" => PaymentStatusEnum.Pending.ToString(),
				"PROCESSING" => PaymentStatusEnum.Processing.ToString(),
				_ => PaymentStatusEnum.Failed.ToString()
			};
		}*/
	}
}
