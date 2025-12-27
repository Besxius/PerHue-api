using Microsoft.EntityFrameworkCore;
using PerHue.Domain.Entities;
using PerHue.Domain.IRepositories;
using PerHue.Infrastructure.Basic;
using PerHue.Infrastructure.Persistence;
using PerHue.Infrastructure.Utils;

namespace PerHue.Infrastructure.Repositories;

public class VerificationRepository : GenericRepository<VerifyInformation>, IVerificationRepository
{
    public VerificationRepository(PerHueDbContext context) : base(context)
	{
    }

    public async Task<IEnumerable<VerifyInformation>> GetAllVerificationRequestsAsync()
    {
        return await _context.VerifyInformations
            .Include(v => v.IdNavigation)
			.Include(v => v.Photos)
            .ToListAsync();
    }

    public async Task<VerifyInformation> GetVerificationRequestByIdAsync(int id)
    {
        return await _context.VerifyInformations
            .Include(v => v.IdNavigation)
			.Include(v => v.Photos)
			.FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<VerifyInformation> CreateVerificationRequestAsync(VerifyInformation verifyInformation)
    {
        var returnEntity = await _context.VerifyInformations.AddAsync(verifyInformation);
		return returnEntity.Entity;
	}

    public async Task DeleteVerificationRequestAsync(int id, bool chosenStatus)
    {
        var verifyInformation = await _context.VerifyInformations.FindAsync(id);
        if (verifyInformation != null)
        {
            if (chosenStatus)
			{
				verifyInformation.Status = VerificationStatus.Approved.ToString();
			}
			else
			{
				verifyInformation.Status = VerificationStatus.Denied.ToString();
			}
		}
    }

    public async Task<bool> ExistsAsync(int userId)
    {
        return await _context.VerifyInformations.AnyAsync(v => v.Id == userId);
    }
}