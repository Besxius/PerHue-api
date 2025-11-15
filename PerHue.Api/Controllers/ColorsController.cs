using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Color;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ColorsController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		public ColorsController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}
		// GET: api/<ColorsController>
		[HttpGet]
		public async Task<PaginatedResult<ColorModel>> Get(int? pageIndex = 1, int? pageSize = 30, string? searchTerm = "")
		{
			return await _servicesProvider.ColorService.GetAllAsync(pageIndex ?? 1, pageSize ?? 30, searchTerm);
		}

		// GET api/<ColorsController>/5
		[HttpGet("{id}")]
		public async Task<ColorModel> Get(int id)
		{
			return await _servicesProvider.ColorService.GetByIdAsync(id);
		}
	}
}
