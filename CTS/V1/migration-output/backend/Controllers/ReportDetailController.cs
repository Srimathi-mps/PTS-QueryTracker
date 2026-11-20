using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace QueryTracker.Api.Controllers
{
    /// <summary>
    /// Replaces Reports/PTSResolved.aspx and Reports/AwaitingReplyReport.aspx
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReportDetailController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ReportDetailController(IConfiguration config) => _config = config;

        private SqlConnection OpenConnection()
        {
            var conn = new SqlConnection(_config.GetConnectionString("jetsConstrNew"));
            conn.Open();
            return conn;
        }

        // GET /api/reportdetail/resolved?stageId=1&customerCode=163&queryType=QUERY&fromDate=&toDate=&userCode=0
        // Mirrors: PTSResolved.aspx → usp_PTS_GetPTSResolvedReportByCustomer_WithQueryType
        [HttpGet("resolved")]
        public IActionResult GetResolved(
            [FromQuery] string stageId,
            [FromQuery] int    customerCode,
            [FromQuery] string queryType  = "QUERY",
            [FromQuery] string? fromDate  = null,
            [FromQuery] string? toDate    = null,
            [FromQuery] string  userCode  = "0")
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("usp_PTS_GetPTSResolvedReportByCustomer_WithQueryType", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@stage",        stageId ?? string.Empty);
            cmd.Parameters.AddWithValue("@user",         string.IsNullOrEmpty(userCode) ? "0" : userCode);
            cmd.Parameters.AddWithValue("@fDate",        string.IsNullOrEmpty(fromDate) ? (object)DBNull.Value : fromDate);
            cmd.Parameters.AddWithValue("@tDate",        string.IsNullOrEmpty(toDate)   ? (object)DBNull.Value : toDate);
            cmd.Parameters.AddWithValue("@CustomerCode", customerCode);
            cmd.Parameters.AddWithValue("@QueryType",    queryType ?? "QUERY");

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            var result = dt.AsEnumerable().Select(r => new
            {
                customerName   = r["Cust_name"]?.ToString()?.Trim(),
                bookName       = r["Jour_sname"]?.ToString()?.Trim(),
                customerCode   = r["Cust_code"],
                bookCode       = r["Jour_code"],
                stage          = r["Stage"]?.ToString(),
                chapter        = r["Article"]?.ToString(),
                queryRaised    = r["CustRemarks"]?.ToString(),
                resolvedRemarks= r["ResolveRemarks"]?.ToString(),
                reportedDate   = r["Reporteddate"] != DBNull.Value ? Convert.ToDateTime(r["Reporteddate"]).ToString("dd-MMM-yyyy") : "",
                reopenedDate   = r["ReOpenedDate"] != DBNull.Value ? Convert.ToDateTime(r["ReOpenedDate"]).ToString("dd-MMM-yyyy") : "",
                resolvedDate   = r["ResolvedDate"] != DBNull.Value ? Convert.ToDateTime(r["ResolvedDate"]).ToString("dd-MMM-yyyy") : "",
                totalTime      = r["TimeTaken"],
            }).ToList();

            return Ok(result);
        }

        // GET /api/reportdetail/awaiting?stageId=1&customerCode=163&queryType=QUERY
        // Mirrors: AwaitingReplyReport.aspx → usp_PTS_GetAwaitingReplyReportByCustomer_WithQueryTypeNew
        [HttpGet("awaiting")]
        public IActionResult GetAwaiting(
            [FromQuery] string stageId,
            [FromQuery] int    customerCode,
            [FromQuery] string queryType  = "QUERY",
            [FromQuery] string? fromDate  = null,
            [FromQuery] string? toDate    = null,
            [FromQuery] string  userCode  = "0")
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("usp_PTS_GetAwaitingReplyReportByCustomer_WithQueryTypeNew", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@stage",        stageId ?? string.Empty);
            cmd.Parameters.AddWithValue("@user",         string.IsNullOrEmpty(userCode) ? "0" : userCode);
            cmd.Parameters.AddWithValue("@fDate",        string.IsNullOrEmpty(fromDate) ? (object)DBNull.Value : fromDate);
            cmd.Parameters.AddWithValue("@tDate",        string.IsNullOrEmpty(toDate)   ? (object)DBNull.Value : toDate);
            cmd.Parameters.AddWithValue("@CustomerCode", customerCode);
            cmd.Parameters.AddWithValue("@QueryType",    queryType ?? "QUERY");

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            var result = dt.AsEnumerable().Select(r => new
            {
                customerName = r["cust_sname"]?.ToString()?.Trim(),
                bookName     = r["Jour_sname"]?.ToString()?.Trim(),
                customerCode = r["CUSTOMER"],
                bookCode     = r["Journal"],
                stage        = r["Stage"]?.ToString(),
                chapter      = r["Article"]?.ToString(),
                queryRaised  = r["PTSRemarks"]?.ToString(),
                reportedDate = r["Reporteddate"] != DBNull.Value ? Convert.ToDateTime(r["Reporteddate"]).ToString("dd-MMM-yyyy") : "",
                days         = r["days"],
            }).ToList();

            return Ok(result);
        }
    }
}
