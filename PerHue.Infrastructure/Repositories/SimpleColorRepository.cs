using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class SimpleColorRepository : GenericRepository<SimpleColor>, ISimpleColorRepository
	{
		public SimpleColorRepository(PerHueDbContext context) : base(context)
		{
		}
	}
}
