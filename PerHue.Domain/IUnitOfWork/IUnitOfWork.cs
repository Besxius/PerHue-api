using PerHue.Domain.IRepositories;

namespace PerHue.Domain.UnitOfWork
{
	public interface IUnitOfWork
	{
		IPaymentRepository PaymentRepository { get; }
		IServicePackageRepository ServicePackageRepository { get; }
		IUserRepository UserRepository { get; }
		IUserSubscriptionRepository UserSubscriptionRepository { get; }
		IPaymentLogRepository PaymentLogRepository { get; }
		IRoleRepository RoleRepository { get; }
		IColorRepository ColorRepository { get; }
		ICapsulePaletteRepository CapsulePaletteRepository { get; }
		IColorTypeRepository ColorTypeRepository { get; }
		ITestResultRepository TestResultRepository { get; }
		IVerificationRepository VerificationRepository { get; }
		IExpertRepository ExpertRepository { get; }
		INotificationRepository NotificationRepository { get; }


		ITestRequestRepository TestRequestRepository { get; }
		ITestResponseRepository TestResponseRepository { get; }
		IExpertTestRequestRepository ExpertTestRequestRepository { get; }

		IAiPictureRepository AiPictureRepository { get; }
		IAiTestResultRepository AiTestResultRepository { get; }

		IRefreshTokenRepository RefreshTokenRepository { get; }

	IPhotoRepository PhotoRepository { get; }

	IReportRepository ReportRepository { get; }

	int SaveChangesWithTransaction();
	Task<int> SaveChangesWithTransactionAsync();
	}
}
