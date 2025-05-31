using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;

namespace PerHue.Infrastructure.Utils
{
	public class PayOSPaymentService
	{
		private readonly IConfiguration _configuration;

		public PayOSPaymentService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task<string> CreatePaymentAsync(int amount, string description, int subscriptionId)
		{
			var clientId = _configuration["PayOS:ClientId"];
			var apiKey = _configuration["PayOS:ApiKey"];
			var checksumKey = _configuration["PayOS:ChecksumKey"];
			var domain = _configuration["AppSettings:Domain"];

			var payOs = new PayOS(clientId, apiKey, checksumKey);

			var paymentLinkRequest = new PaymentData(
				orderCode: subscriptionId,
				amount: amount,
				items: null!, 
				description: description,
				returnUrl: domain + "/api/usersubscriptions/subscription/success",
				cancelUrl: domain + "/api/usersubscriptions/subscription/cancel"
				);
			var response = await payOs.createPaymentLink(paymentLinkRequest);

			return response.checkoutUrl ?? throw new Exception("Failed to create payment link.");
		}
	}
}
