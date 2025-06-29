using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;

namespace PerHue.Infrastructure.Repositories
{
	internal class SimpleColorRepository : GenericRepository<SimpleColor>, ISimpleColorRepository
	{
	}
}
