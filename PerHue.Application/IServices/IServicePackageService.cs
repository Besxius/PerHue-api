using PerHue.Application.Basic;
using PerHue.Application.Models;

namespace PerHue.Application.IServices
{
	public interface IServicePackageService : IGenericService<ServicePackageModel>
	{
		Task CreateAsync(ServicePackageModel servicePackageModel);
		Task<bool> UpdateAsync(int id, ServicePackageModel servicePackageModel);
	}
}
