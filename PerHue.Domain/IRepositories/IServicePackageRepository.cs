using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IServicePackageRepository : IGenericRepository<ServicePackage>
	{
		Task<ServicePackage> GetByAmountAsync(int amount);
	}
}
