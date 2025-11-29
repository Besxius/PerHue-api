using PerHue.Application.Models;
using PerHue.Application.Models.VerifyInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerHue.Application.IServices
{
    public interface IVerificationService
    {
        Task<PaginatedResultV2<VerifyRequestModel>> GetAllAsync(VerificationSearchModel searchModel);
        Task<VerifyRequestModel> GetVerificationRequestByIdAsync(int id);
        Task SubmitVerificationAsync(int userId, VerifyRequestModel model);
        Task<bool> AcceptVerificationAsync(int id); // Updated to return bool
        Task<bool> DenyVerificationAsync(int id, string reason); // Updated to return bool
        Task<bool> HasPendingVerificationAsync(int userId);
    }
}