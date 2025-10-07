using PerHue.Domain.Basic;
using PerHue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Domain.IRepositories
{
	public interface IPostRepository :IGenericRepository<Post>
	{
		Task<IEnumerable<Post>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm);
	}
}
