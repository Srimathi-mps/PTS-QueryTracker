using Microsoft.AspNetCore.Mvc;
using QueryTracker.Api.Models;
using QueryTracker.Api.Services;

namespace QueryTracker.Api.Controllers
{
    /// <summary>
    /// Replaces RaiseProblemsSubmission.aspx.cs
    /// Handles New Query and New Instruction forms.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionController : ControllerBase
    {
        private readonly ISubmissionService _svc;

        public SubmissionController(ISubmissionService svc) => _svc = svc;

        // GET /api/submission/problem-types?customerCode=163&deptId=12&stageId=1
        [HttpGet("problem-types")]
        public IActionResult GetProblemTypes(
            [FromQuery] int customerCode,
            [FromQuery] int deptId,
            [FromQuery] int stageId)
        {
            var data = _svc.GetProblemTypes(customerCode, deptId, stageId);
            return Ok(data);
        }

        // GET /api/submission/problem-categories?problemTypeCode=100&customerCode=163&deptId=12&stageId=1
        [HttpGet("problem-categories")]
        public IActionResult GetProblemCategories(
            [FromQuery] string problemTypeCode,
            [FromQuery] int customerCode,
            [FromQuery] int deptId,
            [FromQuery] int stageId)
        {
            var data = _svc.GetProblemCategories(problemTypeCode ?? string.Empty, customerCode, deptId, stageId);
            return Ok(data);
        }

        // GET /api/submission/check-pm?userName=MAC014393
        [HttpGet("check-pm")]
        public IActionResult CheckPM([FromQuery] string userName)
        {
            var isPM = _svc.CheckIsPM(userName ?? string.Empty);
            return Ok(new { isPM });
        }

        // POST /api/submission/raise-query
        [HttpPost("raise-query")]
        public IActionResult RaiseQuery([FromForm] RaiseQueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Remarks))
                return BadRequest(new { message = "Remarks are required" });

            var result = _svc.RaiseQuery(request);
            return Ok(new { message = result.Message, uploadPath = result.UploadPath });
        }

        // POST /api/submission/add-instruction
        [HttpPost("add-instruction")]
        public IActionResult AddInstruction([FromForm] AddInstructionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Instructions))
                return BadRequest(new { message = "Instructions are required" });

            var result = _svc.AddInstruction(request);
            return Ok(new { message = result.Message, uploadPath = result.UploadPath });
        }

        // POST /api/submission/resolve-query
        [HttpPost("resolve-query")]
        public IActionResult ResolveQuery([FromForm] ResolveByIdRequest request)
        {
            var result = _svc.ResolveQueryById(request);
            return Ok(new { message = result.Message, uploadPath = result.UploadPath });
        }

        // POST /api/submission/upload-background
        [HttpPost("upload-background")]
        public IActionResult UploadBackground([FromForm] List<IFormFile> files, [FromForm] string targetPath)
        {
            try
            {
                _svc.UploadBackgroundFiles(files, targetPath);
                return Ok(new { message = "Upload successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
