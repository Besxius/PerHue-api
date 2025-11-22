using PerHue.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Domain.IRepositories
{
    public interface IExpertRepository
    {
        Task<Expert> GetByIdAsync(int id);
        Task<IEnumerable<Expert>> GetAllAsync();
        Task CreateAsync(Expert expert);
        Task UpdateAsync(Expert expert);
        Task<bool> DeleteAsync(int id);
		Task<bool> ExistsAsync(int id);
		Task<IEnumerable<Expert>> GetAllByRatingDescendingAsync();
	}
}