using Microsoft.AspNetCore.Http;
using PerHue.Application.Models;
using PerHue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PerHue.Application.Models.AiTestModel;

namespace PerHue.Application.IServices
{
	public interface IAiTestService
	{
		Task<AiTestResponseModel> CreateAiTestRequestAsync(int userId, CreateAiTestRequestModel model);
		Task<AiTestResponseModel> ProcessAiTestAsync(int testRequestId);
		Task<AiTestResponseModel?> GetAiTestResultAsync(int testRequestId, int userId);
		Task<List<AiTestResponseModel>> GetUserAiTestsAsync(int userId);
	}
}
