using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Domain.UnitOfWork;

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

		public ServicesProvider(
			IUnitOfWork unitOfWork,
			IUserService userService,
			IPaymentService paymentService,
			IServicePackageService servicePackageService,
			IUserSubscriptionService userSubscriptionService,
			IPaymentLogService paymentLogService,
			IRoleService roleService)
		{
			_unitOfWork = unitOfWork;
			UserService = userService;
			PaymentService = paymentService;
			ServicePackageService = servicePackageService;
			UserSubscriptionService = userSubscriptionService;
			PaymentLogService = paymentLogService;
			RoleService = roleService;
		}

	}
}
