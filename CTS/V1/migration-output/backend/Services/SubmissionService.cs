using System.Data;
using Microsoft.Data.SqlClient;
using QueryTracker.Api.Models;

namespace QueryTracker.Api.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly string _connStr;

        public SubmissionService(IConfiguration config)
        {
            _connStr = config.GetConnectionString("jetsConstrNew")!;
        }

        private SqlConnection OpenConnection()
        {
            var conn = new SqlConnection(_connStr);
            conn.Open();
            return conn;
        }

        // Problem Types: usp_PTS_GetProblemTypesByStage
        public IEnumerable<ProblemTypeItem> GetProblemTypes(int customerCode, int deptId, int stageId)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("usp_PTS_GetProblemTypesByStage", conn)
                             { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@CustomerID", customerCode);
            cmd.Parameters.AddWithValue("@DEPTID",     deptId);
            cmd.Parameters.AddWithValue("@StageID",    stageId);
            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                using var fallbackCmd = new SqlCommand(@"
                    SELECT ProblemTypeID AS fieldID, RTRIM(LTRIM(Code)) + ' -- ' + RTRIM(LTRIM(short_desc)) AS fieldName
                    FROM LKP_PTS_ProblemType
                    WHERE Cust_Code = @CustomerID AND SubStageId = @StageID AND IsActive = 1
                    ORDER BY Code", conn);
                fallbackCmd.Parameters.AddWithValue("@CustomerID", customerCode);
                fallbackCmd.Parameters.AddWithValue("@StageID",    stageId);
                var fallbackDa = new SqlDataAdapter(fallbackCmd);
                dt.Clear();
                fallbackDa.Fill(dt);
            }

            return dt.AsEnumerable().Select(r => new ProblemTypeItem
            {
                Id   = r["fieldID"]?.ToString()   ?? string.Empty,
                Name = r["fieldName"]?.ToString() ?? string.Empty,
            }).ToList();
        }

        // Problem Categories: usp_PTS_GetProblemTypeCategoryByProblemTypeNew
        public IEnumerable<ProblemCategoryItem> GetProblemCategories(
            string problemTypeCode, int customerCode, int deptId, int stageId)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("usp_PTS_GetProblemTypeCategoryByProblemTypeNew", conn)
                             { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Code",          problemTypeCode);
            cmd.Parameters.AddWithValue("@customer_code", customerCode);
            cmd.Parameters.AddWithValue("@departmenId",   deptId);
            cmd.Parameters.AddWithValue("@SUBSTAGEID",    stageId);
            var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0 || dt.Columns.Count == 0)
            {
                using var fallbackCmd = new SqlCommand(@"
                    SELECT RTRIM(LTRIM(PC.ProblemTypeCategoryID)) AS fieldID, RTRIM(LTRIM(PC.Description)) AS fieldName
                    FROM LKP_PTS_ProblemTypeCategory PC
                    INNER JOIN LKP_PTS_ProblemType P ON PC.ProblemTypeId = P.ProblemTypeID AND P.Code = @Code
                    WHERE P.Cust_Code = @customer_code AND P.SubStageId = @SUBSTAGEID AND P.IsActive = 1", conn);
                fallbackCmd.Parameters.AddWithValue("@Code",          problemTypeCode);
                fallbackCmd.Parameters.AddWithValue("@customer_code", customerCode);
                fallbackCmd.Parameters.AddWithValue("@SUBSTAGEID",    stageId);
                var fallbackDa = new SqlDataAdapter(fallbackCmd);
                dt.Clear();
                fallbackDa.Fill(dt);
            }

            if (dt.Columns.Count == 0) return Enumerable.Empty<ProblemCategoryItem>();
            string idCol   = dt.Columns.Contains("fieldID")   ? "fieldID"   :
                             dt.Columns.Contains("id")         ? "id"        : dt.Columns[0].ColumnName;
            string nameCol = dt.Columns.Contains("fieldName")  ? "fieldName" :
                             dt.Columns.Contains("PTCategory") ? "PTCategory": dt.Columns[dt.Columns.Count > 1 ? 1 : 0].ColumnName;
            return dt.AsEnumerable().Select(r => new ProblemCategoryItem
            {
                Id   = r[idCol]?.ToString()   ?? string.Empty,
                Name = r[nameCol]?.ToString() ?? string.Empty,
            }).ToList();
        }

        // Check PM: USP_PTS_CheckPM
        public bool CheckIsPM(string userName)
        {
            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("USP_PTS_CheckPM", conn)
                             { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@empCode", userName);
            var result = cmd.ExecuteScalar();
            return result?.ToString()?.Trim() == "1";
        }

        // ── Raise Query ────────────────────────────────────────────────────────────
        public (string Message, string? UploadPath) RaiseQuery(RaiseQueryRequest request)
        {
            string zipFilePath = string.Empty;
            string? backgroundUploadPath = null;
            if (request.ZipFiles != null && request.ZipFiles.Count > 0) {
                zipFilePath = SaveZipFiles(request.ZipFiles, request.CustomerName, request.JournalName, request.Articles, request.StageName, request.DeptName);
            } else if (!string.IsNullOrWhiteSpace(request.FileName)) {
                zipFilePath = GenerateZipFilePath(request.FileName, request.CustomerName, request.JournalName, request.Articles, request.StageName, request.DeptName);
                backgroundUploadPath = zipFilePath;
            }

            // The frontend sends ProblemTypeCode as the ProblemTypeID (e.g. "534").
            // The SP's @problemTypeCode and the XML <ProblemTypeCode> need the Code value
            // from LKP_PTS_ProblemType (e.g. "13"), NOT the ID.
            // Original ASPX extracted this from the dropdown display text: "13 -- MISC" → "13"
            string ptTypeId = request.ProblemTypeCode ?? string.Empty; // e.g. "534" (the ID)
            string ptCode   = LookupProblemTypeCode(ptTypeId);         // e.g. "13"  (the Code)

            string xml = BuildQueryXml(
                ptTypeId:   ptTypeId,
                ptCode:     ptCode,
                categoryId: request.ProblemCategoryId ?? string.Empty,
                remarks:    request.Remarks ?? string.Empty,
                deptId:     request.DeptId);

            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("usp_PTS_InsertOrUpdate_PTSProblems_withQueryOne", conn)
                             { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@SubstageID",       request.StageId);
            cmd.Parameters.AddWithValue("@stage",            string.Empty);
            cmd.Parameters.AddWithValue("@customerCode",     request.CustomerCode);
            cmd.Parameters.AddWithValue("@journalCode",      request.JournalCode);
            cmd.Parameters.AddWithValue("@article",          request.Articles ?? string.Empty);
            cmd.Parameters.AddWithValue("@DepartmentId",     request.DeptId);
            cmd.Parameters.AddWithValue("@ptsRemarks",       request.Remarks ?? string.Empty);
            cmd.Parameters.AddWithValue("@taskclosed",       false);   // SaveButton = false
            cmd.Parameters.AddWithValue("@userCode",         request.UserName ?? string.Empty);
            cmd.Parameters.AddWithValue("@problemTypeCode",  ptCode);  // numeric code only
            cmd.Parameters.AddWithValue("@zipFileUpload",    !string.IsNullOrEmpty(zipFilePath));
            cmd.Parameters.AddWithValue("@querySavedBy",     request.UserName ?? string.Empty);
            cmd.Parameters.AddWithValue("@queryClosedBy",    string.Empty);
            cmd.Parameters.AddWithValue("@msgID",            string.Empty);
            cmd.Parameters.AddWithValue("@ZipFilePath",      zipFilePath);
            cmd.Parameters.AddWithValue("@Sequence",         string.Empty);
            cmd.Parameters.AddWithValue("@RoundCount",       0);
            cmd.Parameters.AddWithValue("@FILECONTENT",      xml);
            cmd.Parameters.AddWithValue("@QueryType",        "QUERY");
            cmd.Parameters.AddWithValue("@intprocessidlist", string.Empty);
            cmd.Parameters.AddWithValue("@Proceed_Req",      request.ProceedWithQuery?.ToUpper() == "YES");
            var outParam = cmd.Parameters.Add("@insertStatus", SqlDbType.VarChar, 500);
            outParam.Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();

            // SP returns "Insertion successful.|<newId>" — strip the ID part, keep only the message
            string status = outParam.Value?.ToString() ?? string.Empty;
            string message = status.Contains('|') ? status.Split('|')[0].Trim() : status.Trim();
            return (string.IsNullOrEmpty(message) ? "Task Raised successfully" : message, backgroundUploadPath);
        }

        // ── Add Instruction ─────────────────────────────────────────────────────
        // Mirrors: SaveButton_Click with queryType == "INSTRUCTION"
        // SP: usp_PTS_InsertOrUpdate_PTSProblems_withQueryOne
        //
        // Key original logic (from SaveButton_Click INSTRUCTION branch):
        //   ptsProblem.PtsRemarks  = gtxtPTRemarks.Text  (raw instruction text)
        //   ptsProblem.FileContent = hardcoded XML with ProblemTypeId=1, ProblemTypeCode=1,
        //                            ProblemCategoryId=1, DepartmentID=<deptId>
        //   ptsProblem.QueryType   = "INSTRUCTION"
        //   ptsProblem.TaskClosed  = false
        //   @problemTypeCode       = "" (empty for instructions)
        public (string Message, string? UploadPath) AddInstruction(AddInstructionRequest request)
        {
            string zipFilePath = string.Empty;
            string? backgroundUploadPath = null;
            if (request.ZipFiles != null && request.ZipFiles.Count > 0) {
                zipFilePath = SaveZipFiles(request.ZipFiles, request.CustomerName, request.JournalName, request.Articles, request.StageName, request.DeptName);
            } else if (!string.IsNullOrWhiteSpace(request.FileName)) {
                zipFilePath = GenerateZipFilePath(request.FileName, request.CustomerName, request.JournalName, request.Articles, request.StageName, request.DeptName);
                backgroundUploadPath = zipFilePath;
            }

            // INSTRUCTION XML — original always uses hardcoded 1/1/1 for type/code/category
            string xml = BuildInstructionXml(
                remarks: request.Instructions ?? string.Empty,
                deptId:  request.DeptId);

            using var conn = OpenConnection();
            using var cmd  = new SqlCommand("usp_PTS_InsertOrUpdate_PTSProblems_withQueryOne", conn)
                             { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@SubstageID",       request.StageId);
            cmd.Parameters.AddWithValue("@stage",            string.Empty);
            cmd.Parameters.AddWithValue("@customerCode",     request.CustomerCode);
            cmd.Parameters.AddWithValue("@journalCode",      request.JournalCode);
            cmd.Parameters.AddWithValue("@article",          request.Articles ?? string.Empty);
            cmd.Parameters.AddWithValue("@DepartmentId",     request.DeptId);
            cmd.Parameters.AddWithValue("@ptsRemarks",       request.Instructions ?? string.Empty);
            cmd.Parameters.AddWithValue("@taskclosed",       false);
            cmd.Parameters.AddWithValue("@userCode",         request.UserName ?? string.Empty);
            cmd.Parameters.AddWithValue("@problemTypeCode",  string.Empty);  // empty for INSTRUCTION
            cmd.Parameters.AddWithValue("@zipFileUpload",    !string.IsNullOrEmpty(zipFilePath));
            cmd.Parameters.AddWithValue("@querySavedBy",     request.UserName ?? string.Empty);
            cmd.Parameters.AddWithValue("@queryClosedBy",    string.Empty);
            cmd.Parameters.AddWithValue("@msgID",            string.Empty);
            cmd.Parameters.AddWithValue("@ZipFilePath",      zipFilePath);
            cmd.Parameters.AddWithValue("@Sequence",         string.Empty);
            cmd.Parameters.AddWithValue("@RoundCount",       0);
            cmd.Parameters.AddWithValue("@FILECONTENT",      xml);
            cmd.Parameters.AddWithValue("@QueryType",        "INSTRUCTION");
            cmd.Parameters.AddWithValue("@intprocessidlist", string.Empty);
            cmd.Parameters.AddWithValue("@Proceed_Req",      request.ProceedWith?.ToUpper() == "YES");
            var outParam = cmd.Parameters.Add("@insertStatus", SqlDbType.VarChar, 500);
            outParam.Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();

            // SP returns "Insertion successful.|<newId>" — strip the ID part, keep only the message
            string status = outParam.Value?.ToString() ?? string.Empty;
            string message = status.Contains('|') ? status.Split('|')[0].Trim() : status.Trim();
            return (string.IsNullOrEmpty(message) ? "Insertion successful." : message, backgroundUploadPath);
        }

        // Resolve Query: USP_PTS_RESOLVE_PTSPROBLEM_BYID
        // File storage mirrors btnsubmit_Click in RaiseProblemsSubmission.aspx.cs:
        //   1. Get path template from USP_GetQueryResolvedUploadPath
        //   2. Replace <<Customer>>, <<Jouranl>>, <<Article>>, <<Stage>>, <<Datetime>> tokens
        //   3. Save uploaded file + QueryResponse.txt to that UNC path
        public (string Message, string? UploadPath) ResolveQueryById(ResolveByIdRequest request)
        {
            using var conn = OpenConnection();
            string? backgroundUploadPath = null;

            // ── Step 1: Get the configured upload path for this customer ──────
            string uploadPath = GetQueryResolvedUploadPath(conn, request.CustomerCode);

            // ── Step 2: Build the final directory path (mirrors configPath logic) ──
            if (!string.IsNullOrEmpty(uploadPath))
            {
                string dir = uploadPath
                    .Replace("<<Customer>>", request.CustomerName  ?? string.Empty)
                    .Replace("<<Jouranl>>",  request.JournalName   ?? string.Empty)  // note: typo in original
                    .Replace("<<Article>>",  request.ArticleCode   ?? string.Empty)
                    .Replace("<<Vol>>\\",    string.Empty)
                    .Replace("<<Iss>>\\",    string.Empty)
                    .Replace("<<Stage>>",    request.StageName     ?? string.Empty)
                    .Replace("<<Datetime>>", DateTime.Now.ToString("dd_MMM_yyyy_HHmmss"));

                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    // Save uploaded file (mirrors fileUpload.PostedFile.SaveAs)
                    string fileName = string.Empty;
                    if (request.ZipFiles != null && request.ZipFiles.Count > 0)
                    {
                        fileName = Path.GetFileName(request.ZipFiles[0].FileName);
                        string filePath = Path.Combine(dir, fileName);
                        if (request.ZipFiles.Count == 1 && Path.GetExtension(fileName).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            using var stream = File.Create(filePath);
                            request.ZipFiles[0].CopyTo(stream);
                        }
                        else
                        {
                            // Zip multiple files together
                            string firstFileName = Path.GetFileNameWithoutExtension(fileName);
                            string zipPath = Path.Combine(dir, $"{firstFileName}.zip");
                            using (var zipStream = File.Create(zipPath))
                            {
                                using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
                                {
                                    foreach (var file in request.ZipFiles)
                                    {
                                        if (file.Length == 0) continue;
                                        var entry = archive.CreateEntry(file.FileName, System.IO.Compression.CompressionLevel.Optimal);
                                        using (var entryStream = entry.Open())
                                        {
                                            file.CopyTo(entryStream);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(request.FileName))
                    {
                        fileName = Path.GetFileName(request.FileName);
                        backgroundUploadPath = Path.Combine(dir, fileName);
                    }

                    // Save remarks as QueryResponse.txt (mirrors StreamWriter in original)
                    if (!string.IsNullOrWhiteSpace(request.ResolveRemarks))
                    {
                        string txtPath = Path.Combine(dir, "QueryResponse.txt");
                        File.WriteAllText(txtPath, request.ResolveRemarks.Trim());
                    }
                }
                catch
                {
                    // Non-fatal — file save failure should not block the DB update
                }
            }

            // ── Step 3: Resolve the query in DB ──────────────────────────────
            using (var cmd = new SqlCommand("USP_PTS_RESOLVE_PTSPROBLEM_BYID", conn)
                             { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@ID",                     request.PtId);
                cmd.Parameters.AddWithValue("@PTSQueryResolveRemarks", request.ResolveRemarks ?? string.Empty);
                cmd.Parameters.AddWithValue("@ResolvedBy",             request.UserName ?? string.Empty);
                var statusParam = cmd.Parameters.Add("@STATUS", SqlDbType.VarChar, 50);
                statusParam.Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
            }

            // ── Step 4: Update Proceed_Req ────────────────────────────────────
            bool proceedReq = (request.ProceedWith ?? "YES").ToUpper() == "YES";
            using (var cmd2 = new SqlCommand("USP_PTSUpdatequeryNew", conn)
                              { CommandType = CommandType.StoredProcedure })
            {
                cmd2.Parameters.AddWithValue("@ID",          request.PtId);
                cmd2.Parameters.AddWithValue("@Proceed_Req", proceedReq);
                cmd2.Parameters.AddWithValue("@type",        "UpdateProceed");
                cmd2.ExecuteNonQuery();
            }

            return ("QUERY RESOLVED SUCCESSFULLY", backgroundUploadPath);
        }

        // Get the UNC path template from LKP_CustomerApplicationSettings
        // Mirrors: ptsQueryService.GetQueryResolvedUploadPath(CustCode, "QueryResolvedUploadPath")
        private static string GetQueryResolvedUploadPath(SqlConnection conn, int customerCode)
        {
            try
            {
                using var cmd = new SqlCommand("USP_GetQueryResolvedUploadPath", conn)
                                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@CustCode", customerCode);
                cmd.Parameters.AddWithValue("@Param",    "QueryResolvedUploadPath");
                var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0]["ApplicationValue"]?.ToString() ?? string.Empty : string.Empty;
            }
            catch { return string.Empty; }
        }

        private static string CleanPathName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Unknown";
            
            // Remove invalid characters for path
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name.Trim();
        }

        // Save uploaded file — stores under Uploads/QueryFiles/Customer/Project/Job/Stage/Department
        private static string SaveZipFiles(List<IFormFile> files, string customerName, string journalName, string jobName, string stageName, string deptName)
        {
            if (files == null || files.Count == 0) return string.Empty;
            var subDir = Path.Combine(
                CleanPathName(customerName),
                CleanPathName(journalName),
                CleanPathName(jobName),
                CleanPathName(stageName),
                CleanPathName(deptName)
            );
            var dir = Path.Combine("Uploads", "QueryFiles", subDir);
            Directory.CreateDirectory(dir);

            if (files.Count == 1 && Path.GetExtension(files[0].FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                var path = Path.Combine(dir, $"{Guid.NewGuid()}_{files[0].FileName}");
                using var stream = File.Create(path);
                files[0].CopyTo(stream);
                return path;
            }
            else
            {
                string firstFileName = Path.GetFileNameWithoutExtension(files[0].FileName);
                var path = Path.Combine(dir, $"{Guid.NewGuid()}_{firstFileName}.zip");
                using (var zipStream = File.Create(path))
                {
                    using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
                    {
                        foreach (var file in files)
                        {
                            if (file.Length == 0) continue;
                            var entry = archive.CreateEntry(file.FileName, System.IO.Compression.CompressionLevel.Optimal);
                            using (var entryStream = entry.Open())
                            {
                                file.CopyTo(entryStream);
                            }
                        }
                    }
                }
                return path;
            }
        }

        private static string GenerateZipFilePath(string fileName, string customerName, string journalName, string jobName, string stageName, string deptName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return string.Empty;
            var subDir = Path.Combine(
                CleanPathName(customerName),
                CleanPathName(journalName),
                CleanPathName(jobName),
                CleanPathName(stageName),
                CleanPathName(deptName)
            );
            var dir = Path.Combine("Uploads", "QueryFiles", subDir);
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}");
        }

        public void UploadBackgroundFiles(List<IFormFile> files, string targetPath)
        {
            if (files == null || files.Count == 0 || string.IsNullOrWhiteSpace(targetPath)) return;
            // Basic path traversal prevention
            if (targetPath.Contains("..")) throw new UnauthorizedAccessException("Invalid path");
            
            var dir = Path.GetDirectoryName(targetPath);
            if (dir != null && !Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            using (var zipStream = File.Create(targetPath))
            {
                using (var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        if (file.Length == 0) continue;
                        var entry = archive.CreateEntry(file.FileName, System.IO.Compression.CompressionLevel.Optimal);
                        using (var entryStream = entry.Open())
                        {
                            file.CopyTo(entryStream);
                        }
                    }
                }
            }
        }

        // ── XML Helpers ─────────────────────────────────────────────────────────────

        // Look up the Code value from LKP_PTS_ProblemType by ProblemTypeID.
        // Original ASPX extracted this from dropdown display text: "13 -- MISC" → "13"
        // We do it properly by querying the lookup table.
        private string LookupProblemTypeCode(string problemTypeId)
        {
            if (string.IsNullOrWhiteSpace(problemTypeId) || problemTypeId == "0")
                return string.Empty;
            try
            {
                using var conn = OpenConnection();
                using var cmd  = new SqlCommand(
                    "SELECT Code FROM LKP_PTS_ProblemType WHERE ProblemTypeID = @id", conn);
                cmd.Parameters.AddWithValue("@id", problemTypeId);
                var result = cmd.ExecuteScalar()?.ToString()?.Trim();
                return result ?? problemTypeId; // fallback to ID if not found
            }
            catch { return problemTypeId; }
        }

        // Build QUERY XML — mirrors the sb.Append loop in SaveButton_Click for QUERY type
        // <Details>
        //   <ProblemType>
        //     <ProblemTypeId>{id}</ProblemTypeId>
        //     <ProblemTypeCode>{numericCode}</ProblemTypeCode>
        //     <ProblemCategoryId>{catId}</ProblemCategoryId>
        //     <PTCategoryRemark>{remarks}</PTCategoryRemark>
        //     <DepartmentID>{deptId}</DepartmentID>
        //   </ProblemType>
        // </Details>
        private static string BuildQueryXml(
            string ptTypeId, string ptCode, string categoryId, string remarks, int deptId)
        {
            static string Esc(string s) =>
                s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

            int catId = int.TryParse(categoryId, out var c) ? c : 0;

            return "<Details>"
                 + "<ProblemType>"
                 + $"<ProblemTypeId>{Esc(ptTypeId)}</ProblemTypeId>"
                 + $"<ProblemTypeCode>{Esc(ptCode)}</ProblemTypeCode>"
                 + $"<ProblemCategoryId>{catId}</ProblemCategoryId>"
                 + $"<PTCategoryRemark>{Esc(remarks)}</PTCategoryRemark>"
                 + $"<DepartmentID>{deptId}</DepartmentID>"
                 + "</ProblemType>"
                 + "</Details>";
        }

        // Build INSTRUCTION XML — mirrors the hardcoded TT string in SaveButton_Click:
        //   string TT = "<Details><ProblemType>
        //     <ProblemTypeId>1</ProblemTypeId>
        //     <ProblemTypeCode>1</ProblemTypeCode>
        //     <ProblemCategoryId>1</ProblemCategoryId>
        //     <PTCategoryRemark>{remarks}</PTCategoryRemark>
        //     <DepartmentID>{deptId}</DepartmentID>
        //   </ProblemType></Details>"
        private static string BuildInstructionXml(string remarks, int deptId)
        {
            static string Esc(string s) =>
                s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

            return "<Details>"
                 + "<ProblemType>"
                 + "<ProblemTypeId>1</ProblemTypeId>"
                 + "<ProblemTypeCode>1</ProblemTypeCode>"
                 + "<ProblemCategoryId>1</ProblemCategoryId>"
                 + $"<PTCategoryRemark>{Esc(remarks)}</PTCategoryRemark>"
                 + $"<DepartmentID>{deptId}</DepartmentID>"
                 + "</ProblemType>"
                 + "</Details>";
        }
    }
}
