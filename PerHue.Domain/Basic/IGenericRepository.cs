using System.Linq;
using System.Linq.Expressions;

namespace PerHue.Domain.Basic
{
	public interface IGenericRepository<T> where T : class
	{
		void Create(T entity);
		Task<int> CreateAsync(T entity);
		List<T> GetAll();
		Task<List<T>> GetAllAsync();
		T GetById(int id);
		T GetById(string code);
		T GetById(Guid code);
		Task<T> GetByIdAsync(int id);
		Task<T> GetByIdAsync(string code);
		Task<T> GetByIdAsync(Guid code);
		void PrepareCreate(T entity);
		void PrepareRemove(T entity);
		void PrepareUpdate(T entity);
		bool Remove(T entity);
		Task<bool> RemoveAsync(T entity);
		int Save();
		Task<int> SaveAsync();
		void Update(T entity);
		Task<int> UpdateAsync(T entity);
		IQueryable<T> GetQueryable();

		Task<bool> DeleteAsync(int id);
		Task<bool> ExistsAsync(int id);
		Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
		Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
		Task<int> CountAsync();
		Task<int> CountAsync(Expression<Func<T, bool>> predicate);
	}

}
