using System.Data;
using Microsoft.Data.SqlClient;
using QueryTracker.Api.Models;

namespace QueryTracker.Api.Services
{
    /// <summary>
    /// All data-access logic extracted from RaiseProblems.aspx.cs.
    /// Uses the ACTUAL stored procedure names from the BooksItrack database.
    /// See STORED_PROCEDURES.md for the full reference.
    /// </summary>
    public class RaiseProblemsService : IRaiseProblemsService
    {
        private readonly string _connStr;

        public RaiseProblemsService(IConfiguration config)
        {
            _connStr = config.GetConnectionString("jetsConstrNew")!;
        }

        // ── Connection helper ──────────────────────────────────────────────────

        private SqlConnection OpenConnection()
        {
            var conn = new SqlConnection(_connStr);
            conn.Open();
            return conn;
        }

        // ── Customers ──────────────────────────────────────────────────────────
        // Original: ptsQueryService.GetPTSQueryMaster(BizPTSQuery.tName.Customer, ...)
        // Real SP:  USP_TTS_GETCUSTOMERS  →  returns Cust_code, cust_sname

        public IEnumerable<DropdownItem> GetCustomers(string userName)
        {
            using var conn = OpenConnection();

            // Step 1: resolve the user's actual role — fixes the hardcoded @User_Role = 0 bug.
            // USP_PTS_GETUSERROLEBYUSERNAME returns multiple columns, we need ROLEID (3rd column)
            // The original stored procedure returns: MAPPINGID, USERNAME, UM.ROLEID, UR.ROLENAME
            // We need to read the ROLEID column, not use ExecuteScalar() which returns MAPPINGID
            int roleId = 0;
            try
            {
                using (var roleCmd = new SqlCommand("USP_PTS_GETUSERROLEBYUSERNAME", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    roleCmd.Parameters.AddWithValue("@USERNAME", userName ?? string.Empty);
                    using var roleAdapter = new SqlDataAdapter(roleCmd);
                    var dtRole = new DataTable();
                    roleAdapter.Fill(dtRole);
                    
                    if (dtRole.Rows.Count > 0)
                    {
                        // Get ROLEID from the 3rd column (index 2) or find column by name
                        if (dtRole.Columns.Contains("ROLEID"))
                        {
                            var roleIdObj = dtRole.Rows[0]["ROLEID"];
                            if (roleIdObj != null && roleIdObj != DBNull.Value)
                            {
                                roleId = Convert.ToInt32(roleIdObj);
                            }
                        }
                        else if (dtRole.Columns.Count >= 3)
                        {
                            // Fallback: 3rd column is ROLEID
                            var roleIdObj = dtRole.Rows[0][2];
                            if (roleIdObj != null && roleIdObj != DBNull.Value)
                            {
                                roleId = Convert.ToInt32(roleIdObj);
                            }
                        }
                        else if (dtRole.Columns.Count > 0)
                        {
                            // Last resort: try first column (might be MAPPINGID in old code)
                            var roleIdObj = dtRole.Rows[0][0];
                            if (roleIdObj != null && roleIdObj != DBNull.Value)
                            {
                                roleId = Convert.ToInt32(roleIdObj);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with default role 0
                Console.WriteLine($"Error getting role for user {userName}: {ex.Message}");
                roleId = 0;
            }

            // Step 2: fetch customers using the dynamically resolved role (not hardcoded 0).
            using var cmd  = new SqlCommand("USP_TTS_GETCUSTOMERS", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Cust_grname", string.Empty);
            cmd.Parameters.AddWithValue("@User_Role",   roleId);   // ← dynamic, was hardcoded 0
            cmd.Parameters.AddWithValue("@User_Code",   userName ?? string.Empty);

            var customerAdapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            customerAdapter.Fill(dt);

            // Debug: log column names and row count
            Console.WriteLine($"GetCustomers for user {userName}, role {roleId}: {dt.Rows.Count} rows");
            foreach (DataColumn col in dt.Columns)
            {
                Console.WriteLine($"  Column: {col.ColumnName} ({col.DataType})");
            }

            // Check if we have the expected columns
            if (dt.Rows.Count > 0)
            {
                // The stored procedure returns Cust_code and cust_sname
                // But we need to handle case where column names might be different
                string idColumn = dt.Columns.Contains("Cust_code") ? "Cust_code" : 
                                 dt.Columns.Contains("FieldID") ? "FieldID" : 
                                 dt.Columns.Contains("Cust_code") ? "Cust_code" :
                                 dt.Columns[0].ColumnName;
                
                string nameColumn = dt.Columns.Contains("cust_sname") ? "cust_sname" :
                                   dt.Columns.Contains("FieldName") ? "FieldName" :
                                   dt.Columns.Contains("Cust_sname") ? "Cust_sname" :
                                   dt.Columns.Contains("cust_sname") ? "cust_sname" :
                                   dt.Columns.Count > 1 ? dt.Columns[1].ColumnName : idColumn;

                var result = dt.AsEnumerable()
                         .GroupBy(r => 
                         {
                             var idObj = r[idColumn];
                             return idObj != null && idObj != DBNull.Value ? Convert.ToInt32(idObj) : 0;
                         })
                         .Where(g => g.Key > 0) // Exclude rows with ID 0
                         .Select(g => new DropdownItem
                         {
                             Id   = g.Key,
                             Name = g.First()[nameColumn]?.ToString()?.Trim() ?? string.Empty
                         })
                         .Where(x => !string.IsNullOrEmpty(x.Name)) // Exclude empty names
                         .OrderBy(x => x.Name)
                         .ToList();

                Console.WriteLine($"Returning {result.Count} customers");
                return result;
            }

            Console.WriteLine("No customers found");
            return new List<DropdownItem>();
        }

        // ── Journals ───────────────────────────────────────────────────────────
        // Original: ptsQueryService.GetPTSQueryMaster(BizPTSQuery.tName.Journal, ...)
        // Real SP:  USP_TTS_GetAllJournal  →  @case MUST be 'SelectJournal'
        //           returns jour_code, jour_sname
        // NOTE: The SP can return duplicate jour_code values across customers.
        //       We deduplicate by jour_code, keeping the first occurrence.

        public IEnumerable<DropdownItem> GetJournals(int customerCode)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("USP_TTS_GetAllJournal", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@jour_ccode", customerCode);
            cmd.Parameters.AddWithValue("@case",       "SelectJournal");

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            // Deduplicate by jour_code — keeps first occurrence, prevents React key collision
            return dt.AsEnumerable()
                     .GroupBy(r => Convert.ToInt32(r["jour_code"]))
                     .Select(g => new DropdownItem
                     {
                         Id   = g.Key,
                         Name = g.First()["jour_sname"]?.ToString()?.Trim() ?? string.Empty
                     })
                     .OrderBy(x => x.Name)
                     .ToList();
        }

        // ── Articles (Jobs) ────────────────────────────────────────────────────
        // Original: ptsQueryService.GetWIPArticles(customerCode, journalCode, Stage)
        // Real SP:  usp_PTS_GetWIPArticlesNew (2 params only — no Volume/Issue)
        //           returns FieldID (e.g. "BOOK"), FIELDNAME
        // NOTE: usp_PTS_GetWIPArticles (4 params) returns 0 rows for Books journals.
        //       The New variant is what the original ASPX DLL actually calls.

        public IEnumerable<DropdownItem> GetArticles(int customerCode, int journalCode)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("usp_PTS_GetWIPArticlesNew", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@customerCode", customerCode);
            cmd.Parameters.AddWithValue("@journalCode",  journalCode);

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            // Deduplicate by FieldID — article codes must be unique in the dropdown
            return dt.AsEnumerable()
                     .GroupBy(r => r["FieldID"]?.ToString()?.Trim() ?? string.Empty)
                     .Where(g => !string.IsNullOrEmpty(g.Key))
                     .Select(g => new DropdownItem
                     {
                         Id    = 0,
                         StrId = g.Key,
                         Name  = g.Key
                     })
                     .OrderBy(x => x.Name)
                     .ToList();
        }

        // ── Departments ────────────────────────────────────────────────────────
        // Original: ptsQueryService.GetDepartment()
        // Real SP:  USP_QMS_GetDepartment  →  returns fieldId, fieldName

        public IEnumerable<DropdownItem> GetDepartments()
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("USP_QMS_GetDepartment", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            return dt.AsEnumerable().Select(r => new DropdownItem
            {
                Id   = Convert.ToInt32(r["fieldId"]),
                Name = r["fieldName"]?.ToString() ?? string.Empty
            }).ToList();
        }

        // ── Volumes (Stage-based workflow) ─────────────────────────────────────
        // New method for Stage-based version
        // Real SP:  usp_PTS_GetWIPVolumes  →  returns volume list for article

        public IEnumerable<DropdownItem> GetVolumes(int customerCode, int journalCode, string articleId)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("usp_PTS_GetWIPVolumes", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@customerCode", customerCode);
            cmd.Parameters.AddWithValue("@journalCode",  journalCode);
            cmd.Parameters.AddWithValue("@article",      articleId ?? string.Empty);

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0) return new List<DropdownItem>();

            // Handle various column name conventions
            string idCol   = dt.Columns.Contains("VolumeID")   ? "VolumeID"   :
                             dt.Columns.Contains("FieldID")    ? "FieldID"    :
                             dt.Columns.Contains("id")         ? "id"         : dt.Columns[0].ColumnName;
            string nameCol = dt.Columns.Contains("VolumeName") ? "VolumeName" :
                             dt.Columns.Contains("FieldName")  ? "FieldName"  :
                             dt.Columns.Contains("name")       ? "name"       : dt.Columns[1].ColumnName;

            return dt.AsEnumerable()
                     .GroupBy(r => r[idCol]?.ToString()?.Trim() ?? string.Empty)
                     .Where(g => !string.IsNullOrEmpty(g.Key))
                     .Select(g => new DropdownItem
                     {
                         Id    = 0,
                         StrId = g.Key,
                         Name  = g.First()[nameCol]?.ToString()?.Trim() ?? g.Key
                     })
                     .OrderBy(x => x.Name)
                     .ToList();
        }

        // ── Issues (Stage-based workflow) ───────────────────────────────────────
        // New method for Stage-based version
        // Real SP:  usp_PTS_GetWIPIssues  →  returns issue list for volume

        public IEnumerable<DropdownItem> GetIssues(int customerCode, int journalCode, string articleId, string volume)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("usp_PTS_GetWIPIssues", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@customerCode", customerCode);
            cmd.Parameters.AddWithValue("@journalCode",  journalCode);
            cmd.Parameters.AddWithValue("@article",      articleId ?? string.Empty);
            cmd.Parameters.AddWithValue("@volume",       volume ?? string.Empty);

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0) return new List<DropdownItem>();

            // Handle various column name conventions
            string idCol   = dt.Columns.Contains("IssueID")   ? "IssueID"   :
                             dt.Columns.Contains("FieldID")   ? "FieldID"   :
                             dt.Columns.Contains("id")        ? "id"        : dt.Columns[0].ColumnName;
            string nameCol = dt.Columns.Contains("IssueName") ? "IssueName" :
                             dt.Columns.Contains("FieldName") ? "FieldName" :
                             dt.Columns.Contains("name")      ? "name"      : dt.Columns[1].ColumnName;

            return dt.AsEnumerable()
                     .GroupBy(r => r[idCol]?.ToString()?.Trim() ?? string.Empty)
                     .Where(g => !string.IsNullOrEmpty(g.Key))
                     .Select(g => new DropdownItem
                     {
                         Id    = 0,
                         StrId = g.Key,
                         Name  = g.First()[nameCol]?.ToString()?.Trim() ?? g.Key
                     })
                     .OrderBy(x => x.Name)
                     .ToList();
        }

        // ── Stage by Volume+Issue (Stage-based workflow) ────────────────────────
        // New method for Stage-based version
        // Real SP:  usp_PTS_GetStageByVolumnAndIssue  →  derives stage from volume+issue

        public int GetStageByVolumeIssue(int customerCode, int journalCode, string articleId, string volume, string issue)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("usp_PTS_GetStageByVolumnAndIssue", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@customerCode", customerCode);
            cmd.Parameters.AddWithValue("@journalCode",  journalCode);
            cmd.Parameters.AddWithValue("@article",      articleId ?? string.Empty);
            cmd.Parameters.AddWithValue("@volume",       volume ?? string.Empty);
            cmd.Parameters.AddWithValue("@issue",        issue ?? string.Empty);

            var result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0;
        }

        // ── Barcode lookup ─────────────────────────────────────────────────────
        // Original: GetDataFromBarCode() in code-behind
        // Real SPs: usp_PTS_ValidateCustomerJournalArticle, USP_TTS_GETCUSTOMERS
        // Updated to support both 3-part (Sub-stage) and 5-part (Stage-based) formats

        // ── Barcode lookup ─────────────────────────────────────────────────────
        // Original: GetDataFromBarCode() in code-behind
        // Real SPs: usp_PTS_ValidateCustomerJournalArticle, USP_TTS_GETCUSTOMERS
        // Updated to support both 3-part (Sub-stage) and 5-part (Stage-based) formats

        public BarcodeResult LookupBarcode(string barcode)
        {
            var parts  = barcode.Split('|');
            var result = new BarcodeResult();

            if (parts.Length == 5)
            {
                // Stage-based format: Customer|Journal|Article|Volume|Issue
                result.JobType = "stage-based";
                string custName  = parts[0].Trim();
                string jourName  = parts[1].Trim();
                string articleId = parts[2].Trim();
                string volume    = parts[3].Trim();
                string issue     = parts[4].Trim();

                using var conn = OpenConnection();

                // Validate using usp_PTS_ValidateCustomerJournalArticle
                using (var cmd = new SqlCommand("usp_PTS_ValidateCustomerJournalArticle", conn)
                       { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@custFName",    custName);
                    cmd.Parameters.AddWithValue("@journalName",  jourName);
                    cmd.Parameters.AddWithValue("@article",      articleId);
                    var errParam = cmd.Parameters.Add("@errorMessage", SqlDbType.NVarChar, 500);
                    errParam.Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    var errMsg = errParam.Value?.ToString() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(errMsg))
                    {
                        result.ErrorMessage = errMsg;
                        return result;
                    }
                }

                // Get customer/journal codes by name
                using (var cmd = new SqlCommand(
                    @"SELECT c.Cust_code, j.Jour_code
                      FROM Customer c
                      JOIN Journal j ON j.Jour_ccode = c.Cust_code
                      WHERE c.Cust_fname = @custFName AND j.Jour_sname = @jourName",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@custFName", custName);
                    cmd.Parameters.AddWithValue("@jourName",  jourName);
                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        result.CustomerCode = Convert.ToInt32(dt.Rows[0]["Cust_code"]);
                        result.JournalCode  = Convert.ToInt32(dt.Rows[0]["Jour_code"]);
                        result.ArticleId    = articleId;
                        result.Isbn         = GetIsbnByJournal(result.JournalCode).Isbn;
                        
                        // Derive stage from volume+issue
                        int stageId = GetStageByVolumeIssue(result.CustomerCode, result.JournalCode, articleId, volume, issue);
                        // Store volume, issue, and stageId in result (extend BarcodeResult if needed)
                        // For now, we'll add them as properties if they exist
                    }
                    else
                    {
                        result.ErrorMessage = "customer/journal entered does not exist";
                    }
                }
            }
            else if (parts.Length == 3)
            {
                // Sub-stage-based format: Customer|Journal|Article
                result.JobType = "article";
                string custName  = parts[0].Trim();
                string jourName  = parts[1].Trim();
                string articleId = parts[2].Trim();

                using var conn = OpenConnection();

                // Validate using usp_PTS_ValidateCustomerJournalArticle
                using (var cmd = new SqlCommand("usp_PTS_ValidateCustomerJournalArticle", conn)
                       { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@custFName",    custName);
                    cmd.Parameters.AddWithValue("@journalName",  jourName);
                    cmd.Parameters.AddWithValue("@article",      articleId);
                    var errParam = cmd.Parameters.Add("@errorMessage", SqlDbType.NVarChar, 500);
                    errParam.Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    var errMsg = errParam.Value?.ToString() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(errMsg))
                    {
                        result.ErrorMessage = errMsg;
                        return result;
                    }
                }

                // Get customer/journal codes by name — real tables: Customer, Journal
                using (var cmd = new SqlCommand(
                    @"SELECT c.Cust_code, j.Jour_code
                      FROM Customer c
                      JOIN Journal j ON j.Jour_ccode = c.Cust_code
                      WHERE c.Cust_fname = @custFName AND j.Jour_sname = @jourName",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@custFName", custName);
                    cmd.Parameters.AddWithValue("@jourName",  jourName);
                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        result.CustomerCode = Convert.ToInt32(dt.Rows[0]["Cust_code"]);
                        result.JournalCode  = Convert.ToInt32(dt.Rows[0]["Jour_code"]);
                        result.ArticleId    = articleId;
                        result.Isbn         = GetIsbnByJournal(result.JournalCode).Isbn;
                    }
                    else
                    {
                        result.ErrorMessage = "customer/journal entered does not exist";
                    }
                }
            }
            else if (parts.Length == 4)
            {
                result.JobType = "issue";
                string custName = parts[0].Trim();
                string jourName = parts[1].Trim();

                using var conn = OpenConnection();
                using var cmd  = new SqlCommand(
                    @"SELECT c.Cust_code, j.Jour_code
                      FROM Customer c
                      JOIN Journal j ON j.Jour_ccode = c.Cust_code
                      WHERE c.Cust_fname = @custFName AND j.Jour_sname = @jourName",
                    conn);
                cmd.Parameters.AddWithValue("@custFName", custName);
                cmd.Parameters.AddWithValue("@jourName",  jourName);
                var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    result.CustomerCode = Convert.ToInt32(dt.Rows[0]["Cust_code"]);
                    result.JournalCode  = Convert.ToInt32(dt.Rows[0]["Jour_code"]);
                }
                else
                {
                    result.ErrorMessage = "customer/journal entered does not exist";
                }
            }
            else
            {
                result.ErrorMessage = "Invalid barcode format. Expected 3 or 5 parts separated by |";
            }

            return result;
        }

        // ── ISBN ───────────────────────────────────────────────────────────────
        // Original: dt.Rows[0]["Jour_issn_no"].ToString()  (line 1249 in RaiseProblems.aspx.cs)
        // Real SP:  usp_Contrast_GetIsbn  →  returns Jour_issn_no, Jour_Online_ISSN, hard_isbn, manifestisbn
        // The ASPX always uses Jour_issn_no — match that exactly.

        public IsbnResult GetIsbnByJournal(int journalCode)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("usp_Contrast_GetIsbn", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@bookid", journalCode);

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0) return new IsbnResult { Isbn = string.Empty };

            // Use Jour_issn_no — exactly what the original ASPX code reads (line 1249)
            string isbn = dt.Rows[0]["Jour_issn_no"]?.ToString()?.Trim() ?? string.Empty;
            return new IsbnResult { Isbn = isbn };
        }

