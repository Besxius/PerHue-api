using PerHue.Application.Models.AiTest;
using System.Threading.Tasks;

namespace PerHue.Application.IServices
{
	public interface IVirtualTryOnService
	{
		Task<VirtualTryOnResponse> GenerateVirtualTryOnImagesAsync(VirtualTryOnRequest request);
		Task<HuggingFaceModel.HFVirtualTryOnResponse> HFGenerateVirtualTryOnImagesAsync(VirtualTryOnRequest request);
	}
}