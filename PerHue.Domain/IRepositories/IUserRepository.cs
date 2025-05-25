using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface IUserRepository : IGenericRepository<UserAccount>
	{
		Task<bool> DeleteByEmailAsync(string email);
		Task<UserAccount> GetByEmailAsync(string email);
	}
}
