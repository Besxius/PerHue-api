using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class ColorTypeRespository : GenericRepository<ColorType>, IColorTypeRepository
	{
		public ColorTypeRespository(PerHueDbContext context) : base(context)
		{
		}

		public async Task<ColorType> GetByNameAsync(string name)
		{
			return await _context.ColorTypes.FirstOrDefaultAsync(ct => ct.Name.ToLower().Equals(name.ToLower()));
		}
	}
}
