using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class ServicePackageRepository : GenericRepository<ServicePackage>, IServicePackageRepository
	{
		public ServicePackageRepository(PerHueDbContext context) : base(context)
		{
		}
	}
}
