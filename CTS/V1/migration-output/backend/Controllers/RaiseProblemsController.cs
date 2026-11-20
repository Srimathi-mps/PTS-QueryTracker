using Microsoft.AspNetCore.Mvc;
using QueryTracker.Api.Models;
using QueryTracker.Api.Services;

namespace QueryTracker.Api.Controllers
{
    /// <summary>
    /// REST API that replaces all server-side logic in RaiseProblems.aspx.cs.
    /// Route prefix: /api/raiseproblems
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RaiseProblemsController : ControllerBase
    {
        private readonly IRaiseProblemsService _svc;

        public RaiseProblemsController(IRaiseProblemsService svc)
        {
            _svc = svc;
        }

        // ── GET /api/raiseproblems/volumes?customerCode=163&journalCode=1&articleId=ART001 ──
        /// <summary>
        /// Returns volumes for a given article (Stage-based workflow).
        /// New endpoint for Stage-based version.
        /// </summary>
        [HttpGet("volumes")]
        public IActionResult GetVolumes([FromQuery] int customerCode, [FromQuery] int journalCode, [FromQuery] string articleId)
        {
            var data = _svc.GetVolumes(customerCode, journalCode, articleId);
            return Ok(data);
        }

        // ── GET /api/raiseproblems/issues?customerCode=163&journalCode=1&articleId=ART001&volume=Vol10 ──
        /// <summary>
        /// Returns issues for a given volume (Stage-based workflow).
        /// New endpoint for Stage-based version.
        /// </summary>
        [HttpGet("issues")]
        public IActionResult GetIssues([FromQuery] int customerCode, [FromQuery] int journalCode, [FromQuery] string articleId, [FromQuery] string volume)
        {
            var data = _svc.GetIssues(customerCode, journalCode, articleId, volume);
            return Ok(data);
        }

        // ── GET /api/raiseproblems/stage-by-volume-issue?customerCode=163&journalCode=1&articleId=ART001&volume=Vol10&issue=Issue3 ──
        /// <summary>
        /// Derives stage from volume and issue (Stage-based workflow).
        /// New endpoint for Stage-based version.
        /// </summary>
        [HttpGet("stage-by-volume-issue")]
        public IActionResult GetStageByVolumeIssue([FromQuery] int customerCode, [FromQuery] int journalCode, 
            [FromQuery] string articleId, [FromQuery] string volume, [FromQuery] string issue)
        {
            var stageId = _svc.GetStageByVolumeIssue(customerCode, journalCode, articleId, volume, issue);
            return Ok(new { stageId });
        }

        // ── GET /api/raiseproblems/stage-variant?customerCode=163 ───────────
        /// <summary>
        /// Returns which stage dropdown list to show for a given customer.
        /// Original logic: customerCode == 163 → standard, customerCode == 877 → book.
        /// Driven by the database: checks the customer's group name via USP_TTS_GETCUSTOMERS.
        /// Returns: { variant: "standard" | "book" }
        /// </summary>
        [HttpGet("stage-variant")]
        public IActionResult GetStageVariant([FromQuery] int customerCode)
        {
            var variant = _svc.GetStageVariant(customerCode);
            return Ok(new { variant });
        }

        // ── GET /api/raiseproblems/customers?userName=MAC014393 ────────────────
        /// <summary>Replaces FillCustomerDDL()</summary>
        [HttpGet("customers")]
        public IActionResult GetCustomers([FromQuery] string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest(new { message = "userName is required" });

            var data = _svc.GetCustomers(userName);
            return Ok(data);
        }

        // ── GET /api/raiseproblems/journals?customerCode=163 ──────────────────
        /// <summary>Replaces FillJournalDDL()</summary>
        [HttpGet("journals")]
        public IActionResult GetJournals([FromQuery] int customerCode)
        {
            var data = _svc.GetJournals(customerCode);
            return Ok(data);
        }

        // ── GET /api/raiseproblems/articles?customerCode=163&journalCode=42 ───
        /// <summary>Replaces FillArticleDDL()</summary>
        [HttpGet("articles")]
        public IActionResult GetArticles([FromQuery] int customerCode, [FromQuery] int journalCode)
        {
            var data = _svc.GetArticles(customerCode, journalCode);
            return Ok(data);
        }

        // ── GET /api/raiseproblems/departments ────────────────────────────────
        /// <summary>Replaces FillDeptDDL()</summary>
        [HttpGet("departments")]
        public IActionResult GetDepartments()
        {
            var data = _svc.GetDepartments();
            return Ok(data);
        }

        // ── GET /api/raiseproblems/barcode?barcode=CUST|PROJ|ART ─────────────
        /// <summary>Replaces btnGoBarcode_Click() + GetDataFromBarCode()</summary>
        [HttpGet("barcode")]
        public IActionResult LookupBarcode([FromQuery] string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return BadRequest(new { message = "barcode is required" });

            var result = _svc.LookupBarcode(barcode);

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result);
        }

