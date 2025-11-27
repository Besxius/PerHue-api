using PerHue.Application.Basic;
using PerHue.Application.Models;
using PerHue.Application.Models.Color;

namespace PerHue.Application.IServices
{
	public interface IColorService : IGenericService<ColorModel>
	{
		Task<PaginatedResult<ColorModel>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm);
		Task<IEnumerable<ColorModel>> GetRelativeColors(List<string> selectedColors);
		Task<IEnumerable<ColorModel>> GetAllBySpectrumAsync();
		Task<PaginatedResult<ColorModel>> GetAllBySpectrumPagedAsync(int pageIndex, int pageSize, string? searchTerm);
		Task<IEnumerable<ColorModel>> GetColorsByColorTypeNormalAsync(int colorTypeId);
		Task<PaginatedResult<ColorModel>> GetColorsByColorTypePagingAsync(int colorTypeId, int pageIndex, int pageSize, string? searchTerm);
	}
}
