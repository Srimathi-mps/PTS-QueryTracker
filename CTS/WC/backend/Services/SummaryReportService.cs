using System.Data;
using Microsoft.Data.SqlClient;
using QueryTracker.Api.Models;

namespace QueryTracker.Api.Services
{
    /// <summary>
    /// Implements all data-access logic from Reports/SummaryReport.aspx.cs.
    /// Mirrors the exact SP calls in FillStages() and the static helper methods.
    /// </summary>
    public class SummaryReportService : ISummaryReportService
    {
        private readonly string _connStr;

        public SummaryReportService(IConfiguration config)
        {
            _connStr = config.GetConnectionString("jetsConstrNew")!;
        }

        private SqlConnection OpenConnection()
        {
            var conn = new SqlConnection(_connStr);
            conn.Open();
            return conn;
        }

        // ── GetAllStages ───────────────────────────────────────────────────────
        // Mirrors: GetAllStagesWithQueries(ddlCustomer.SelectedValue)
        // SP: usp_PTS_GetAllStagesWithQueriesNew(@CustomerCode int)
        // Returns: PTS_StageId, SubStageName, SubStageDescription

        public IEnumerable<StageItem> GetAllStages(int customerCode)
        {
            using var conn = OpenConnection();
            //using var cmd  = new SqlCommand("usp_PTS_GetAllStagesWithQueriesNew", conn)
            using var cmd  = new SqlCommand("usp_PTS_GetAllStagesWithQueriesNew1", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@CustomerCode", customerCode);

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            return dt.AsEnumerable().Select(r => new StageItem
            {
                StageId          = Convert.ToInt32(r["PTS_StageId"]),
                StageName        = r["SubStageName"]?.ToString()        ?? string.Empty,
                StageDescription = r["SubStageDescription"]?.ToString() ?? string.Empty,
            }).ToList();
        }

        // ── GetSummaryRows ─────────────────────────────────────────────────────
        // Mirrors: FillStages() — iterates stages and calls 4 count SPs per row.
        // Matches the screenshot columns exactly:
        //   Stage | Instruction | Queries pending | Queries Resolved (Last 30 days)

        public IEnumerable<SummaryReportRow> GetSummaryRows(int customerCode)
        {
            var stages = GetAllStages(customerCode).ToList();
            var result = new List<SummaryReportRow>();

            using var conn = OpenConnection();

            foreach (var stage in stages)
            {
                string stageIdStr = stage.StageId.ToString();

                result.Add(new SummaryReportRow
                {
                    StageId          = stage.StageId,
                    StageName        = stage.StageName,
                    StageDescription = stage.StageDescription,

                    // Instructions awaiting reply
                    // Mirrors: GetStageRecordInstructionCountByCustomer(stageId, taskClosed=1, custCode)
                    // SP: usp_PTS_Instruction_GetAwaitingReplyRecordCountByCustomerNew(@stage, @Customer)
                    Instruction = GetScalar(conn,
                        "usp_PTS_Instruction_GetAwaitingReplyRecordCountByCustomerNew",
                        ("@stage",    (object)stageIdStr),
                        ("@Customer", (object)customerCode)),

                    // Queries pending (awaiting reply)
                    // Mirrors: GetStageRecordCountByCustomer(stageId, taskClosed=1, custCode)
                    // SP: usp_PTS_GetAwaitingReplyRecordCountByCustomerNew(@stage, @Customer)
                    QueriesPending = GetScalar(conn,
                        "usp_PTS_GetAwaitingReplyRecordCountByCustomerNew",
                        ("@stage",    (object)stageIdStr),
                        ("@Customer", (object)customerCode)),

                    // Queries resolved in last 30 days
                    // Mirrors: GetPTSResolvedRecordCountByCustomer(stageId, custCode)
                    // SP: usp_PTS_GetPTSResolvedRecordCountByCustomerNew(@SubStageID, @CustomerCode)
                    QueriesResolved = GetScalar(conn,
                        "usp_PTS_GetPTSResolvedRecordCountByCustomerNew",
                        ("@SubStageID",   (object)stage.StageId),
                        ("@CustomerCode", (object)customerCode)),
                });
            }

            return result;
        }

        // ── Helper: execute scalar SP ──────────────────────────────────────────

        private static int GetScalar(SqlConnection conn, string spName,
            params (string name, object value)[] parameters)
        {
            using var cmd = new SqlCommand(spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            foreach (var (name, value) in parameters)
                cmd.Parameters.AddWithValue(name, value);

            var result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value
                ? Convert.ToInt32(result) : 0;
        }
    }
}
