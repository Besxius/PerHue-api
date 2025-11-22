using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.PaymentLog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentLogsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		public PaymentLogsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}
		// GET: api/<PaymentLogsController>
		[HttpGet]
		public async Task<IEnumerable<PaymentLogModel>> Get()
		{
			return await _servicesProvider.PaymentLogService.GetAllAsync();
		}

		// GET api/<PaymentLogsController>/5
		[HttpGet("{id}")]
		public async Task<ActionResult<PaymentLogModel>> Get(int id)
		{
			var model = await _servicesProvider.PaymentLogService.GetByIdAsync(id);
			if (model == null)
			{
				return NotFound();
			}
			return Ok(model);
		}
	}
}
