using Microsoft.AspNetCore.Mvc;
using PerHue.Application.IServicesProvider;
using PerHue.Application.Models;
using PerHue.Application.Models.Dashboard;

namespace PerHue.Api.Controllers.Admin
{
    [Route("api/admin/dashboard")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IServicesProvider _servicesProvider;

        public AdminDashboardController(IServicesProvider servicesProvider)
        {
            _servicesProvider = servicesProvider;
        }

        /// <summary>
        /// Get high-level dashboard metrics
        /// </summary>
        [HttpGet("metrics")]
        public async Task<ActionResult<DashboardMetricsModel>> GetDashboardMetrics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var metrics = await _servicesProvider.AdminDashboardService.GetDashboardMetricsAsync(startDate, endDate);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get account count statistics
        /// </summary>
        [HttpGet("accounts/count")]
        public async Task<ActionResult<AccountCountModel>> GetAccountCounts([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var counts = await _servicesProvider.AdminDashboardService.GetAccountCountsAsync(startDate, endDate);
                return Ok(counts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get paginated account list with search and filtering
        /// </summary>
        [HttpGet("accounts")]
        public async Task<ActionResult<PaginatedResultV2<AccountListItemModel>>> GetAccountList([FromQuery] AccountSearchModel searchModel)
        {
            try
            {
                var accounts = await _servicesProvider.AdminDashboardService.GetAccountListAsync(searchModel);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get detailed account information
        /// </summary>
        [HttpGet("accounts/{id}")]
        public async Task<ActionResult<AccountDetailModel>> GetAccountDetail(int id)
        {
            try
            {
                var accountDetail = await _servicesProvider.AdminDashboardService.GetAccountDetailAsync(id);
                if (accountDetail == null)
                {
                    return NotFound(new { message = "Account not found" });
                }
                return Ok(accountDetail);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get expert penalty statistics
        /// </summary>
        [HttpGet("experts/penalties")]
        public async Task<ActionResult<PaginatedResultV2<ExpertPenaltyModel>>> GetExpertPenalties([FromQuery] ExpertPenaltySearchModel searchModel)
        {
            try
            {
                var penalties = await _servicesProvider.AdminDashboardService.GetExpertPenaltiesAsync(searchModel);
                return Ok(penalties);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get revenue statistics for a period
        /// </summary>
        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueStatisticsModel>> GetRevenueStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string? groupBy = "day")
        {
            try
            {
                var revenue = await _servicesProvider.AdminDashboardService.GetRevenueStatisticsAsync(startDate, endDate, groupBy);
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get test count statistics for a period
        /// </summary>
        [HttpGet("tests/count")]
        public async Task<ActionResult<TestCountStatisticsModel>> GetTestCountStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string? groupBy = "day", [FromQuery] string? testType = null)
        {
            try
            {
                var testStats = await _servicesProvider.AdminDashboardService.GetTestCountStatisticsAsync(startDate, endDate, groupBy, testType);
                return Ok(testStats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get penalty statistics for a period
        /// </summary>
        [HttpGet("penalties/statistics")]
        public async Task<ActionResult<PenaltyStatisticsModel>> GetPenaltyStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string? groupBy = "day")
        {
            try
            {
                var penalties = await _servicesProvider.AdminDashboardService.GetPenaltyStatisticsAsync(startDate, endDate, groupBy);
                return Ok(penalties);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get expert activity overview
        /// </summary>
        [HttpGet("experts/activity")]
        public async Task<ActionResult<PaginatedResultV2<ExpertActivityModel>>> GetExpertActivity([FromQuery] ExpertActivitySearchModel searchModel)
        {
            try
            {
                var activity = await _servicesProvider.AdminDashboardService.GetExpertActivityAsync(searchModel);
                return Ok(activity);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}