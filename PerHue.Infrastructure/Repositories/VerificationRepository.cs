using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Infrastructure.Repositories;

public class VerificationRepository : IVerificationRepository
{
    private readonly PerHueDbContext _context;

    public VerificationRepository(PerHueDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VerifyInformation>> GetAllVerificationRequestsAsync()
    {
        return await _context.VerifyInformations
            .Include(v => v.IdNavigation)
            .ToListAsync();
    }

    public async Task<VerifyInformation> GetVerificationRequestByIdAsync(int id)
    {
        return await _context.VerifyInformations
            .Include(v => v.IdNavigation)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task CreateVerificationRequestAsync(VerifyInformation verifyInformation)
    {
        await _context.VerifyInformations.AddAsync(verifyInformation);
    }

    public async Task DeleteVerificationRequestAsync(int id)
    {
        var verifyInformation = await _context.VerifyInformations.FindAsync(id);
        if (verifyInformation != null)
        {
            _context.VerifyInformations.Remove(verifyInformation);
        }
    }

    public async Task<bool> ExistsAsync(int userId)
    {
        return await _context.VerifyInformations.AnyAsync(v => v.Id == userId);
    }
}