using PerHue.Domain.Basic;
using PerHue.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Domain.IRepositories;

public interface IVerificationRepository : IGenericRepository<VerifyInformation>
{
    Task<IEnumerable<VerifyInformation>> GetAllVerificationRequestsAsync();
    Task<VerifyInformation> GetVerificationRequestByIdAsync(int id);
    Task CreateVerificationRequestAsync(VerifyInformation verifyInformation);
    Task DeleteVerificationRequestAsync(int id);
    Task<bool> ExistsAsync(int userId);
}