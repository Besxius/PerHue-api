using PerHue.Application.Models;

namespace PerHue.Application.IServices
{
	public interface IServicePackageService
	{
		Task<IEnumerable<ServicePackageModel>> GetAllServicePackagesAsync();
		Task<ServicePackageModel> GetServicePackageByIdAsync(int id);
		Task CreateServicePackageAsync(ServicePackageModel servicePackageModel);
		Task<bool> UpdateServicePackageAsync(int id, ServicePackageModel servicePackageModel);
		Task DeleteServicePackageAsync(int id);
	}
}
