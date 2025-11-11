using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IAiPictureRepository : IGenericRepository<AiPicture>
	{
		Task<IEnumerable<AiPicture>> GetPicturesForTestRequestAsync(int testRequestId);
	}
}