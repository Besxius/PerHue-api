using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using PerHue.Application.Models.Payment;

namespace PerHue.Infrastructure.Utils
{
	public class PayOSPaymentService
	{
		private readonly IConfiguration _configuration;

		public PayOSPaymentService(IConfiguration configuration)		{
			_configuration = configuration;
		}

		public async Task<string> CreatePaymentAsync(PayOSRequestModel model)
		{
			var clientId = _configuration["PayOS:ClientId"];
			var apiKey = _configuration["PayOS:ApiKey"];
			var checksumKey = _configuration["PayOS:ChecksumKey"];

			var payOs = new PayOS(clientId, apiKey, checksumKey);

			var paymentLinkRequest = new PaymentData(
				orderCode: int.Parse(DateTimeOffset.Now.ToString("ffffff")),
				amount: model.Amount,
				items: null!,
				description: model.Description,
				returnUrl: model.ReturnUrl,
				cancelUrl: model.CancelUrl
				);
			var response = await payOs.createPaymentLink(paymentLinkRequest);

			return response.checkoutUrl ?? throw new Exception("Failed to create payment link.");
		}

		public async Task<PaymentLinkInformation> GetPaymentRequestInformationAsync(long id)
		{
			var clientId = _configuration["PayOS:ClientId"];
			var apiKey = _configuration["PayOS:ApiKey"];
			var checksumKey = _configuration["PayOS:ChecksumKey"];

			var payOs = new PayOS(clientId, apiKey, checksumKey);
			var paymentInformation = await payOs.getPaymentLinkInformation(id);
			return paymentInformation;
		}
	}
}
