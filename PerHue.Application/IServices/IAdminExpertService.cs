using PerHue.Application.Basic;
using PerHue.Application.Models;
using PerHue.Application.Models.Expert;

namespace PerHue.Application.IServices
{
	public interface IAdminExpertService : IGenericService<ExpertModel>
	{
		Task<PaginatedResultV2<ExpertModel>> GetAllAsync(ExpertSearchModel searchModel);
		Task<PaginatedResultV2<ExpertSalaryModel>> GetAllSalaryReportsAsync(ExpertSalarySearchModel searchModel);
	}
}
