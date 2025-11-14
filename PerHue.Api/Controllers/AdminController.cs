using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.User;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdminController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public AdminController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		[HttpGet]
		public async Task<IEnumerable<UserModel>> GetUsers()
		{
			return await _servicesProvider.UserService.GetAllAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<UserModel>> GetUser(int id)
		{
			var user = await _servicesProvider.UserService.GetByIdAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}
	}
}