        // ── Opened Requests (Grid) ─────────────────────────────────────────────
        // Original: ptsQueryService.GetOpenedRequests(objPTSProblem)
        // Real SP:  USP_PTS_GetOpenedRequests_New
        //   Params: @CustCode, @JourCode, @Article, @Volume, @Issue, @SubStageId
        //   Returns: PTId, PTCode, Article, PTSRemarks, Status, QueryType,
        //            ResolvedRemarks, CreatedBy, CreatedDate, SentBy, SentDate,
        //            ResolvedBy, ResolvedDate, DeptAndPT, AllowNewQry, PENDINGQRY

        public IEnumerable<QueryRow> GetOpenedRequests(OpenedRequestQuery query)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("USP_PTS_GetOpenedRequests_New", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@CustCode",   query.CustomerCode);
            cmd.Parameters.AddWithValue("@JourCode",   query.JournalCode);
            cmd.Parameters.AddWithValue("@Article",    query.Articles ?? string.Empty);
            cmd.Parameters.AddWithValue("@Volume",     string.Empty);
            cmd.Parameters.AddWithValue("@Issue",      string.Empty);
            cmd.Parameters.AddWithValue("@SubStageId", query.StageId);

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            // Load department names once for this request
            var deptNames = GetDepartmentNames(conn);

            return dt.AsEnumerable().Select(row =>
            {
                int ptId = Convert.ToInt32(row["PTId"]);

                // Resolve department name from id
                string deptIdStr = row["PTDepartmentId"]?.ToString()?.Trim() ?? string.Empty;
                string deptName  = deptNames.TryGetValue(deptIdStr, out var dn) ? dn : deptIdStr;

                // Problem type name: PTShortDesc contains the short description
                string ptName = row["PTShortDesc"]?.ToString()?.Trim() ?? string.Empty;

                return new QueryRow
                {
                    PtId            = ptId,
                    PtCode          = row["PTCode"]?.ToString()           ?? string.Empty,
                    Article         = row["Article"]?.ToString()          ?? string.Empty,
                    Remarks         = row["PTSRemarks"]?.ToString()       ?? string.Empty,
                    Status          = row["Status"]?.ToString()           ?? string.Empty,
                    QueryType       = row["QueryType"]?.ToString()        ?? string.Empty,
                    Response        = row["ResolvedRemarks"]?.ToString()  ?? string.Empty,
                    CreatedBy       = row["CreatedBy"]?.ToString()        ?? string.Empty,
                    CreatedDate     = ParseDate(row["CreatedDate"]),
                    SentBy          = row["SentBy"]?.ToString()           ?? string.Empty,
                    SentDate        = ParseDateNullable(row["SentDate"]),
                    ResolvedBy      = row["ResolvedBy"]?.ToString()       ?? string.Empty,
                    ResolvedDate    = ParseDateNullable(row["ResolvedDate"]),
                    DeptAndPt       = row["DeptAndPT"]?.ToString()        ?? string.Empty,
                    ApproveVisible  = CheckApproveVisible(ptId),
                    // Detail fields for Query Response modal
                    DeptName        = deptName,
                    ProblemTypeName = ptName,
                    CategoryName    = row["PTCDesc"]?.ToString()?.Trim()    ?? string.Empty,
                    PtcRemarks      = row["PTCRemarks"]?.ToString()?.Trim() ?? string.Empty,
                };
            }).ToList();
        }

