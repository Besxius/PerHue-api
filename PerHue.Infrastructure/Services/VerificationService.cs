using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Application.Models.Notification;
using PerHue.Application.Models.VerifyInformation;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Services
{
	public class VerificationService : IVerificationService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IImageUploadService _imageUploadService;
		private readonly INotificationService _notificationService;
		private readonly EmailService _emailService;
		private readonly IDateTimeService _dateTimeService;

		public VerificationService(IUnitOfWork unitOfWork, IImageUploadService imageUploadService, INotificationService notificationService, EmailService emailService, IDateTimeService dateTimeService)
		{
			_unitOfWork = unitOfWork;
			_imageUploadService = imageUploadService;
			_notificationService = notificationService;
			_emailService = emailService;
			_dateTimeService = dateTimeService;
		}

		public async Task<PaginatedResultV2<VerifyRequestModel>> GetAllAsync(VerificationSearchModel searchModel)
		{
			var baseQuery = _unitOfWork
					.VerificationRepository
					.GetQueryable()
					.Include(v => v.IdNavigation);

			IQueryable<VerifyInformation> query = baseQuery;

			// Apply search
			if (!string.IsNullOrEmpty(searchModel.SearchTerm))
			{
				var searchTerm = searchModel.SearchTerm.ToLower();
				switch (searchModel.SearchBy?.ToLower())
				{
					case "id":
						if (int.TryParse(searchTerm, out int id))
						{
							query = query.Where(v => v.Id == id);
						}
						else
						{
							query = query.Where(v => false); // No results if parsing fails
						}
						break;
					case "email":
						query = query.Where(v => v.Email.ToLower().Contains(searchTerm));
						break;
					case "nickname":
						query = query.Where(v => v.Nickname.ToLower().Contains(searchTerm));
						break;
					case "specialization":
						query = query.Where(v => v.Specialization.ToLower().Contains(searchTerm));
						break;
					case "yearsofexperience":
						if (int.TryParse(searchTerm, out int years))
						{
							query = query.Where(v => v.YearsOfExperience == years);
						}
						else
						{
							query = query.Where(v => false); // No results if parsing fails
						}
						break;
					default:
						query = query.Where(v =>
							v.Email.ToLower().Contains(searchTerm) ||
							v.Nickname.ToLower().Contains(searchTerm) ||
							v.Specialization.ToLower().Contains(searchTerm));
						break;
				}
			}

			// Apply sorting
			if (!string.IsNullOrEmpty(searchModel.SortBy))
			{
				var sortBy = searchModel.SortBy.ToLower();
				var sortOrder = searchModel.SortOrder?.ToLower() ?? "desc";

				query = sortBy switch
				{
					"id" => sortOrder == "asc" ? query.OrderBy(v => v.Id) : query.OrderByDescending(v => v.Id),
					"email" => sortOrder == "asc" ? query.OrderBy(v => v.Email) : query.OrderByDescending(v => v.Email),
					"nickname" => sortOrder == "asc" ? query.OrderBy(v => v.Nickname) : query.OrderByDescending(v => v.Nickname),
					"specialization" => sortOrder == "asc" ? query.OrderBy(v => v.Specialization) : query.OrderByDescending(v => v.Specialization),
					"yearsofexperience" => sortOrder == "asc" ? query.OrderBy(v => v.YearsOfExperience) : query.OrderByDescending(v => v.YearsOfExperience),
					_ => sortOrder == "asc" ? query.OrderBy(v => v.Id) : query.OrderByDescending(v => v.Id)
				};
			}

			// Apply pagination
			var totalCount = await query.CountAsync();

			// var items = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
			var items = await query
				.Skip((searchModel.PageIndex - 1) * searchModel.PageSize)
				.Take(searchModel.PageSize)
				.Select(v => new VerifyRequestModel
				{
					Id = v.Id,
					Email = v.Email,
					Nickname = v.Nickname,
					Specialization = v.Specialization,
					Bio = v.Bio,
					YearsOfExperience = v.YearsOfExperience,
					Languages = v.Languages,
					LinkedInAccount = v.LinkedInAccount,
					FacebookAccount = v.FacebookAccount,
					InstagramAccount = v.InstagramAccount
				})
				.ToListAsync();

			return new PaginatedResultV2<VerifyRequestModel>
			{
				List = items,
				Total = totalCount,
				Current = searchModel.PageIndex
			};
		}

		public async Task<VerifyRequestModel> GetVerificationRequestByIdAsync(int id)
		{
			var verification = await _unitOfWork.VerificationRepository.GetVerificationRequestByIdAsync(id);
			return verification == null ? null : MapToVerifyRequestModel(verification);
		}

		public async Task SubmitVerificationAsync(int userId, VerifyRequestModel model)
		{
			var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
			if (user == null)
			{
				throw new InvalidOperationException("User not found.");
			}

			if (!string.Equals(model.Email, user.Email, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("The provided email must match your account email.");
			}

			if (model.YearsOfExperience <= 0)
			{
				throw new InvalidOperationException("Years of experience must be a positive number.");
			}

			if (await _unitOfWork.VerificationRepository.ExistsAsync(userId))
			{
				throw new InvalidOperationException("User already has a verification request pending.");
			}

			if (user.Expert != null)
			{
				throw new InvalidOperationException("User is already an expert.");
			}

			if (model.Photo == null
				|| model.PhotoType == null
				|| model.Photo.Count == 0
				|| model.PhotoType.Count == 0
				|| model.Photo.Count != model.PhotoType.Count)
			{
				throw new InvalidOperationException("At least one photo and matching type are required for verification.");
			}

			var verifyInfo = new VerifyInformation
			{
				Id = userId,
				Email = model.Email,
				Nickname = model.Nickname,
				Specialization = model.Specialization,
				Bio = model.Bio,
				YearsOfExperience = model.YearsOfExperience,
				Languages = model.Languages,
				Status = VerificationStatus.Pending.ToString()
			};

			var justSavedVerifyInfo = await _unitOfWork.VerificationRepository.CreateVerificationRequestAsync(verifyInfo);


			// Lưu ảnh vào db Photo với URL và Type
			var photos = new List<Photo>();
			for (int i = 0; i < model.Photo.Count; i++)
			{
				var photoFile = model.Photo[i];
				var photoType = model.PhotoType[i];

				// Validate photo type
				if (photoType != PhotoTypeEnum.ID_FRONT.ToString()
					&& photoType != PhotoTypeEnum.ID_BACK.ToString()
					&& photoType != PhotoTypeEnum.CERTIFICATE.ToString()
					&& photoType != PhotoTypeEnum.FACE_FRONT.ToString()
					&& photoType != PhotoTypeEnum.FACE_RIGHT.ToString()
					&& photoType != PhotoTypeEnum.FACE_LEFT.ToString())
				{
					throw new InvalidOperationException($"Invalid photo type: {photoType}. Allowed types are Face, Certification, or Identity.");
				}

				// Upload ảnh lên Cloudinary và lấy URL
				var imageUrl = await _imageUploadService.UploadImageAsync(photoFile);

				// Tạo Photo entity với URL, Type và VerifyInformationId
				photos.Add(new Photo
				{
					PhotoUrl = imageUrl,
					Type = photoType,
					VerifyInformationId = justSavedVerifyInfo.Id
				});
			}

			// Lưu tất cả photos vào database
			await _unitOfWork.PhotoRepository.CreatePhotosAsync(photos);

			await _unitOfWork.SaveChangesWithTransactionAsync();
		}

		public async Task<bool> AcceptVerificationAsync(int id)
		{
			var verifyInfo = await _unitOfWork.VerificationRepository.GetVerificationRequestByIdAsync(id);
			if (verifyInfo == null)
			{
				throw new InvalidOperationException("Verification request not found.");
			}

			var expert = new Expert
			{
				Id = id,
				Nickname = verifyInfo.Nickname,
				Specialization = verifyInfo.Specialization,
				Bio = verifyInfo.Bio,
				YearsOfExperience = verifyInfo.YearsOfExperience,
				Languages = verifyInfo.Languages,
				Rating = 0,
				Introduction = null,
				FacebookAccount = verifyInfo.FacebookAccount,
				LinkedInAccount = verifyInfo.LinkedInAccount,
				InstagramAccount = verifyInfo.InstagramAccount
			};

			await _unitOfWork.ExpertRepository.CreateAsync(expert);

			// Update user role to expert
			var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
			if (user != null)
			{
				var expertRole = await _unitOfWork.RoleRepository.GetByIdAsync(3);
				if (expertRole != null)
				{
					user.RoleId = expertRole.Id;
					await _unitOfWork.UserRepository.UpdateAsync(user);
				}
			}

			//send in-app notification
			var notiModel = new CreateNotificationModel
			{
				Title = "Verification Approved",
				Content = "Your identity verification has been approved. You can now access all verified-user features.",
				Receiver = verifyInfo.Id
			};
			await _notificationService.CreateAsync(notiModel);

			//send email
			var emailModel = new EmailServiceRequestModel
			{
				ToEmail = verifyInfo.Email,
				Subject = "[PerHue] Your verification is approved",
				Body =
@"Hi " + verifyInfo.Nickname + @",

Good news! Your identity verification has been approved.

You can now access all features that require a verified account.

If you have any questions, reply to this email or contact our Support Center.

Best regards,
PerHue Team"
			};
			await _emailService.SendEmailAsync(emailModel.ToEmail, emailModel.Subject, emailModel.Body);

			await _unitOfWork.VerificationRepository.DeleteVerificationRequestAsync(id, true);
			await _unitOfWork.SaveChangesWithTransactionAsync();
			return true;
		}

		public async Task<bool> DenyVerificationAsync(int id, string reason)
		{
			var verifyInfo = await _unitOfWork.VerificationRepository.GetVerificationRequestByIdAsync(id);
			if (verifyInfo == null)
			{
				throw new InvalidOperationException("Verification request not found.");
			}

			var notification = new Notification
			{
				Receiver = id,
				Title = "Verification Denied",
				Content = $"Your verification request has been denied. Reason: {reason}",
				ReceivedTime = _dateTimeService.GetCurrentTime(),
				IsRead = false
			};

			await _unitOfWork.NotificationRepository.CreateAsync(notification);

			//send email
			var emailModel = new EmailServiceRequestModel
			{
				ToEmail = verifyInfo.Email,
				Subject = "[PerHue] Your verification is denied",
				Body =
			@"Hi " + verifyInfo.Nickname + @",

Thank you for submitting your verification request.

After reviewing your information, we’re unable to approve your verification at this time.

Please update and resubmit your verification.

You can resubmit your verification directly in the PerHue app anytime.

If you need help, reply to this email or contact our Support Center.

Best regards,
PerHue Team"
			};
			await _emailService.SendEmailAsync(emailModel.ToEmail, emailModel.Subject, emailModel.Body);

			await _unitOfWork.VerificationRepository.DeleteVerificationRequestAsync(id, false);
			await _unitOfWork.SaveChangesWithTransactionAsync();
			return true;
		}

		public async Task<bool> HasPendingVerificationAsync(int userId)
		{
			return await _unitOfWork.VerificationRepository.ExistsAsync(userId);
		}

		private VerifyRequestModel MapToVerifyRequestModel(VerifyInformation verifyInformation)
		{
			return new VerifyRequestModel
			{
				Id = verifyInformation.Id,
				Email = verifyInformation.Email,
				Nickname = verifyInformation.Nickname,
				Specialization = verifyInformation.Specialization,
				Bio = verifyInformation.Bio,
				YearsOfExperience = verifyInformation.YearsOfExperience,
				Languages = verifyInformation.Languages,
				FacebookAccount = verifyInformation.FacebookAccount,
				LinkedInAccount = verifyInformation.LinkedInAccount,
				InstagramAccount = verifyInformation.InstagramAccount,
			};
		}

		public async Task Version2SubmitVerificationAsync(int userId, VerifyInformationModel model)
		{
			var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
			if (user == null)
			{
				throw new InvalidOperationException("User not found.");
			}

			if (!string.Equals(model.Email, user.Email, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("The provided email must match your account email.");
			}

			if (model.YearsOfExperience <= 0)
			{
				throw new InvalidOperationException("Years of experience must be a positive number.");
			}

			if (await _unitOfWork.VerificationRepository.ExistsAsync(userId))
			{
				throw new InvalidOperationException("User already has a verification request pending.");
			}

			if (user.Expert != null)
			{
				throw new InvalidOperationException("User is already an expert.");
			}

			if (model.PhotoAndType == null)
			{
				throw new InvalidOperationException("At least one photo and matching type are required for verification.");
			}

			var verifyInfo = new VerifyInformation
			{
				Email = model.Email,
				Nickname = model.Nickname,
				Specialization = model.Specialization,
				Bio = model.Bio,
				YearsOfExperience = model.YearsOfExperience,
				Languages = model.Languages,
				Status = VerificationStatus.Pending.ToString()
			};

			var justSavedVerifyInfo = await _unitOfWork.VerificationRepository.CreateVerificationRequestAsync(verifyInfo);


			// Lưu ảnh vào db Photo với URL và Type
			var photos = new List<Photo>();
			foreach (var item in model.PhotoAndType)
			{
				if (item.PhotoType != PhotoTypeEnum.ID_FRONT.ToString()
				&& item.PhotoType != PhotoTypeEnum.ID_BACK.ToString()
				&& item.PhotoType != PhotoTypeEnum.CERTIFICATE.ToString()
				&& item.PhotoType != PhotoTypeEnum.FACE_FRONT.ToString()
				&& item.PhotoType != PhotoTypeEnum.FACE_RIGHT.ToString()
				&& item.PhotoType != PhotoTypeEnum.FACE_LEFT.ToString())
				{
					throw new InvalidOperationException($"Invalid photo type: {item.PhotoType}. Allowed types are Face, Certification, or Identity.");
				}

				var imageUrl = await _imageUploadService.UploadImageAsync(item.Photo);

				photos.Add(new Photo
				{
					PhotoUrl = imageUrl,
					Type = item.PhotoType,
					VerifyInformationId = justSavedVerifyInfo.Id
				});
			}

			// Lưu tất cả photos vào database
			await _unitOfWork.PhotoRepository.CreatePhotosAsync(photos);

			await _unitOfWork.SaveChangesWithTransactionAsync();
		}
	}
}