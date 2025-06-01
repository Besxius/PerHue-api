using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;
using System.Security.Cryptography.X509Certificates;

namespace PerHue.Infrastructure.Repositories
{
	internal class UserRepository : GenericRepository<UserAccount>, IUserRepository
	{
		public UserRepository(PerHueDbContext context) : base(context)
		{
		}

		public async Task<UserAccount> GetByEmailAsync(string email)
		{
			return await _context.UserAccounts.Include(p => p.Role).FirstOrDefaultAsync(u => u.Email == email);
		}

		public async Task<bool> DeleteByEmailAsync(string email)
		{
			var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);
			if (user == null)
				return false;
			_context.UserAccounts.Remove(user);
			await _context.SaveChangesAsync();
			return true;
		}

	}
}
