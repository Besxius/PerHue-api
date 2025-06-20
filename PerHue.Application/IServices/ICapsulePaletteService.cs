using PerHue.Application.Basic;
using PerHue.Application.Models;

namespace PerHue.Application.IServices
{
	public interface ICapsulePaletteService : IGenericService<CapsulePaletteModel>
	{
		Task<PaginatedResult<CapsulePaletteModel>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm);
		Task<IEnumerable<CapsulePaletteModel>> GetRelativeCapsulePalettes(List<string> selectedColors);
	}
}
