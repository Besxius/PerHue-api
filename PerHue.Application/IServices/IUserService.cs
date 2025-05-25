using PerHue.Application.Models;

namespace PerHue.Application.IServices
{
	public interface IUserService
	{
		Task<IEnumerable<UserModel>> GetAllUsersAsync();
		Task<UserModel> GetUserByIdAsync(int id);
		Task<UserModel> GetUserByEmailAsync(string email);
		Task CreateUserAsync(CreateUserModel model);
		Task DeleteUserAsync(string email);
		Task DeleteUserAsync(int id);
		Task<bool> ChangePasswordAsync(ChangePasswordModel model);
		Task<bool> ChangePasswordAsync(int id, string newPassword);
		Task<bool> UpdateUserAsync(int id, UpdateUserModel user);
		Task<bool> UserExistsAsync(string email);
		Task<string> GetUserPasswordAsync(string email);
		Task<string> ValidateUserAsync(LoginModel model);
	}
}
