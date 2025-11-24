using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models.Authentication;
using PerHue.Application.Models.User;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Authentication;
using PerHue.Infrastructure.Utils;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PerHue.Application.Models;

namespace PerHue.Infrastructure.Services
{
	internal class UserService : IUserService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly JwtProvider _jwtProvider;
		private readonly IOtpService _otpService;

		public UserService(IUnitOfWork unitOfWork, IMapper mapper, JwtProvider jwtProvider, IOtpService otpService)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_jwtProvider = jwtProvider;
			_otpService = otpService;
		}
		public async Task<bool> ChangePasswordAsync(ChangePasswordModel model)
		{
			var user = await _unitOfWork.UserRepository.GetByIdAsync(model.Id);
			bool isValid = _otpService.VerifyOtp(model.SentEmail, model.Otp);
			if (!isValid)
			{
				Console.WriteLine("Invalid OTP.");
				return false;

			}
			if (user is null)
				return false;
			/*if (model.NewPassword != model.OldPassword)
			{*/
				//user.Password = model.NewPassword;
				var HashPassword = HashPassWithSHA256.HashWithSHA256(model.NewPassword);
				user.Password = HashPassword;
				await _unitOfWork.UserRepository.UpdateAsync(user);
				return true;
			/*}*/
			/*return false;*/
		}

		public async Task<bool> ChangePasswordAsync(int id, string newPassword)
		{
			var entity = await _unitOfWork.UserRepository.GetByIdAsync(id);
			if (entity is null)
				return false;
			//entity.Password = newPassword;
			var HashPassword = HashPassWithSHA256.HashWithSHA256(newPassword);
			entity.Password = HashPassword;
			await _unitOfWork.UserRepository.UpdateAsync(entity);
			return true;
		}

		public async Task CreateAsync(CreateUserRequestModel model)
		{
			var entity = _mapper.Map<UserAccount>(model);
			entity.Username = GenerateUserName(model.Email);
			entity.Password = HashPassWithSHA256.HashWithSHA256(model.Password);
			entity.IsActive = true;
			entity.IsAitested = false;
			entity.RoleId = 2;

			await _unitOfWork.UserRepository.CreateAsync(entity);
		}
		public async Task CreateAsync(CreateUserByEmailModel model)
		{
			var entity = _mapper.Map<UserAccount>(model);
			entity.Password = PerHueDefaultPassword.PerHueDefaultPasswordFA25SE166.ToString();
			entity.Username = GenerateUserName(model.Email);
			entity.Gender = model.Gender;
			entity.IsActive = true;
			entity.IsAitested = false;
			entity.RoleId = 2;

			await _unitOfWork.UserRepository.CreateAsync(entity);
		}

		public async Task<bool> DeleteAsync(string email)
		{
			return await _unitOfWork.UserRepository.DeleteByEmailAsync(email);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _unitOfWork.UserRepository.GetByIdAsync(id);
			return await _unitOfWork.UserRepository.RemoveAsync(entity);
		}

		public async Task<IEnumerable<UserModel>> GetAllAsync()
		{
			var entities = await _unitOfWork.UserRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<UserModel>>(entities);
		}

		public async Task<UserModel> GetByEmailAsync(string email)
		{
			var entity = await _unitOfWork.UserRepository.GetByEmailAsync(email);
			return _mapper.Map<UserModel>(entity);
		}

		public async Task<UserModel> GetByIdAsync(int id)
		{
			var entity = await _unitOfWork.UserRepository.GetByIdAsync(id);
			return _mapper.Map<UserModel>(entity);
		}

		public async Task<string> GetUserPasswordAsync(string email)
		{
			var entity = await _unitOfWork.UserRepository.GetByEmailAsync(email);
			return entity.Password;
		}

		public async Task<bool> UpdateAsync(int id, UpdateUserModel user)
		{
			var entity = await _unitOfWork.UserRepository.GetByIdAsync(id);
			if (entity is null)
				return false;

			entity.Fullname = user.Fullname;
			entity.Phone = user.Phone;
			entity.Gender = user.Gender;
			entity.Dob = user.Dob;
			entity.ProfilePicture = user.Profilepicture;

			await _unitOfWork.UserRepository.UpdateAsync(entity);
			return true;
		}

		public async Task<bool> UserExistsAsync(string email)
		{
			var entity = await _unitOfWork.UserRepository.GetByIdAsync(email);
			return entity is not null;
		}

		/*public async Task<string> ValidateUserAsync(LoginRequestModel model)
		{
			var entity = await _unitOfWork.UserRepository.GetByEmailAsync(model.Email);
			//var HashPass = HashPassWithSHA256.HashWithSHA256(model.Password);
			//if (entity.Password != HashPass)
			//	return string.Empty;

			var token = _jwtProvider.GenerateToken(entity);
			return token;
		}*/

		public async Task<LoginResponseModel> ValidateUserAsync(LoginRequestModel model)
		{
			var entity = await _unitOfWork.UserRepository.GetByEmailAsync(model.Email);
			//var HashPass = HashPassWithSHA256.HashWithSHA256(model.Password);
			//if (entity == null || entity.Password != HashPass)
			//	throw new SecurityTokenException("Invalid email or password");

			// Generate tokens
			var accessToken = _jwtProvider.GenerateToken(entity);
			var refreshToken = _jwtProvider.GenerateRefreshToken();

			// Save Refresh Token to DB
			var refreshTokenEntity = new RefreshToken
			{
				Token = refreshToken,
				ExpireDate = DateTime.UtcNow.AddDays(7), // Set refresh token expiry (e.g., 7 days)
				UserAccountId = entity.Id
			};

			await _unitOfWork.RefreshTokenRepository.CreateAsync(refreshTokenEntity);
			await _unitOfWork.SaveChangesWithTransactionAsync();

			return new LoginResponseModel
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken
			};
		}

		public async Task<string> ValidateUserAsync(string email)
		{
			var entity = await _unitOfWork.UserRepository.GetByEmailAsync(email);

			//if (entity is null)
			//{
			//	entity = new UserAccount
			//	{
			//		Email = email,
			//		Password = "PerHuedefaultPassword166203", 
			//		Username = GenerateUserName(email),
			//		IsActive = true,
			//		IsAitested = false,
			//		RoleId = 2,
			//		Role = await _unitOfWork.RoleRepository.GetByIdAsync(2),
			//	};
			//	await _unitOfWork.UserRepository.CreateAsync(entity);
			//}
			var token = _jwtProvider.GenerateToken(entity);

			return token;
		}
		
		


		public async Task<UserModel> CreateOrLinkGoogleUserAsync(string email, string name, string picture)
		{
			if (string.IsNullOrEmpty(email))
			{
				return null;
			}

			var existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(email);

			if (existingUser != null)
			{
				return _mapper.Map<UserModel>(existingUser);
			}

			var newUser = new UserAccount
			{
				Email = email,
				Username = GenerateUserName(email),
				Password = HashPassWithSHA256.HashWithSHA256(PerHueDefaultPassword.PerHueDefaultPasswordFA25SE166.ToString()),
				Fullname = name,
				Gender = false,
				ProfilePicture = picture,
				IsActive = true,
				IsAitested = false,
				RoleId = 2,
			};

			await _unitOfWork.UserRepository.CreateAsync(newUser);

			var userModel = _mapper.Map<UserModel>(newUser);

			return userModel;
		}
		public async Task<LoginResponseModel> RefreshTokenAsync(RefreshTokenRequestModel model)
		{
			var principal = _jwtProvider.GetPrincipalFromExpiredToken(model.AccessToken);
			var userIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);

			if (!int.TryParse(userIdString, out var userId))
				throw new SecurityTokenException("Invalid token claims");

			var storedRefreshToken = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(model.RefreshToken);

			if (storedRefreshToken == null)
				throw new SecurityTokenException("Refresh token not found");

			if (storedRefreshToken.UserAccountId != userId)
				throw new SecurityTokenException("Refresh token mismatch");

			if (storedRefreshToken.ExpireDate <= DateTime.UtcNow)
				throw new SecurityTokenException("Refresh token expired");

			// All checks passed. Generate new tokens.
			var user = storedRefreshToken.UserAccount;
			var newAccessToken = _jwtProvider.GenerateToken(user);
			var newRefreshToken = _jwtProvider.GenerateRefreshToken();

			// Rotate the refresh token: update the old one with the new value and expiry
			storedRefreshToken.Token = newRefreshToken;
			storedRefreshToken.ExpireDate = DateTime.UtcNow.AddDays(7);
			await _unitOfWork.RefreshTokenRepository.UpdateAsync(storedRefreshToken);
			await _unitOfWork.SaveChangesWithTransactionAsync();

			return new LoginResponseModel
			{
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken
			};
		}
		public string GenerateTokenForUser(UserModel model)
		{
			if (model == null)
			{
				return string.Empty;
			}
			var entity = _mapper.Map<UserAccount>(model);

			// Sử dụng _jwtProvider đã được inject để tạo JWT Token
			var token = _jwtProvider.GenerateToken(entity);
			return token;
		}

		private string GenerateUserName(string email)
		{
			var userName = email.Split('@')[0];
			return userName;
		}

		public async Task<UserInfoModel?> GetUserInfoAsync(int userId)
		{
			var user = await _unitOfWork.UserRepository.GetQueryable()
				.Include(u => u.Role)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null) return null;

			return new UserInfoModel
			{
				Id = user.Id,
				Email = user.Email,
				Username = user.Username,
				Fullname = user.Fullname,
				Phone = user.Phone,
				Gender = user.Gender,
				Dob = user.Dob,
				Isactive = user.IsActive,
				Profilepicture = user.ProfilePicture,
				Isaitested = user.IsAitested,
				RoleId = user.RoleId,
				RoleName = user.Role.Name
			};
		}
	}
}
