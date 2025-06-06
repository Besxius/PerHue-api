using PerHue.Domain.IRepositories;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Persistence;

namespace PerHue.Infrastructure.UnitOfWorks
{
	internal class UnitOfWork : IUnitOfWork
	{
		private readonly PerHueDbContext _context;

		public IUserRepository UserRepository { get; private set; }
		public IServicePackageRepository ServicePackageRepository { get; private set; }
		public IPaymentRepository PaymentRepository { get; private set; }
		public IUserSubscriptionRepository UserSubscriptionRepository { get; private set; }
		public IPaymentLogRepository PaymentLogRepository { get; private set; }
		public IRoleRepository RoleRepository { get; private set; }

		public UnitOfWork(
			PerHueDbContext context,
			IUserRepository userRepository,
			IServicePackageRepository servicePackageRepository,
			IPaymentRepository paymentRepository,
			IUserSubscriptionRepository userSubscriptionRepository,
			IPaymentLogRepository paymentLogRepository,
			IRoleRepository roleRepository)
		{
			_context = context;
			UserRepository = userRepository;
			ServicePackageRepository = servicePackageRepository;
			PaymentRepository = paymentRepository;
			UserSubscriptionRepository = userSubscriptionRepository;
			PaymentLogRepository = paymentLogRepository;
			RoleRepository = roleRepository;
		}

		public int SaveChangesWithTransaction()
		{
			int result = -1;

			//System.Data.IsolationLevel.Snapshot
			using (var dbContextTransaction = _context.Database.BeginTransaction())
			{
				try
				{
					result = _context.SaveChanges();
					dbContextTransaction.Commit();
				}
				catch (Exception)
				{
					//Log Exception Handling message                      
					result = -1;
					dbContextTransaction.Rollback();
				}
			}

			return result;
		}

		public async Task<int> SaveChangesWithTransactionAsync()
		{
			int result = -1;

			//System.Data.IsolationLevel.Snapshot
			using (var dbContextTransaction = _context.Database.BeginTransaction())
			{
				try
				{
					result = await _context.SaveChangesAsync();
					dbContextTransaction.Commit();
				}
				catch (Exception)
				{
					//Log Exception Handling message                      
					result = -1;
					dbContextTransaction.Rollback();
				}
			}

			return result;
		}
	}
}