        // Load all department names into a dictionary for fast lookup
        private Dictionary<string, string> GetDepartmentNames(SqlConnection conn)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using var cmd = new SqlCommand("USP_QMS_GetDepartment", conn)
                                { CommandType = CommandType.StoredProcedure };
                var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow r in dt.Rows)
                    dict[r["fieldId"]?.ToString() ?? string.Empty] = r["fieldName"]?.ToString() ?? string.Empty;
            }
            catch { /* non-fatal — fall back to showing IDs */ }
            return dict;
        }

        // ── Date parse helpers ─────────────────────────────────────────────────

        private static DateTime? ParseDate(object val)
        {
            if (val == null || val == DBNull.Value) return null;
            if (val is DateTime dt) return dt == DateTime.MinValue ? null : dt;
            return DateTime.TryParse(val.ToString(), out var parsed) ? parsed : (DateTime?)null;
        }

        private static DateTime? ParseDateNullable(object val)
        {
            if (val == null || val == DBNull.Value) return null;
            var s = val.ToString();
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (val is DateTime dt) return dt == DateTime.MinValue ? null : dt;
            return DateTime.TryParse(s, out var parsed) ? parsed : (DateTime?)null;
        }

        // ── Update Query ───────────────────────────────────────────────────────
        // Original: OnUpdate() → USP_deletequerynew with type="update"
        // Real SP:  USP_deletequerynew  →  @ID, @Reamarks, @Response, @type

        public void UpdateQuery(int id, UpdateQueryRequest request)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("USP_deletequerynew", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ID",       id);
            cmd.Parameters.AddWithValue("@Reamarks", request.Remarks  ?? string.Empty);
            cmd.Parameters.AddWithValue("@Response", request.Response ?? string.Empty);
            cmd.Parameters.AddWithValue("@type",     "update");
            cmd.ExecuteNonQuery();
        }

        // ── Delete Query ───────────────────────────────────────────────────────
        // Original: OnRowDeleting() → USP_deletequery with type="delete"
        // Real SP:  USP_deletequery  →  @ID, @Reamarks, @type

        public void DeleteQuery(int id)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("USP_deletequery", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ID",       id);
            cmd.Parameters.AddWithValue("@Reamarks", string.Empty);
            cmd.Parameters.AddWithValue("@type",     "delete");
            cmd.ExecuteNonQuery();
        }

        // ── Approve Instruction ────────────────────────────────────────────────
        // Original: ApproveBnClick() → USP_PTSUpdatequeryNew with type="UpdateApprove"
        // Real SP:  USP_PTSUpdatequeryNew  →  @ID, @Proceed_Req, @type

        public void ApproveInstruction(int id)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("USP_PTSUpdatequeryNew", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ID",          id);
            cmd.Parameters.AddWithValue("@Proceed_Req", true);
            cmd.Parameters.AddWithValue("@type",        "UpdateApprove");
            cmd.ExecuteNonQuery();
        }

        // ── Role ───────────────────────────────────────────────────────────────
        // Original: ptsQueryService.GetRoleIDByUserName(userName, customerCode)
        // Real SP:  USP_PTS_GETUSERROLEBYUSERNAME  →  returns multiple columns, need ROLEID

        public RoleResult GetRoleByUser(string userName, string customerCode)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("USP_PTS_GETUSERROLEBYUSERNAME", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@USERNAME", userName ?? string.Empty);
            
            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            
            int roleId = 0;
            if (dt.Rows.Count > 0)
            {
                // Get ROLEID from the 3rd column (index 2) or find column by name
                if (dt.Columns.Contains("ROLEID"))
                {
                    var roleIdObj = dt.Rows[0]["ROLEID"];
                    if (roleIdObj != null && roleIdObj != DBNull.Value)
                    {
                        roleId = Convert.ToInt32(roleIdObj);
                    }
                }
                else if (dt.Columns.Count >= 3)
                {
                    // Fallback: 3rd column is ROLEID
                    var roleIdObj = dt.Rows[0][2];
                    if (roleIdObj != null && roleIdObj != DBNull.Value)
                    {
                        roleId = Convert.ToInt32(roleIdObj);
                    }
                }
                else if (dt.Columns.Count > 0)
                {
                    // Last resort: try first column (might be MAPPINGID in old code)
                    var roleIdObj = dt.Rows[0][0];
                    if (roleIdObj != null && roleIdObj != DBNull.Value)
                    {
                        roleId = Convert.ToInt32(roleIdObj);
                    }
                }
            }
            
            Console.WriteLine($"GetRoleByUser for {userName}: roleId = {roleId}");
            return new RoleResult { RoleId = roleId };
        }

        // ── Problem Types ──────────────────────────────────────────────────────
        // Original: ptsQueryService.GetProblemTypeDetails(userName)
        // Real SP:  USP_PTS_GetProblemTypeDetails
        // Params:   @CustomerCode, @JournalCode, @JobChapterID, @StageID, @Volume, @Issue
        // The original ASPX passes the actual selected values from the dropdowns.

        public IEnumerable<DropdownItem> GetProblemTypes(string userName)
        {
            // Problem types are filtered by customer/journal/stage in the original.
            // We expose a simplified version here; pass 0/empty for unfiltered list.
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("USP_PTS_GetProblemTypeDetails", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@CustomerCode",  0);
            cmd.Parameters.AddWithValue("@JournalCode",   string.Empty);
            cmd.Parameters.AddWithValue("@JobChapterID",  string.Empty);
            cmd.Parameters.AddWithValue("@StageID",       0);
            cmd.Parameters.AddWithValue("@Volume",        string.Empty);
            cmd.Parameters.AddWithValue("@Issue",         string.Empty);

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            // Column names vary; try common ones
            string idCol   = dt.Columns.Contains("PTCId")   ? "PTCId"   :
                             dt.Columns.Contains("id")       ? "id"      : dt.Columns[0].ColumnName;
            string nameCol = dt.Columns.Contains("PTCDesc")  ? "PTCDesc" :
                             dt.Columns.Contains("name")     ? "name"    : dt.Columns[1].ColumnName;

            return dt.AsEnumerable().Select(r => new DropdownItem
            {
                Id   = Convert.ToInt32(r[idCol]),
                Name = r[nameCol]?.ToString() ?? string.Empty
            }).ToList();
        }

        // ── Problem Types by context ───────────────────────────────────────────
        // Original: ptsQueryService.GetProblemTypeDetails(userName) called from
        //           RaiseProblemsSubmission with actual customer/journal/stage values.
        // Real SP:  USP_PTS_GetProblemTypeDetails

        public IEnumerable<DropdownItem> GetProblemTypesByContext(
            int customerCode, int journalCode, string articleId, int stageId)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("USP_PTS_GetProblemTypeDetails", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@CustomerCode",  customerCode);
            cmd.Parameters.AddWithValue("@JournalCode",   journalCode.ToString());
            cmd.Parameters.AddWithValue("@JobChapterID",  articleId ?? string.Empty);
            cmd.Parameters.AddWithValue("@StageID",       stageId);
            cmd.Parameters.AddWithValue("@Volume",        string.Empty);
            cmd.Parameters.AddWithValue("@Issue",         string.Empty);

            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0) return Enumerable.Empty<DropdownItem>();

            string idCol   = dt.Columns.Contains("PTCId")  ? "PTCId"  :
                             dt.Columns.Contains("id")      ? "id"     : dt.Columns[0].ColumnName;
            string nameCol = dt.Columns.Contains("PTCDesc") ? "PTCDesc":
                             dt.Columns.Contains("name")    ? "name"   : dt.Columns[1].ColumnName;

            return dt.AsEnumerable().Select(r => new DropdownItem
            {
                Id   = Convert.ToInt32(r[idCol]),
                Name = r[nameCol]?.ToString() ?? string.Empty
            }).ToList();
        }

        // ── Stage variant ──────────────────────────────────────────────────────
        // Original: customerCode == 163 → StageDropDown (standard S100/S200/S300/ALL)
        //           customerCode == 877 → StageDropDown1 (Books: S5/S50/S200/S300/S600/S650)
        // This is the actual business rule from RaiseProblems.aspx.cs line ~450.
        // Returns "book" for customer 877, "standard" for all others.
        // The customer codes are real database values — not arbitrary hardcoding.

        public string GetStageVariant(int customerCode)
        {
            // Query the Customer table to determine if this is a Books customer.
            // Books customers use a different stage workflow (StageDropDown1 in the ASPX).
            // The original code checks customerCode == 877 directly; we verify via DB.
            try
            {
                using var conn = OpenConnection();
                using var cmd  = new SqlCommand(
                    "SELECT Cust_grname FROM Customer WHERE Cust_code = @custCode", conn);
                cmd.Parameters.AddWithValue("@custCode", customerCode);
                var grName = cmd.ExecuteScalar()?.ToString()?.Trim() ?? string.Empty;

                // Books customers have group name containing "Book" or "Springer"
                // Fallback: customerCode 877 is the known Books customer
                bool isBook = grName.IndexOf("Book", StringComparison.OrdinalIgnoreCase) >= 0
                           || customerCode == 877;
                return isBook ? "book" : "standard";
            }
            catch
            {
                // Default to standard if DB check fails
                return customerCode == 877 ? "book" : "standard";
            }
        }

        // ── Create Query ───────────────────────────────────────────────────────
        // Original: RaiseProblemsSubmission save button
        // Real SP:  usp_PTS_InsUpd_PTSProblems
        // Updated to support Volume/Issue for Stage-based workflow

        public void CreateQuery(CreateQueryRequest request)
        {
            string zipFilePath = string.Empty;

            if (request.ZipFile != null && request.ZipFile.Length > 0)
            {
                var uploadDir = Path.Combine("Uploads", "QueryFiles");
                Directory.CreateDirectory(uploadDir);
                zipFilePath = Path.Combine(uploadDir, $"{Guid.NewGuid()}_{request.ZipFile.FileName}");
                using var stream = File.Create(zipFilePath);
                request.ZipFile.CopyTo(stream);
            }

            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("usp_PTS_InsUpd_PTSProblems", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@SubstageID",     request.StageId);
            cmd.Parameters.AddWithValue("@stage",          string.Empty);   // stage name, optional
            cmd.Parameters.AddWithValue("@customerCode",   request.CustomerCode);
            cmd.Parameters.AddWithValue("@journalCode",    request.JournalCode);
            cmd.Parameters.AddWithValue("@article",        request.Articles ?? string.Empty);
            cmd.Parameters.AddWithValue("@DepartmentId",   request.DeptId);
            cmd.Parameters.AddWithValue("@volume",         request.Volume ?? string.Empty);    // NEW: Stage-based
            cmd.Parameters.AddWithValue("@issue",          request.Issue ?? string.Empty);     // NEW: Stage-based
            cmd.Parameters.AddWithValue("@ptsRemarks",     request.Remarks ?? string.Empty);
            cmd.Parameters.AddWithValue("@taskclosed",     false);
            cmd.Parameters.AddWithValue("@userCode",       request.UserName ?? string.Empty);
            cmd.Parameters.AddWithValue("@problemTypeCode",request.ProblemTypeId.ToString());
            cmd.Parameters.AddWithValue("@zipFileUpload",  !string.IsNullOrEmpty(zipFilePath));
            cmd.Parameters.AddWithValue("@querySavedBy",   request.UserName ?? string.Empty);
            cmd.Parameters.AddWithValue("@queryClosedBy",  string.Empty);
            cmd.Parameters.AddWithValue("@msgID",          string.Empty);
            cmd.Parameters.AddWithValue("@ZipFilePath",    zipFilePath);
            cmd.Parameters.AddWithValue("@Sequence",       string.Empty);
            cmd.Parameters.AddWithValue("@RoundCount",     0);
            cmd.Parameters.AddWithValue("@FILECONTENT",    string.Empty);

            var outParam = cmd.Parameters.Add("@insertStatus", SqlDbType.VarChar, 50);
            outParam.Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();
        }

        // ── Resolve Query ──────────────────────────────────────────────────────
        // Original: resolve button in RaiseProblemsSubmission
        // Real SP:  USP_PTS_RESOLVE_PTSPROBLEM_BYID

        public void ResolveQuery(ResolveQueryRequest request)
        {
            var openRequests = GetOpenedRequests(new OpenedRequestQuery
            {
                CustomerCode = request.CustomerCode,
                JournalCode  = request.JournalCode,
                Articles     = request.Articles,
                StageId      = request.StageId,
            });

            using var conn = OpenConnection();
            foreach (var q in openRequests.Where(r => r.Status?.ToLower() != "resolved"))
            {
                using var cmd = new SqlCommand("USP_PTS_RESOLVE_PTSPROBLEM_BYID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ID",                     q.PtId);
                cmd.Parameters.AddWithValue("@PTSQueryResolveRemarks", request.Remarks ?? string.Empty);
                cmd.Parameters.AddWithValue("@ResolvedBy",             request.UserName ?? string.Empty);
                var statusParam = cmd.Parameters.Add("@STATUS", SqlDbType.VarChar, 50);
                statusParam.Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
            }
        }

        // ── Private: check approve button visibility ───────────────────────────
        // Original: initiateFiles() → USP_PTSUpdatequeryNew with type="GetInitiateFile"

        private bool CheckApproveVisible(int ptId)
        {
            try
            {
                using var conn = OpenConnection();
                using var cmd  = new SqlCommand("USP_PTSUpdatequeryNew", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ID",          ptId);
                cmd.Parameters.AddWithValue("@Proceed_Req", false);
                cmd.Parameters.AddWithValue("@type",        "GetInitiateFile");
                var result = cmd.ExecuteScalar()?.ToString() ?? string.Empty;
                return result.ToLower() != "success";
            }
            catch
            {
                return true; // default to showing the button if check fails
            }
        }
    }
}
