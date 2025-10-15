using AutoMapper;
using PerHue.Application.IServices;
using PerHue.Application.Models;
using PerHue.Domain.Entities;
using PerHue.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Services
{
	internal class PostService : IPostService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public PostService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _unitOfWork.PostRepository.GetByIdAsync(id);
			return await _unitOfWork.PostRepository.RemoveAsync(entity);
		}

		public async Task<PaginatedResult<PostModel>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm)
		{
			var entities = await _unitOfWork.PostRepository.GetAllAsync(pageIndex, pageSize, searchTerm);
			var totalCount = entities.Count();

			if (string.IsNullOrEmpty(searchTerm))
			{
				totalCount = (await _unitOfWork.PostRepository.GetAllAsync()).Count();
			}
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
			var paginatedResult = new PaginatedResult<PostModel>
			{
				Items = _mapper.Map<IEnumerable<PostModel>>(entities),
				PageSize = pageSize,
				PageIndex = pageIndex,
				TotalCount = totalCount,
				TotalPages = totalPages
			};
			return paginatedResult;
		}

		public async Task<IEnumerable<PostModel>> GetAllAsync()
		{
			var posts = await _unitOfWork.PostRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<PostModel>>(posts);
		}

		public async Task<PostModel> GetByIdAsync(int id)
		{
			var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
			return post == null ? null : _mapper.Map<PostModel>(post);
		}
		public async Task<PostModel> CreateAsync(PostModel model)
		{
			var entity = _mapper.Map<Post>(model);
			entity.Time = DateTime.UtcNow; // auto set time

			await _unitOfWork.PostRepository.CreateAsync(entity);
			await _unitOfWork.SaveChangesWithTransactionAsync();

			return _mapper.Map<PostModel>(entity);
		}
	}
}
