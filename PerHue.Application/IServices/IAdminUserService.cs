using PerHue.Application.Models;

namespace PerHue.Application.IServices
{
	public interface IAdminUserService
	{
		Task<PaginatedResultV2<AdminUserModel>> GetUsersAsync(AdminUserSearchModel searchModel);
		Task<AdminUserModel?> GetUserByIdAsync(int id);
		Task<bool> CreateUserAsync(AdminCreateUserModel model);
		Task<bool> UpdateUserAsync(int id, AdminUpdateUserModel model);
		Task<bool> DeleteUserAsync(int id);
		Task<bool> UpdateUserStatusAsync(int id, UserStatusUpdateModel model);
		Task<bool> BanUserAsync(int id, string reason);
		Task<bool> UnbanUserAsync(int id);
		Task<IEnumerable<RoleModel>> GetAllRolesAsync();
	}
}