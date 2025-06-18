using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IColorRepository : IGenericRepository<Color>
	{
		Task<IEnumerable<Color>> GetAllAsync(int pageIndex, int pageSize, string? searchTerm);
	}
}
