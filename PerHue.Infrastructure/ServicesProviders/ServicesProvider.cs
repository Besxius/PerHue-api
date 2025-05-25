using PerHue.Application.IServices;
using PerHue.Application.IServicesProvider;
using PerHue.Domain.UnitOfWork;

namespace PerHue.Infrastructure.ServicesProviders
{
	internal class ServicesProvider : IServicesProvider
	{
		private readonly IUnitOfWork _unitOfWork;

		public IUserService UserService { get; private set; }

		public ServicesProvider(IUnitOfWork unitOfWork, IUserService userService)
		{
			_unitOfWork = unitOfWork;
			UserService = userService;
		}

	}
}
