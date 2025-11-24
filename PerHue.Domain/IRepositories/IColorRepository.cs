using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IColorRepository : IGenericRepository<Color>
	{
		Task<IEnumerable<Color>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm);
		Task<IEnumerable<Color>> GetRelativeColors(List<string> selectedColors);

		Task<List<Color>> GetAllColorsAsync();
		Task<Color?> GetColorByHexCodeAsync(string hexCode);
		Task<List<Color>> GetColorsByColorTypeIdAsync(int colorTypeId);
		Task<Color?> FindClosestColorByHexAsync(string hexCode);
	}
}
