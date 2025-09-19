using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Repositories
{
    public class ExpertRepository : IExpertRepository
    {
        private readonly PerHueDbContext _context;

        public ExpertRepository(PerHueDbContext context)
        {
            _context = context;
        }

        public async Task<Expert> GetByIdAsync(int id)
        {
            return await _context.Experts
                .Include(e => e.IdNavigation)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Expert>> GetAllAsync()
        {
            return await _context.Experts
                .Include(e => e.IdNavigation)
                .ToListAsync();
        }

        public async Task CreateAsync(Expert expert)
        {
            await _context.Experts.AddAsync(expert);
        }

        public async Task UpdateAsync(Expert expert)
        {
            _context.Entry(expert).State = EntityState.Modified;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var expert = await _context.Experts.FindAsync(id);
            if (expert == null)
                return false;

            _context.Experts.Remove(expert);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Experts.AnyAsync(e => e.Id == id);
        }
    }
}