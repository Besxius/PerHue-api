using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class AiPictureRepository : GenericRepository<AiPicture>, IAiPictureRepository
	{
		public AiPictureRepository(PerHueDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<AiPicture>> GetPicturesForTestRequestAsync(int testRequestId)
		{
			return await _context.AiPictures
				.Where(p => p.TestRequestId == testRequestId)
				.ToListAsync();
		}
	}
}