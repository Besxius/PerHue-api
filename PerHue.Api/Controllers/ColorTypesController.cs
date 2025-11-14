using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.ColorType;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ColorTypesController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		public ColorTypesController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}
		[HttpGet]
		public async Task<IEnumerable<ColorTypeModel>> Get()
		{
			return await _servicesProvider.ColorTypeService.GetAllAsync();
		}

		[HttpGet("{id}")]
		public async Task<ColorTypeModel> Get(int id)
		{
			return await _servicesProvider.ColorTypeService.GetByIdAsync(id);
		}
	}
}
