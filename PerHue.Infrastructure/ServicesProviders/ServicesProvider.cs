using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Domain.UnitOfWork;
using PerHue.Infrastructure.Services;

namespace PerHue.Infrastructure.ServicesProviders
{
	internal class ServicesProvider : IServicesProvider
	{
		private readonly IUnitOfWork _unitOfWork;

		public IUserService UserService { get; private set; }
		public IPaymentService PaymentService { get; private set; }
		public IServicePackageService ServicePackageService { get; private set; }
		public IUserSubscriptionService UserSubscriptionService { get; private set; }
		public IPaymentLogService PaymentLogService { get; private set; }
		public IRoleService RoleService { get; private set; }
		public IColorService ColorService { get; private set; }
		public ICapsulePaletteService CapsulePaletteService { get; private set; }
		public IColorTypeService ColorTypeService { get; private set; }
		public ITestResultService TestResultService { get; private set; }
		public IOtpService OtpService { get; private set; }

		public IExpertTestService ExpertTestService { get; private set; }
		public IExpertService ExpertService { get; private set; }
		public INotificationService NotificationService { get; private set; }

		public IAIImageAnalysisService AIImageAnalysisService { get; private set; }
		public IAiTestService AiTestService { get; private set; }

		public IAdminColorService AdminColorService { get; private set; }
		public IAdminCapsulePaletteService AdminCapsulePaletteService { get; private set; }
		public IAdminDashboardService AdminDashboardService { get; private set; }
		public IAdminExpertService AdminExpertService { get; private set; }

		public IReportService ReportService { get; private set; }
		public ServicesProvider(
				IUnitOfWork unitOfWork,
				IUserService userService,
				IPaymentService paymentService,
				IServicePackageService servicePackageService,
				IUserSubscriptionService userSubscriptionService,
				IPaymentLogService paymentLogService,
				IRoleService roleService,
				IColorService colorService,
				ICapsulePaletteService capsulePaletteService,
				IColorTypeService colorTypeService,
				ITestResultService testResultService,
				IOtpService otpService,

			IExpertTestService expertTestService,
			IExpertService expertService,
			INotificationService notificationService,
			IAIImageAnalysisService aIImageAnalysisService,
			IAiTestService aiTestService,
			IAdminColorService adminColorService,
			IAdminCapsulePaletteService adminCapsulePaletteService,
			IAdminDashboardService adminDashboardService,
			IAdminExpertService adminExpertService,
			IReportService reportService
			)
		{
			_unitOfWork = unitOfWork;
			UserService = userService;
			PaymentService = paymentService;
			ServicePackageService = servicePackageService;
			UserSubscriptionService = userSubscriptionService;
			PaymentLogService = paymentLogService;
			RoleService = roleService;
			ColorService = colorService;
			CapsulePaletteService = capsulePaletteService;
			ColorTypeService = colorTypeService;
			TestResultService = testResultService;
			OtpService = otpService;

			ExpertTestService = expertTestService;
			ExpertService = expertService;
			NotificationService = notificationService;
			AIImageAnalysisService = aIImageAnalysisService;
			AiTestService = aiTestService;
			AdminColorService = adminColorService;
			AdminCapsulePaletteService = adminCapsulePaletteService;
			AdminDashboardService = adminDashboardService;
			AdminExpertService = adminExpertService;
			ReportService = reportService;
		}

	}
}