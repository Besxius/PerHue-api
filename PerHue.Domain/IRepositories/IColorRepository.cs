using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IColorRepository : IGenericRepository<Color>
	{
		// Define any additional methods specific to Color repository if needed
		// For example, methods to find colors by name, hex value, etc.
	}
}
