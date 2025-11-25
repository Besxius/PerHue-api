using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IColorRepository : IGenericRepository<Color>
	{
		Task<IEnumerable<Color>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm);
		Task<IEnumerable<Color>> GetRelativeColors(List<string> selectedColors);
		Task<IEnumerable<Color>> GetByColorTypeIdAsync(int colorTypeId);
		Task<IEnumerable<Color>> GetByColorTypeIdAsync(int colorTypeId, int pageIndex, int pageSize, string? searchTerm);
		Task<int> CountByColorTypeIdAsync(int colorTypeId, string? searchTerm);
		Task<IEnumerable<Color>> GetAllBySpectrumAsync();
		Task<(IEnumerable<Color> Items, int TotalCount)> GetAllBySpectrumPagedAsync(int pageIndex, int pageSize, string? searchTerm);
	}
}
