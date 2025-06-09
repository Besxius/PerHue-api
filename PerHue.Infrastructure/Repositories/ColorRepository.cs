using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class ColorRepository : GenericRepository<Color>, IColorRepository
	{
		public ColorRepository(PerHueDbContext context) : base(context)
		{
		}
	}
}
