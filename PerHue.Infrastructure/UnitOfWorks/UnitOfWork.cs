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
		public IColorRepository ColorRepository { get; private set; }
		public ICapsulePaletteRepository CapsulePaletteRepository { get; private set; }
		public IColorTypeRepository ColorTypeRepository { get; private set; }
		public ITestResultRepository TestResultRepository { get; private set; }
		public IVerificationRepository VerificationRepository { get; private set; }
		public IExpertRepository ExpertRepository { get; private set; }
		public INotificationRepository NotificationRepository { get; private set; }

		public ITestRequestRepository TestRequestRepository { get; private set; }
		public ITestResponseRepository TestResponseRepository { get; private set; }
		public IExpertTestRequestRepository ExpertTestRequestRepository { get; private set; }

		public IAiPictureRepository AiPictureRepository { get; private set; }
		public IAiTestResultRepository AiTestResultRepository { get; private set; }
		public IRefreshTokenRepository RefreshTokenRepository { get; private set; }

		public IPhotoRepository PhotoRepository { get; private set; }

		public IReportRepository ReportRepository { get; private set; }

		public UnitOfWork(
				PerHueDbContext context,
				IUserRepository userRepository,
				IServicePackageRepository servicePackageRepository,
				IPaymentRepository paymentRepository,
				IUserSubscriptionRepository userSubscriptionRepository,
				IPaymentLogRepository paymentLogRepository,
				IRoleRepository roleRepository,
				IColorRepository colorRepository,
				ICapsulePaletteRepository capsulePaletteRepository,
				IColorTypeRepository colorTypeRepository,
				ITestResultRepository testResultRepository,
				IVerificationRepository verificationRepository,
				IExpertRepository expertRepository,
				INotificationRepository notificationRepository,

				ITestRequestRepository testRequestRepository,
				ITestResponseRepository testResponseRepository,
				IExpertTestRequestRepository expertTestRequestRepository,

				IAiPictureRepository aiPictureRepository,
				IAiTestResultRepository aiTestResultRepository,
				IRefreshTokenRepository refreshTokenRepository,
				IPhotoRepository photoRepository,
				IReportRepository reportRepository)
		{
			_context = context;
			UserRepository = userRepository;
			ServicePackageRepository = servicePackageRepository;
			PaymentRepository = paymentRepository;
			UserSubscriptionRepository = userSubscriptionRepository;
			PaymentLogRepository = paymentLogRepository;
			RoleRepository = roleRepository;
			ColorRepository = colorRepository;
			CapsulePaletteRepository = capsulePaletteRepository;
			ColorTypeRepository = colorTypeRepository;
			TestResultRepository = testResultRepository;
			VerificationRepository = verificationRepository;
			ExpertRepository = expertRepository;
			NotificationRepository = notificationRepository;

			TestRequestRepository = testRequestRepository;
			TestResponseRepository = testResponseRepository;
			ExpertTestRequestRepository = expertTestRequestRepository;

			AiPictureRepository = aiPictureRepository;
			AiTestResultRepository = aiTestResultRepository;
			RefreshTokenRepository = refreshTokenRepository;
			PhotoRepository = photoRepository;
			ReportRepository = reportRepository;
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
