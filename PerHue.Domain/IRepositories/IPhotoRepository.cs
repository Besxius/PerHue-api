using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IPhotoRepository : IGenericRepository<Photo>
	{
		Task CreatePhotosAsync(List<Photo> photos);
	}
}