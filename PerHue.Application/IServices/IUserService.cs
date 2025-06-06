using PerHue.Application.Basic;
using PerHue.Application.Models;

namespace PerHue.Application.IServices
{
	public interface IUserService : IGenericService<UserModel>
	{
		Task<UserModel> GetByEmailAsync(string email);
		Task CreateAsync(CreateUserModel model);
		Task<bool> DeleteAsync(string email);
		Task<bool> UpdateAsync(int id, UpdateUserModel user);
		Task<bool> ChangePasswordAsync(ChangePasswordModel model);
		Task<bool> ChangePasswordAsync(int id, string newPassword);
		Task<bool> UserExistsAsync(string email);
		Task<string> GetUserPasswordAsync(string email);
		Task<string> ValidateUserAsync(LoginModel model);
	}
}
