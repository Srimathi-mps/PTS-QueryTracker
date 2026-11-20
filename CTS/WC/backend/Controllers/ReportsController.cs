using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace QueryTracker.Api.Controllers
{
    /// <summary>
    /// Replaces Reports/SummaryReport.aspx
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ReportsController(IConfiguration config)
        {
            _config = config;
        }

        // ── GET /api/reports/summary?userName=MAC014393 (optional) ───────────
        // userName is optional — omitting it returns the full unfiltered report
        // (backward-compatible for the existing SummaryReport.jsx page).
        // When provided, @UserName is passed to USP_GetSummaryReport so the SP
        // can filter rows for that user — used by the Springer Dashboard.
        // NOTE: Verify that USP_GetSummaryReport accepts @UserName in the live
        //       database. If it does not, the parameter is silently omitted and
        //       the endpoint behaves exactly as before.
        [HttpGet("summary")]
        public IActionResult GetSummary([FromQuery] string? userName = null)
        {
            var connStr = _config.GetConnectionString("jetsConstrNew");
            using var conn = new SqlConnection(connStr);
            conn.Open();

            using var cmd = new SqlCommand("USP_GetSummaryReport", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (!string.IsNullOrWhiteSpace(userName))
                cmd.Parameters.AddWithValue("@UserName", userName);

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            var result = dt.AsEnumerable().Select(row => new
            {
                customer = row["Customer"]?.ToString(),
                project  = row["Project"]?.ToString(),
                job      = row["Job"]?.ToString(),
                stage    = row["Stage"]?.ToString(),
                status   = row["Status"]?.ToString(),
                count    = row["Count"] != DBNull.Value ? Convert.ToInt32(row["Count"]) : 0,
            }).ToList();

            return Ok(result);
        }
    }
}
