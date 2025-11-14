using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models.Expert;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Services
{
    public class ExpertService : IExpertService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExpertService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ExpertModel> GetByIdAsync(int id)
        {
            var expert = await _unitOfWork.ExpertRepository.GetByIdAsync(id);
            return _mapper.Map<ExpertModel>(expert);
        }

        public async Task<IEnumerable<ExpertModel>> GetAllAsync()
        {
            var experts = await _unitOfWork.ExpertRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ExpertModel>>(experts);
        }

        public async Task<bool> UpdateAsync(int id, UpdateExpertModel model)
        {
            var expert = await _unitOfWork.ExpertRepository.GetByIdAsync(id);
            if (expert == null)
                return false;

            // Update expert properties
            expert.Nickname = model.Nickname;
            expert.Specialization = model.Specialization;
            expert.Bio = model.Bio;
            expert.YearsOfExperience = model.YearsOfExperience;
            expert.Languages = model.Languages;
            expert.Certification = model.Certification;
            expert.Introduction = model.Introduction;
            expert.FacebookAccount = model.FacebookAccount;
            expert.LinkedInAccount = model.LinkedInAccount;
            expert.InstagramAccount = model.InstagramAccount;

            await _unitOfWork.ExpertRepository.UpdateAsync(expert);
            await _unitOfWork.SaveChangesWithTransactionAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.ExpertRepository.DeleteAsync(id);
            if (result)
            {
                await _unitOfWork.SaveChangesWithTransactionAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _unitOfWork.ExpertRepository.ExistsAsync(id);
        }
    }
}