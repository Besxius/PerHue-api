using PerHue.Application.Models.AiTest;
using PerHue.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Application.IServices
{
	public interface IColorMatchingService
	{
		Task<List<ColorMatchResult>> MatchColorsFromHexCodesAsync(List<string> hexCodes, List<Color> allColors);
		Task<ColorMatchResult> FindClosestColorAsync(string hexCode);
		Task<List<Color>> GetAllColorsForMatchingAsync();
	}
}