using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VerificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VerifyRequestModel>> GetAllVerificationRequestsAsync()
        {
            var verifications = await _unitOfWork.VerificationRepository.GetAllVerificationRequestsAsync();
            return verifications.Select(MapToVerifyRequestModel);
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

            var verifyInfo = new VerifyInformation
            {
                Id = userId,
                Email = model.Email,
                Nickname = model.Nickname,
                Specialization = model.Specialization,
                Bio = model.Bio,
                YearsOfExperience = model.YearsOfExperience,
                Languages = model.Languages,
                Certification = model.Certification
            };

            await _unitOfWork.VerificationRepository.CreateVerificationRequestAsync(verifyInfo);
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
                Certification = verifyInfo.Certification,
                Rating = 0,
                Introduction = null,
                FacebookAccount = null,
                LinkedInAccount = null,
                InstagramAccount = null
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

            await _unitOfWork.VerificationRepository.DeleteVerificationRequestAsync(id);
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
                Title = "Expert Verification Request Denied",
                Content = $"Your verification request has been denied. Reason: {reason}",
                ReceivedTime = DateTime.Now,
                IsRead = false
            };

            await _unitOfWork.NotificationRepository.CreateAsync(notification);
            await _unitOfWork.VerificationRepository.DeleteVerificationRequestAsync(id);
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
                Certification = verifyInformation.Certification
            };
        }
    }
}