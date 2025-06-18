using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface ICapsulePaletteRepository : IGenericRepository<CapsulePalette>
	{
		Task<IEnumerable<CapsulePalette>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm);
	}
}
