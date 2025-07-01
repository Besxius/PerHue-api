using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Authentication;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Services
{
	internal class UserService : IUserService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly JwtProvider _jwtProvider;

		public UserService(IUnitOfWork unitOfWork, IMapper mapper, JwtProvider jwtProvider)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_jwtProvider = jwtProvider;
		}
		public async Task<bool> ChangePasswordAsync(ChangePasswordModel model)
		{
			var user = await _unitOfWork.UserRepository.GetByIdAsync(model.Id);
			if (user is null)
				return false;
			if (model.NewPassword != model.OldPassword)
			{
				user.Password = model.NewPassword;
				await _unitOfWork.UserRepository.UpdateAsync(user);
				return true;
			}
			return false;
		}

		public async Task<bool> ChangePasswordAsync(int id, string newPassword)
		{
			var entity = await _unitOfWork.UserRepository.GetByIdAsync(id);
			if (entity is null)
				return false;
			entity.Password = newPassword;
			await _unitOfWork.UserRepository.UpdateAsync(entity);
			return true;
		}

		public async Task CreateAsync(CreateUserModel model)
		{
			var entity = _mapper.Map<UserAccount>(model);
			entity.Username = GenerateUserName(model.Email);
			entity.IsActive = true;
			entity.IsAitested = false;
			entity.RoleId = 3;

			await _unitOfWork.UserRepository.CreateAsync(entity);
		}
		public async Task CreateAsync(CreateUserByEmailModel model)
		{
			var entity = _mapper.Map<UserAccount>(model);
			entity.Password = PerHueDefaultPassword.PerHueDefaultPassword166203.ToString();
			entity.Username = model.Fullname;
			entity.Gender = model.Gender;
			entity.IsActive = true;
			entity.IsAitested = false;
			entity.RoleId = 3;

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
			entity.ProfilePicture = user.ProfilePicture;

			await _unitOfWork.UserRepository.UpdateAsync(entity);
			return true;
		}

		public async Task<bool> UserExistsAsync(string email)
		{
			var entity = await _unitOfWork.UserRepository.GetByIdAsync(email);
			return entity is not null;
		}

		public async Task<string> ValidateUserAsync(LoginModel model)
		{
			var entity = await _unitOfWork.UserRepository.GetByEmailAsync(model.Email);
			if (entity.Password != model.Password)
				return string.Empty;

			var token = _jwtProvider.GenerateToken(entity);
			return token;
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

		private string GenerateUserName(string email)
		{
			var userName = email.Split('@')[0];
			return userName;
		}
	}
}
