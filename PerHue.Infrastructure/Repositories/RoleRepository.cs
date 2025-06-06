using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class RoleRepository : GenericRepository<Role>, IRoleRepository
	{
		public RoleRepository(PerHueDbContext context) : base(context)
		{
		}
	}
}
