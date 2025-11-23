using PerHue.Application.Basic;
using PerHue.Application.Models;
using PerHue.Application.Models.CapsulePalette;

namespace PerHue.Application.IServices
{
	public interface IAdminCapsulePaletteService : IGenericService<AdminCapsulePaletteModel>
	{
		Task<PaginatedResultV2<AdminCapsulePaletteModel>> GetAllAsync(AdminCapsulePaletteSearchModel searchModel);
		Task<AdminCapsulePaletteModel?> GetByIdAsync(int id);
		Task<AdminCapsulePaletteModel> CreateAsync(AdminCapsulePaletteCreateModel model);
		Task<AdminCapsulePaletteModel> UpdateAsync(AdminCapsulePaletteUpdateModel model);
		Task<bool> DeleteAsync(int id);
	}
}