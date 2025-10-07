using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Repositories
{
	internal class PostRepository : GenericRepository<Post>, IPostRepository
	{
		public PostRepository(PerHueDbContext context) : base(context)
		{
		}
		public async Task<IEnumerable<Post>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm)
		{
			var query = _context.Posts
						.Include(p => p.User)
						.Include(p => p.Topic)
						.AsQueryable();
	
			if (!string.IsNullOrEmpty(searchTerm))
			{
				query = query.Where(p => p.Content.Contains(searchTerm));
			}

			query = query.OrderByDescending(p => p.Time)
			 .Skip((pageIndex - 1) * pageSize)
			 .Take(pageSize);

			return await query.ToListAsync();
		}

		public async Task<Post> GetByIdAsync(int id)
		{
			/*return await _context.Posts.Include(p => p.Topic).FirstOrDefaultAsync(p => p.Id == id);*/
			return await _context.Posts
					.Include(p => p.User)
					.Include(p => p.Topic)
					.FirstOrDefaultAsync(p => p.Id == id);
		}

		public IQueryable<Post> GetAll()
		{
			return _context.Posts.Include(p => p.Topic);
		}

	}
}
