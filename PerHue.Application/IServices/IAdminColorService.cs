using PerHue.Application.Basic;
using PerHue.Application.Models;
using PerHue.Application.Models.Color;

namespace PerHue.Application.IServices
{
	public interface IAdminColorService : IGenericService<AdminColorModel>
	{
		Task<PaginatedResultV2<AdminColorModel>> GetAllAsync(AdminColorSearchModel searchModel);
		Task<AdminColorModel?> GetByIdAsync(int id);
		Task<AdminColorModel> CreateAsync(AdminColorCreateModel model);
		Task<AdminColorModel> UpdateAsync(AdminColorUpdateModel model);
		Task<bool> DeleteAsync(int id);
	}
}