        // ── GET /api/raiseproblems/isbn?journalCode=42 ────────────────────────
        /// <summary>Replaces usp_Contrast_GetIsbn call in JournalDropDown_SelectedIndexChanged</summary>
        [HttpGet("isbn")]
        public IActionResult GetIsbn([FromQuery] int journalCode)
        {
            var result = _svc.GetIsbnByJournal(journalCode);
            return Ok(result);
        }

        // ── GET /api/raiseproblems/queries?customerCode=163&journalCode=42&articles=ART1,ART2&stageId=1 ──
        /// <summary>Replaces BindOpenedRequests() + QueryBind()</summary>
        [HttpGet("queries")]
        public IActionResult GetOpenedRequests([FromQuery] OpenedRequestQuery query)
        {
            var data = _svc.GetOpenedRequests(query);
            return Ok(data);
        }

        // ── PUT /api/raiseproblems/queries/{id} ───────────────────────────────
        /// <summary>Replaces OnUpdate() – updates Remarks and Response</summary>
        [HttpPut("queries/{id:int}")]
        public IActionResult UpdateQuery(int id, [FromBody] UpdateQueryRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Request body is required" });

            _svc.UpdateQuery(id, request);
            return NoContent();
        }

        // ── DELETE /api/raiseproblems/queries/{id} ────────────────────────────
        /// <summary>Replaces OnRowDeleting()</summary>
        [HttpDelete("queries/{id:int}")]
        public IActionResult DeleteQuery(int id)
        {
            _svc.DeleteQuery(id);
            return NoContent();
        }

        // ── PUT /api/raiseproblems/queries/{id}/approve ───────────────────────
        /// <summary>Replaces ApproveBnClick() for INSTRUCTION type</summary>
        [HttpPut("queries/{id:int}/approve")]
        public IActionResult ApproveInstruction(int id)
        {
            _svc.ApproveInstruction(id);
            return NoContent();
        }

        // ── GET /api/raiseproblems/role?userName=MAC014393&customerCode=163 ───
        /// <summary>Replaces ptsQueryService.GetRoleIDByUserName()</summary>
        [HttpGet("role")]
        public IActionResult GetRole([FromQuery] string userName, [FromQuery] string customerCode)
        {
            var result = _svc.GetRoleByUser(userName, customerCode ?? "0");
            return Ok(result);
        }

        // ── GET /api/raiseproblems/problem-types?userName=MAC014393 ───────────
        /// <summary>Replaces ptsQueryService.GetProblemTypeDetails() — unfiltered list</summary>
        [HttpGet("problem-types")]
        public IActionResult GetProblemTypes([FromQuery] string userName)
        {
            var data = _svc.GetProblemTypes(userName ?? string.Empty);
            return Ok(data);
        }

        // ── GET /api/raiseproblems/problem-types-context?customerCode=163&journalCode=42&articleId=ART1&stageId=1 ──
        /// <summary>
        /// Replaces ptsQueryService.GetProblemTypeDetails() called from RaiseProblemsSubmission
        /// with actual context values — returns filtered problem types for the given selection.
        /// </summary>
        [HttpGet("problem-types-context")]
        public IActionResult GetProblemTypesByContext(
            [FromQuery] int customerCode,
            [FromQuery] int journalCode,
            [FromQuery] string? articleId,
            [FromQuery] int stageId)
        {
            var data = _svc.GetProblemTypesByContext(customerCode, journalCode, articleId ?? string.Empty, stageId);
            return Ok(data);
        }

        // ── POST /api/raiseproblems/queries ───────────────────────────────────
        /// <summary>Replaces btnRaiseProblem_Click / btnNew_Instruction_Click + RaiseProblemsSubmission save</summary>
        [HttpPost("queries")]
        public IActionResult CreateQuery([FromForm] CreateQueryRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Request body is required" });

            if (string.IsNullOrWhiteSpace(request.Remarks))
                return BadRequest(new { message = "Remarks are required" });

            _svc.CreateQuery(request);
            return Ok(new { message = request.QueryType == "QUERY" ? "Task Raised successfully" : "Insertion successful." });
        }

        // ── PUT /api/raiseproblems/queries/resolve ────────────────────────────
        /// <summary>Resolves an open query</summary>
        [HttpPut("queries/resolve")]
        public IActionResult ResolveQuery([FromBody] ResolveQueryRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Request body is required" });

            _svc.ResolveQuery(request);
            return Ok(new { message = "QUERY RESOLVED SUCCESSFULLY" });
        }

        // ── GET /api/raiseproblems/queries/{id}/download ──────────────────────
        /// <summary>Downloads the query attachment</summary>
        [HttpGet("queries/{id:int}/download")]
        public IActionResult DownloadAttachment(int id)
        {
            try
            {
                var (filePath, contentType, fileName) = _svc.GetAttachmentPath(id);
                var fullPath = Path.GetFullPath(filePath);
                return PhysicalFile(fullPath, contentType, fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
