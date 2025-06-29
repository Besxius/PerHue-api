using PerHue.Domain.Basic;
using PerHue.Domain.Entities;

namespace PerHue.Domain.IRepositories
{
	public interface ITestResultRepository : IGenericRepository<TestResult>
	{
		Task<IEnumerable<TestResult>> GetAllAsync();
		Task<TestResult> GetByIdAsync(int id);
	}
}
