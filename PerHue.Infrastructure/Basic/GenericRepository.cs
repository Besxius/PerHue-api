using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Basic;
using PerHue.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace PerHue.Infrastructure.Basic
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		protected PerHueDbContext _context;
		protected readonly DbSet<T> _dbSet;

		public GenericRepository(PerHueDbContext context)
		{
			_context = context;
			_dbSet = context.Set<T>();
		}

		public List<T> GetAll()
		{
			return _context.Set<T>().ToList();
		}
		public async Task<List<T>> GetAllAsync()
		{
			return await _context.Set<T>().ToListAsync();
		}
		public void Create(T entity)
		{
			_context.Add(entity);
			_context.SaveChanges();
		}

		public async Task<int> CreateAsync(T entity)
		{
			_context.Add(entity);
			return await _context.SaveChangesAsync();
		}
		public void Update(T entity)
		{
			_context.ChangeTracker.Clear();
			var tracker = _context.Attach(entity);
			tracker.State = EntityState.Modified;
			_context.SaveChanges();
		}

		public virtual async Task<int> UpdateAsync(T entity)
		{
			_context.ChangeTracker.Clear();
			var tracker = _context.Attach(entity);
			tracker.State = EntityState.Modified;
			return await _context.SaveChangesAsync();
		}

		public bool Remove(T entity)
		{
			_context.Remove(entity);
			_context.SaveChanges();
			return true;
		}

		public async Task<bool> RemoveAsync(T entity)
		{
			_context.Remove(entity);
			await _context.SaveChangesAsync();
			return true;
		}

		public T GetById(int id)
		{
			return _context.Set<T>().Find(id);
		}

		public async Task<T> GetByIdAsync(int id)
		{
			return await _context.Set<T>().FindAsync(id);
		}

		public T GetById(string code)
		{
			return _context.Set<T>().Find(code);
		}

		public async Task<T> GetByIdAsync(string code)
		{
			return await _context.Set<T>().FindAsync(code);
		}

		public T GetById(Guid code)
		{
			return _context.Set<T>().Find(code);
		}

		public async Task<T> GetByIdAsync(Guid code)
		{
			return await _context.Set<T>().FindAsync(code);
		}

		public virtual async Task<bool> DeleteAsync(int id)
		{
			var entity = await GetByIdAsync(id);
			if (entity == null)
				return false;

			_dbSet.Remove(entity);
			await _context.SaveChangesAsync();
			return true;
		}

		public virtual async Task<bool> ExistsAsync(int id)
		{
			var entity = await GetByIdAsync(id);
			return entity != null;
		}

		public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbSet.Where(predicate).ToListAsync();
		}

		public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbSet.FirstOrDefaultAsync(predicate);
		}

		public virtual async Task<int> CountAsync()
		{
			return await _dbSet.CountAsync();
		}

		public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbSet.CountAsync(predicate);
		}

		#region Separating asigned entity and save operators        

		public void PrepareCreate(T entity)
		{
			_context.Add(entity);
		}

		public void PrepareUpdate(T entity)
		{
			var tracker = _context.Attach(entity);
			tracker.State = EntityState.Modified;
		}

		public void PrepareRemove(T entity)
		{
			_context.Remove(entity);
		}

		public int Save()
		{
			return _context.SaveChanges();
		}

		public async Task<int> SaveAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public IQueryable<T> GetQueryable()
		{
			return _context.Set<T>().AsQueryable();
		}

		#endregion Separating asign entity and save operators
	}
}
