using PerHue.Application.Basic;
using PerHue.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Application.IServices
{
	public interface IPostService : IGenericService<PostModel>
	{
		Task<PaginatedResult<PostModel>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm);
		Task<PostModel> CreateAsync(PostModel model);

	}
}
