using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IColorTypeRepository : IGenericRepository<ColorType>
	{
		Task<ColorType> GetByNameAsync(string name);
	}
}
