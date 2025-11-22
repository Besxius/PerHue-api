using PerHue.Application.Models.Expert;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Application.IServices
{
    public interface IExpertService
    {
        Task<ExpertModel> GetByIdAsync(int id);
        Task<IEnumerable<ExpertModel>> GetAllAsync();
        Task<bool> UpdateAsync(int id, UpdateExpertModel model);
        Task<bool> DeleteAsync(int id);
		Task<bool> ExistsAsync(int id);
		Task<IEnumerable<ExpertModel>> GetAllByRatingDescendingAsync();
	}
}