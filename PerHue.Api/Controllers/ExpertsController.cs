using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.Expert;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExpertsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public ExpertsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		[HttpGet("ranking")]
		public async Task<ActionResult<IEnumerable<ExpertModel>>> GetExpertsByRating()
		{
			var experts = await _servicesProvider.ExpertService.GetAllByRatingDescendingAsync();
			return Ok(experts);
		}

		// You can also add the GetAll endpoint here if needed for clearer API structure
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ExpertModel>>> GetAllExperts()
		{
			var experts = await _servicesProvider.ExpertService.GetAllAsync();
			return Ok(experts);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ExpertModel>> GetExpertById(int id)
		{
			var expert = await _servicesProvider.ExpertService.GetByIdAsync(id);
			if (expert == null)
			{
				return NotFound();
			}
			return Ok(expert);
		}
	}
}