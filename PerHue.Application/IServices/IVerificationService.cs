using PerHue.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Application.IServices
{
    public interface IVerificationService
    {
        Task<IEnumerable<VerifyRequestModel>> GetAllVerificationRequestsAsync();
        Task<VerifyRequestModel> GetVerificationRequestByIdAsync(int id);
        Task SubmitVerificationAsync(int userId, VerifyRequestModel model);
        Task<bool> AcceptVerificationAsync(int id); // Updated to return bool
        Task<bool> DenyVerificationAsync(int id, string reason); // Updated to return bool
        Task<bool> HasPendingVerificationAsync(int userId);
    }
}