using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface ICapsulePaletteRepository : IGenericRepository<CapsulePalette>
	{
		Task<IEnumerable<CapsulePalette>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm);
		Task<CapsulePalette> GetByIdAsync(int id);
		Task<IEnumerable<CapsulePalette>> GetRelativeCapsulePalettes(List<string> selectedColors);
		Task<IEnumerable<CapsulePalette>> GetRelativeCapsulePalettes(List<string> selectedColors, string colorType);
	}
}
