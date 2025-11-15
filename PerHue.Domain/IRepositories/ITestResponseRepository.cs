using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface ITestResponseRepository : IGenericRepository<TestResponse>
	{
		Task<IEnumerable<TestResponse>> GetResponsesForRequestAsync(int testRequestId);
	}
}