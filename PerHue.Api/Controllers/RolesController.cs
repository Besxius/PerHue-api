using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.Role;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RolesController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;
		public RolesController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}
		// GET: api/<RolesController>
		[HttpGet]
		public async Task<IEnumerable<RoleModel>> Get()
		{
			return await _servicesProvider.RoleService.GetAllAsync();
		}

		// GET api/<RolesController>/5
		[HttpGet("{id}")]
		public async Task<ActionResult<RoleModel>> Get(int id)
		{
			var model = await _servicesProvider.RoleService.GetByIdAsync(id);

			if (model == null)
			{
				return NotFound();
			}
			return Ok(model);
		}
	}
}
