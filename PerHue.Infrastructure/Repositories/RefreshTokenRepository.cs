using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.Repositories
{
	internal class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
	{
		public RefreshTokenRepository(PerHueDbContext context) : base(context)
		{
		}

		public async Task<RefreshToken> GetByTokenAsync(string token)
		{
			return await _context.RefreshTokens
				.Include(rt => rt.UserAccount) // Eager load the user
				.ThenInclude(u => u.Role)
				.FirstOrDefaultAsync(rt => rt.Token == token);
		}
	}
}