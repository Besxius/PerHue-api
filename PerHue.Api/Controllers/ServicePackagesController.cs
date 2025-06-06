using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PerHue.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
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
			await _servicesProvider.ServicePackageService.CreateAsync(model);
		}

		[HttpPut("{id}")]
		public async Task Put(int id, [FromBody] ServicePackageModel model)
		{
			var isExists = await _servicesProvider.ServicePackageService.GetByIdAsync(id) == null;
			if (!isExists)
			{
				NotFound();
			}
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
