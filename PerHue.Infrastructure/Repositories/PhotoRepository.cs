using PerHue.Domain.Entities;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;
using PerHue.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Repositories
{
	internal class PhotoRepository : GenericRepository<Photo>, IPhotoRepository
	{
		public PhotoRepository(PerHueDbContext context) : base(context)
		{
		}

		public async Task CreatePhotosAsync(List<Photo> photos)
		{
			await _context.Photos.AddRangeAsync(photos);
			await _context.SaveChangesAsync();
		}
	}
}
