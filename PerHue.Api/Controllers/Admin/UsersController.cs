using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Authentication;
using PerHue.Application.Models.Role;
using System.Security.Claims;
using PerHue.Application.Models;

namespace PerHue.Api.Controllers.Admin
{
	[Route("api/admin/[controller]")]
	[ApiController]
	[Authorize(Roles = "admin")] // Assuming you have role-based authorization
	public class UsersController : ControllerBase
	{
		private readonly IAdminUserService _adminUserService;
		private readonly IServicesProvider _servicesProvider;

		public UsersController(IAdminUserService adminUserService, IServicesProvider servicesProvider)
		{
			_adminUserService = adminUserService;
			_servicesProvider = servicesProvider;
		}

		/// <summary>
		/// Get paginated list of users with search and filter options
		/// </summary>
		/// <param name="searchModel">Search and pagination parameters</param>
		/// <returns>Paginated list of users</returns>
		[HttpGet("user-list")]
		public async Task<ServiceResponse<	PaginatedResultV2<AdminUserModel>>> GetUsers([FromQuery] AdminUserSearchModel searchModel)
		{
			var result = await _adminUserService.GetUsersAsync(searchModel);
			return ServiceResponse<PaginatedResultV2<AdminUserModel>>.Ok(result, "Users retrieved successfully");
		}

		/// <summary>
		/// Get user by ID
		/// </summary>
		/// <param name="id">User ID</param>
		/// <returns>User details</returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<AdminUserModel>> GetUser(int id)
		{
			try
			{
				var user = await _adminUserService.GetUserByIdAsync(id);
				if (user == null)
				{
					return NotFound(new { message = "User not found" });
				}
				return Ok(user);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while retrieving the user", error = ex.Message });
			}
		}

		/// <summary>
		/// Create a new user
		/// </summary>
		/// <param name="model">User creation model</param>
		/// <returns>Success or error response</returns>
		[HttpPost]
		public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var result = await _adminUserService.CreateUserAsync(model);
				if (!result)
				{
					return BadRequest(new { message = "Failed to create user. Email might already exist." });
				}

				return CreatedAtAction(nameof(GetUser), new { id = 0 }, new { message = "User created successfully" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while creating the user", error = ex.Message });
			}
		}

		/// <summary>
		/// Update an existing user
		/// </summary>
		/// <param name="id">User ID</param>
		/// <param name="model">User update model</param>
		/// <returns>Success or error response</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(int id, [FromBody] AdminUpdateUserModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var result = await _adminUserService.UpdateUserAsync(id, model);
				if (!result)
				{
					return NotFound(new { message = "User not found or email already exists" });
				}

				return Ok(new { message = "User updated successfully" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
			}
		}

		/// <summary>
		/// Delete a user
		/// </summary>
		/// <param name="id">User ID</param>
		/// <returns>Success or error response</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			try
			{
				var result = await _adminUserService.DeleteUserAsync(id);
				if (!result)
				{
					return NotFound(new { message = "User not found" });
				}

				return Ok(new { message = "User deleted successfully" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while deleting the user", error = ex.Message });
			}
		}

		/// <summary>
		/// Update user status (activate/deactivate)
		/// </summary>
		/// <param name="id">User ID</param>
		/// <param name="model">Status update model</param>
		/// <returns>Success or error response</returns>
		[HttpPatch("{id}/status")]
		public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UserStatusUpdateModel model)
		{
			try
			{
				var result = await _adminUserService.UpdateUserStatusAsync(id, model);
				if (!result)
				{
					return NotFound(new { message = "User not found" });
				}

				var status = model.Isactive ? "activated" : "deactivated";
				return Ok(new { message = $"User {status} successfully" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while updating user status", error = ex.Message });
			}
		}

		/// <summary>
		/// Ban a user
		/// </summary>
		/// <param name="id">User ID</param>
		/// <param name="reason">Ban reason</param>
		/// <returns>Success or error response</returns>
		[HttpPatch("{id}/ban")]
		public async Task<IActionResult> BanUser(int id)
		{
			try
			{
				var result = await _adminUserService.BanUserAsync(id, "");
				if (!result)
				{
					return NotFound(new { message = "User not found" });
				}

				return Ok(new { message = "User banned successfully" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while banning the user", error = ex.Message });
			}
		}

		/// <summary>
		/// Unban a user
		/// </summary>
		/// <param name="id">User ID</param>
		/// <returns>Success or error response</returns>
		[HttpPatch("{id}/unban")]
		public async Task<IActionResult> UnbanUser(int id)
		{
			try
			{
				var result = await _adminUserService.UnbanUserAsync(id);
				if (!result)
				{
					return NotFound(new { message = "User not found" });
				}

				return Ok(new { message = "User unbanned successfully" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while unbanning the user", error = ex.Message });
			}
		}

		/// <summary>
		/// Get all available roles
		/// </summary>
		/// <returns>List of roles</returns>
		[HttpGet("roles")]
		public async Task<ActionResult<IEnumerable<RoleModel>>> GetRoles()
		{
			try
			{
				var roles = await _adminUserService.GetAllRolesAsync();
				return Ok(roles);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while retrieving roles", error = ex.Message });
			}
		}

		/// <summary>
		/// Get current logged-in user information (stateless)
		/// </summary>
		/// <returns>Current user information</returns>
		[HttpGet]
		[Route("user-info")]
		public async Task<IActionResult> GetCurrentUserInfo()
		{
			try
			{
				// Extract user ID from JWT token claims
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
				{
					return Unauthorized(new { message = "Invalid or missing user ID in token" });
				}

				// Get user information from database
				var userInfo = await _servicesProvider.UserService.GetUserInfoAsync(userId);

				if (userInfo == null)
				{
					return NotFound(new { message = "User not found" });
				}

				// Check if user is still active
				if (!userInfo.Isactive)
				{
					return StatusCode(403, new { message = "User account is deactivated" });
				}

				return Ok(new
				{
					code = 200,
					result = new
					{
						id = userId,
						avatar = userInfo.Profilepicture,
						username = userInfo.Username,
						email = userInfo.Email,
						phoneNumber = userInfo.Phone,
						roles = new List<string> { userInfo.RoleName }
					},
					success = true,
					message = "User information retrieved successfully"
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					success = false,
					message = "An error occurred while retrieving user information",
					error = ex.Message
				});
			}
		}
	}
}