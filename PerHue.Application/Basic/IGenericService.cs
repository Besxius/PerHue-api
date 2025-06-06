namespace PerHue.Application.Basic
{
	public interface IGenericService<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> GetByIdAsync(int id);
		Task<bool> DeleteAsync(int id);
	}
}
