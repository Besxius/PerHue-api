using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.Payment;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		public PaymentsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}
		// GET: api/<PaymentsController>
		[HttpGet]
		public async Task<IEnumerable<PaymentModel>> Get()
		{
			return await _servicesProvider.PaymentService.GetAllAsync();
		}

		// GET api/<PaymentsController>/5
		[HttpGet("{id}")]
		public async Task<ActionResult<PaymentModel>> Get(int id)
		{
			var payment = await _servicesProvider.PaymentService.GetByIdAsync(id);
			if (payment == null)
			{
				return NotFound();
			}
			return Ok(payment);
		}
	}
}
