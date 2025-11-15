using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class AiTestResultRepository : GenericRepository<AiTestResult>, IAiTestResultRepository
	{
		public AiTestResultRepository(PerHueDbContext context) : base(context)
		{
		}
	}
}
