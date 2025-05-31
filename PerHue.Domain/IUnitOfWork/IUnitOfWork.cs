using PerHue.Domain.IRepositories;

namespace PerHue.Domain.UnitOfWork
{
	public interface IUnitOfWork
	{
		IPaymentRepository PaymentRepository { get; }
		IServicePackageRepository ServicePackageRepository { get; }
		IUserRepository UserRepository { get; }
		IUserSubscriptionRepository UserSubscriptionRepository { get; }

		int SaveChangesWithTransaction();
		Task<int> SaveChangesWithTransactionAsync();
	}
}
