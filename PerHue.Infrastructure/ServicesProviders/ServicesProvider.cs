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

		public IAIImageAnalysisService AIImageAnalysisService { get; private set; }
		public IAiTestService AiTestService { get; private set; }
		
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
			IAIImageAnalysisService aIImageAnalysisService,
			IAiTestService aiTestService


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
			AIImageAnalysisService = aIImageAnalysisService;
			AiTestService = aiTestService;
		}

	}
}
