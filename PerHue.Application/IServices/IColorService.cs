using PerHue.Application.Basic;
using PerHue.Application.Models;

namespace PerHue.Application.IServices
{
	public interface IColorService : IGenericService<ColorModel>
	{
		Task<PaginatedResult<ColorModel>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm);
		Task<IEnumerable<ColorModel>> GetRelativeColors(List<string> selectedColors);
	}
}
