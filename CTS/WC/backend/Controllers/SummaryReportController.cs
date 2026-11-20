using Microsoft.AspNetCore.Mvc;
using QueryTracker.Api.Services;

namespace QueryTracker.Api.Controllers
{
    /// <summary>
    /// Replaces Reports/SummaryReport.aspx.cs
    /// GET /api/summaryreport/stages?customerCode=163
    /// GET /api/summaryreport/counts?customerCode=163&stageId=1
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SummaryReportController : ControllerBase
    {
        private readonly ISummaryReportService _svc;

        public SummaryReportController(ISummaryReportService svc)
        {
            _svc = svc;
        }

        // GET /api/summaryreport/stages?customerCode=163
        [HttpGet("stages")]
        public IActionResult GetStages([FromQuery] int customerCode)
        {
            var data = _svc.GetAllStages(customerCode);
            return Ok(data);
        }

        // GET /api/summaryreport/rows?customerCode=163
        // Returns full summary rows (stages + all counts) in one call
        [HttpGet("rows")]
        public IActionResult GetSummaryRows([FromQuery] int customerCode)
        {
            var data = _svc.GetSummaryRows(customerCode);
            return Ok(data);
        }
    }
}
