using PerHue.Application.IServices;

namespace PerHue.Application.IServicesProvider
{
	public interface IServicesProvider
	{
		IUserService UserService { get; }
		IPaymentService PaymentService { get; }
		IServicePackageService ServicePackageService { get; }
		IUserSubscriptionService UserSubscriptionService { get; }
		IPaymentLogService PaymentLogService { get; }
		IRoleService RoleService { get; }
	}
}
