using PerHue.Domain.IRepositories;

namespace PerHue.Domain.UnitOfWork
{
	public interface IUnitOfWork
	{
		IUserRepository UserRepository { get; }

		int SaveChangesWithTransaction();
		Task<int> SaveChangesWithTransactionAsync();
	}
}
