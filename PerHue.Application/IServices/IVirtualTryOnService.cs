using PerHue.Application.Models.AiTest;

namespace PerHue.Application.IServices
{
	public interface IVirtualTryOnService
	{
		Task<VirtualTryOnResponse> GenerateVirtualTryOnImagesAsync(VirtualTryOnRequest request);
	}
}