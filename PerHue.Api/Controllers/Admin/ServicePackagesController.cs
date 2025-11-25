using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models.ServicePackage;

namespace PerHue.Api.Controllers.Admin
{
	[Route("api/admin/[controller]")]
	[ApiController]
	[Authorize(Roles = "admin")]
	public class ServicePackagesController : ControllerBase
	{
		private readonly IServicesProvider _servicesProvider;

		public ServicePackagesController(IServicesProvider servicesProvider)
		{
			_servicesProvider = servicesProvider;
		}

		[HttpGet]
		public async Task<IEnumerable<ServicePackageModel>> Gets()
		{
			var models = await _servicesProvider.ServicePackageService.GetAllAsync();
			return models;
		}

		[HttpGet("{id}")]
		public async Task<ServicePackageModel> Get(int id)
		{
			return await _servicesProvider.ServicePackageService.GetByIdAsync(id);
		}

		[HttpPost]
		public async Task Post([FromBody] ServicePackageModel model)
		{
			model.CreatedDate = DateTime.Now;
			model.UpdatedDate = DateTime.Now;
			await _servicesProvider.ServicePackageService.CreateAsync(model);
		}

		[HttpPut("{id}")]
		public async Task Put(int id, [FromBody] ServicePackageModel model)
		{
			var packageService = await _servicesProvider.ServicePackageService.GetByIdAsync(id);
			if (packageService == null)
			{
				NotFound();
			}
			model.CreatedDate = packageService.CreatedDate;
			model.UpdatedDate = DateTime.Now;
			await _servicesProvider.ServicePackageService.UpdateAsync(id, model);
			Ok();
		}

		[HttpDelete("{id}")]
		public async Task Delete(int id)
		{
			var isExists = await _servicesProvider.ServicePackageService.GetByIdAsync(id) == null;
			if (!isExists)
			{
				NotFound();
			}
			await _servicesProvider.ServicePackageService.DeleteAsync(id);
		}
	}
}
