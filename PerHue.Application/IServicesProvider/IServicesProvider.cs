using PerHue.Application.IServices;

namespace PerHue.Application.IServicesProvider
{
	public interface IServicesProvider
	{
		IUserService UserService { get; }
	}
}
