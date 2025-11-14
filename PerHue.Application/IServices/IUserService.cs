using PerHue.Application.Basic;
using PerHue.Application.Models.Authentication;
using PerHue.Application.Models.User;
using PerHue.Domain.Entities;

namespace PerHue.Application.IServices
{
	public interface IUserService : IGenericService<UserModel>
	{
		Task<UserModel> GetByEmailAsync(string email);
		Task CreateAsync(CreateUserRequestModel model);
		Task<bool> DeleteAsync(string email);
		Task<bool> UpdateAsync(int id,UpdateUserModel user);
		Task<bool> ChangePasswordAsync(ChangePasswordModel model);
		Task<bool> ChangePasswordAsync(int id, string newPassword);
		Task<bool> UserExistsAsync(string email);
		Task<string> GetUserPasswordAsync(string email);

		//Task<string> ValidateUserAsync(LoginRequestModel model);
		Task<string> ValidateUserAsync(string email);
		Task CreateAsync(CreateUserByEmailModel model);
		Task<UserModel> CreateOrLinkGoogleUserAsync(string email, string name, string picture);

		string GenerateTokenForUser(UserModel model);

		Task<LoginResponseModel> ValidateUserAsync(LoginRequestModel model);
		Task<LoginResponseModel> RefreshTokenAsync(RefreshTokenRequestModel model);
	}
}
