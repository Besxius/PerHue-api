using PerHue.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Domain.IRepositories;

public interface IVerificationRepository
{
    Task<IEnumerable<VerifyInformation>> GetAllVerificationRequestsAsync();
    Task<VerifyInformation> GetVerificationRequestByIdAsync(int id);
    Task CreateVerificationRequestAsync(VerifyInformation verifyInformation);
    Task DeleteVerificationRequestAsync(int id);
    Task<bool> ExistsAsync(int userId);
}