using PerHue.Application.Basic;
using PerHue.Application.Models;

namespace PerHue.Application.IServices
{
	public interface IColorTypeService : IGenericService<ColorTypeModel>
	{
		Task<ColorTypeModel> GetByNameAsync(string name);
	}
}
