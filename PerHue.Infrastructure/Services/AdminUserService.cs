using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.Role;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Services
{
	internal class AdminUserService : IAdminUserService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public AdminUserService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<PaginatedResultV2<AdminUserModel>> GetUsersAsync(AdminUserSearchModel searchModel)
		{
			var baseQuery = _unitOfWork.UserRepository.GetQueryable()
				.Include(u => u.Role);

			IQueryable<UserAccount> query = baseQuery;
			if (!string.IsNullOrEmpty(searchModel.Phone))
			{
				query = query.Where(u => u.Phone != null && u.Phone.Contains(searchModel.Phone));
			}

			if (!string.IsNullOrEmpty(searchModel.Email))
			{
				query = query.Where(u => u.Email.Contains(searchModel.Email));
			}

			if (!string.IsNullOrEmpty(searchModel.Fullname))
			{
				query = query.Where(u => u.Fullname != null && u.Fullname.Contains(searchModel.Fullname));
			}

			if (!string.IsNullOrEmpty(searchModel.RoleName))
			{
				query = query.Where(u => u.Role.Name.Contains(searchModel.RoleName));
			}

			// Apply status filter
			if (searchModel.IsActive.HasValue)
			{
				query = query.Where(u => u.IsActive == searchModel.IsActive.Value);
			}

			// Apply sorting
			IOrderedQueryable<UserAccount> orderedQuery;
			switch (searchModel.SortBy)
			{
				case UserSortBy.Username:
					orderedQuery = searchModel.SortOrder == SortOrder.Ascending
						? query.OrderBy(u => u.Username)
						: query.OrderByDescending(u => u.Username);
					break;
				case UserSortBy.Email:
					orderedQuery = searchModel.SortOrder == SortOrder.Ascending
						? query.OrderBy(u => u.Email)
						: query.OrderByDescending(u => u.Email);
					break;
				case UserSortBy.Fullname:
					orderedQuery = searchModel.SortOrder == SortOrder.Ascending
						? query.OrderBy(u => u.Fullname)
						: query.OrderByDescending(u => u.Fullname);
					break;
				case UserSortBy.AccountId:
					orderedQuery = searchModel.SortOrder == SortOrder.Ascending
						? query.OrderBy(u => u.Id)
						: query.OrderByDescending(u => u.Id);
					break;
				case UserSortBy.CreatedDate:
				default:
					orderedQuery = searchModel.SortOrder == SortOrder.Ascending
						? query.OrderBy(u => u.Id) // Assuming Id represents creation order
						: query.OrderByDescending(u => u.Id);
					break;
			}

			var totalCount = await query.CountAsync();
			var totalPages = (int)Math.Ceiling((double)totalCount / searchModel.PageSize);

			var users = await orderedQuery
				.Skip((searchModel.PageIndex - 1) * searchModel.PageSize)
				.Take(searchModel.PageSize)
				.Select(u => new AdminUserModel
				{
					Id = u.Id,
					Email = u.Email,
					Username = u.Username,
					Fullname = u.Fullname,
					Phone = u.Phone,
					Gender = u.Gender,
					Dob = u.Dob,
					Isactive = u.IsActive,
					Profilepicture = u.ProfilePicture,
					Isaitested = u.IsAitested,
					RoleId = u.RoleId,
					RoleName = u.Role.Name,
					CreatedDate = DateTime.Now, // You might need to add this field to UserAccount entity
					UpdatedDate = null
				})
				.ToListAsync();

			return new PaginatedResultV2<AdminUserModel>
			{
				List = users,
				Total = totalCount,
				Current = searchModel.PageIndex,
			};
		}

		public async Task<AdminUserModel?> GetUserByIdAsync(int id)
		{
			var user = await _unitOfWork.UserRepository.GetQueryable()
				.Include(u => u.Role)
				.FirstOrDefaultAsync(u => u.Id == id);

			if (user == null) return null;

			return new AdminUserModel
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
				RoleName = user.Role.Name,
				CreatedDate = DateTime.Now,
				UpdatedDate = null
			};
		}

		public async Task<bool> CreateUserAsync(AdminCreateUserModel model)
		{
			try
			{
				// Check if email already exists
				var existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(model.Email);
				if (existingUser != null)
					return false;

				var entity = new UserAccount
				{
					Email = model.Email,
					Username = model.Username,
					Password = HashPassWithSHA256.HashWithSHA256(model.Password),
					Fullname = model.Fullname,
					Phone = model.Phone,
					Gender = model.Gender,
					Dob = model.Dob,
					IsActive = model.Isactive,
					ProfilePicture = model.Profilepicture,
					IsAitested = false,
					RoleId = model.RoleId
				};

				await _unitOfWork.UserRepository.CreateAsync(entity);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> UpdateUserAsync(int id, AdminUpdateUserModel model)
		{
			try
			{
				var entity = await _unitOfWork.UserRepository.GetByIdAsync(id);
				if (entity == null)
					return false;

				// Check if email is being changed and if it already exists
				if (entity.Email != model.Email)
				{
					var existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(model.Email);
					if (existingUser != null)
						return false;
				}

				entity.Email = model.Email;
				entity.Username = model.Username;
				entity.Fullname = model.Fullname;
				entity.Phone = model.Phone;
				entity.Gender = model.Gender;
				entity.Dob = model.Dob;
				entity.IsActive = model.Isactive;
				entity.ProfilePicture = model.Profilepicture;
				entity.RoleId = model.RoleId;

				// Update password if provided
				if (!string.IsNullOrEmpty(model.NewPassword))
				{
					entity.Password = HashPassWithSHA256.HashWithSHA256(model.NewPassword);
				}

				await _unitOfWork.UserRepository.UpdateAsync(entity);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> DeleteUserAsync(int id)
		{
			try
			{
				var entity = await _unitOfWork.UserRepository.GetByIdAsync(id);
				if (entity == null)
					return false;

				return await _unitOfWork.UserRepository.RemoveAsync(entity);
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> UpdateUserStatusAsync(int id, UserStatusUpdateModel model)
		{
			try
			{
				var entity = await _unitOfWork.UserRepository.GetByIdAsync(id);
				if (entity == null)
					return false;

				entity.IsActive = model.Isactive;
				await _unitOfWork.UserRepository.UpdateAsync(entity);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> BanUserAsync(int id, string reason)
		{
			return await UpdateUserStatusAsync(id, new UserStatusUpdateModel { Isactive = false, Reason = reason });
		}

		public async Task<bool> UnbanUserAsync(int id)
		{
			return await UpdateUserStatusAsync(id, new UserStatusUpdateModel { Isactive = true });
		}

		public async Task<IEnumerable<RoleModel>> GetAllRolesAsync()
		{
			var roles = await _unitOfWork.RoleRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<RoleModel>>(roles);
		}
	}
}