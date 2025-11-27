using PerHue.Application.Models.AiTest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Application.IServices
{
	public interface IColorMatchingService
	{
		Task<List<ColorMatchResult>> MatchColorsFromHexCodesAsync(List<string> hexCodes);
		Task<ColorMatchResult> FindClosestColorAsync(string hexCode);
	}
}