using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public UsersController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login(LoginModel model)
		{
			var account = await _servicesProvider.UserService.GetUserByEmailAsync(model.Email);
			if (account is null)
				return NotFound();
			if (account.Isactive == false)
				return Accepted();
			var token = await _servicesProvider.UserService.ValidateUserAsync(model);
			if (token.Length == 0)
				return BadRequest();

			return Ok(token);
		}

		[HttpPost]
		[Route("change-password")]
		public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
		{
			if (await _servicesProvider.UserService.ChangePasswordAsync(model))
			{
				return Ok();
			}
			return BadRequest("Failed to change password.");
		}

		// GET: api/Users
		[HttpGet]
		public async Task<IEnumerable<UserModel>> GetUsers()
		{
			return await _servicesProvider.UserService.GetAllUsersAsync();
		}

		// GET: api/Users/5
		[HttpGet("{id}")]
		public async Task<ActionResult<UserModel>> GetUser(int id)
		{
			var user = await _servicesProvider.UserService.GetUserByIdAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		// PUT: api/Users/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutUser(int id, UpdateUserModel model)
		{
			try
			{
				await _servicesProvider.UserService.UpdateUserAsync(id, model);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await UserExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/Users
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		[Route("register")]
		public async Task PostUser(CreateUserModel user)
		{
			await _servicesProvider.UserService.CreateUserAsync(user);
		}

		// DELETE: api/Users/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var user = await _servicesProvider.UserService.GetUserByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			await _servicesProvider.UserService.DeleteUserAsync(user.Email);

			return NoContent();
		}

		private async Task<bool> UserExists(int id)
		{
			var result = await _servicesProvider.UserService.GetUserByIdAsync(id);
			return result is not null;
		}
	}
}
