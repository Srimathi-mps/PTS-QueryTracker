using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace CTS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MigrationController : ControllerBase
    {
        private readonly string _server1Connection;
        private readonly string _server2Connection;

        public MigrationController(IConfiguration configuration)
        {
            _server1Connection = configuration.GetConnectionString("Server1Connection");
            _server2Connection = configuration.GetConnectionString("Server2Connection");
        }

        [HttpGet("preview")]
        public async Task<IActionResult> GetMigrationData(
            [FromQuery] string targetJournalName,
            [FromQuery] int sourceJournalCode,
            [FromQuery] int targetCustomerCode,
            [FromQuery] int targetDepartmentId,
            [FromQuery] int? sourceCustomerCode = null)
        {
            try
            {
                // 1. Get jour_code and Cust_sname from Server 1
                using var server1Conn = new SqlConnection(_server1Connection);
                
                // Get Journal
                var journalQuery = "SELECT * FROM journal WHERE jour_sname LIKE '%' + @TargetJournalName + '%'";
                var targetJournal = await server1Conn.QueryFirstOrDefaultAsync(journalQuery, new { TargetJournalName = targetJournalName });

                if (targetJournal == null)
                {
                    return BadRequest(new { Success = false, Message = $"Target journal '{targetJournalName}' not found on Server 1." });
                }

                int resolvedJournalCode = 0;
                var journalDict = (IDictionary<string, object>)targetJournal;
                
                string? jourCodeKey = null;
                foreach (var k in journalDict.Keys)
                {
                    if (k.Equals("jour_code", StringComparison.OrdinalIgnoreCase))
                    {
                        jourCodeKey = k;
                        break;
                    }
                }

                if (jourCodeKey != null)
                {
                    resolvedJournalCode = Convert.ToInt32(journalDict[jourCodeKey]);
                }
                else
                {
                    return StatusCode(500, new { Success = false, Message = "Target journal found, but 'jour_code' column is missing." });
                }

                // Get Customer Name from Server 1
                var customerSnameQuery = "SELECT Cust_sname FROM Customer WHERE Cust_code = @TargetCustomerCode";
                var resolvedCustSname = await server1Conn.QueryFirstOrDefaultAsync<string>(customerSnameQuery, new { TargetCustomerCode = targetCustomerCode });

                if (string.IsNullOrEmpty(resolvedCustSname))
                {
                    return BadRequest(new { Success = false, Message = $"Customer with code '{targetCustomerCode}' not found on Server 1." });
                }

                resolvedCustSname = resolvedCustSname.Trim();

                // 2. Resolve Source Customer Code on Server 2 (only if not provided by user)
                using var server2Conn = new SqlConnection(_server2Connection);
                
                int finalSourceCustomerCode;
                if (sourceCustomerCode.HasValue)
                {
                    finalSourceCustomerCode = sourceCustomerCode.Value;
                }
                else
                {
                    var sourceCustomerCodeQuery = "SELECT Cust_code FROM Customer WHERE Cust_sname = @CustSname";
                    var resolvedSourceCode = await server2Conn.QueryFirstOrDefaultAsync<int?>(sourceCustomerCodeQuery, new { CustSname = resolvedCustSname });

                    if (resolvedSourceCode == null)
                    {
                        return BadRequest(new { Success = false, Message = $"Customer with name '{resolvedCustSname}' not found on Server 2." });
                    }
                    finalSourceCustomerCode = resolvedSourceCode.Value;
                }

                // 3. Execute main query on Server 2
                // Mapping IntProcessIdList for Customer 163 as per instruction
                string intProcessIdList = targetCustomerCode == 163 
                    ? "109,310,312,313,110,111,101,318,112,113,116,117,118,119,122,127,128,129,130,316,134,135,149,150"
                    : ""; // Default or from parameter if we had one

                var query = @"
                    SELECT 
                        ID,
                        [SUBSTAGEID],
                        @SourceCustomerCode AS customercode,
                        @ResolvedJournalCode AS journal,
                        [Article],
                        @TargetDepartmentId AS DepartmentId,
                        [Volume],
                        [Issue],
                        [PTSRemarks],
                        [TaskClosed],
                        [ReportedDate],
                        UserCode,
                        [ProblemTypeCode],
                        [ProblemMsgID],
                        [ZipFileUpload],
                        [QuerySavedBy],
                        [QuerySavedDate],
                        [QueryClosedBy],
                        [QueryClosedDate],
                        [QueryType],
                        [CreatedBy],
                        [CreatedDate],
                        [LastUpdatedBy],
                        [LastUpdatedDate],
                        [TaskClosedDate],
                        [PtsDueDate],
                        [ZipFilePath],
                        [Sequence],
                        [JobChapterId],
                        [RoundCount],
                        [JobChapterStageID],
                        null AS ManuscriptPath,
                        null AS GraphicPath,
                        null AS IsLatex,
                        null AS IsReferenceResolved,
                        @IntProcessIdList AS IntProcessIdList,
                        [Proceed_Req]
                    FROM [BooksItrack].[dbo].[tbl_PTS_Problem] 
                    WHERE customercode = @SourceCustomerCode 
                      AND journal = @SourceJournalCode 
                      AND DepartmentId = 24";

                var data = await server2Conn.QueryAsync(query, new { 
                    ResolvedJournalCode = resolvedJournalCode,
                    SourceJournalCode = sourceJournalCode,
                    SourceCustomerCode = finalSourceCustomerCode,
                    TargetCustomerCode = targetCustomerCode,
                    TargetDepartmentId = targetDepartmentId,
                    IntProcessIdList = intProcessIdList
                });
                
                return Ok(new 
                {
                    Success = true,
                    Message = "Data successfully fetched with aliased fields.",
                    ResolvedJournalCode = resolvedJournalCode,
                    ResolvedCustSname = resolvedCustSname,
                    ResolvedSourceCustomerCode = finalSourceCustomerCode,
                    SourceJournalCodeUsed = sourceJournalCode,
                    PreviewData = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message, StackTrace = ex.StackTrace });
            }
        }
    }
}
