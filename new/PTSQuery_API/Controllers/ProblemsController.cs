using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace PTSQuery_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProblemsController : ControllerBase
    {
        private readonly string _connectionString;

        public ProblemsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            using var connection = new SqlConnection(_connectionString);
            var customers = await connection.QueryAsync("usp_PTS_GetCustomer", commandType: CommandType.StoredProcedure);
            return Ok(customers);
        }

        [HttpGet("journals/{customerCode}")]
        public async Task<IActionResult> GetJournals(int customerCode)
        {
            using var connection = new SqlConnection(_connectionString);
            var journals = await connection.QueryAsync("usp_PTS_GetJournal", 
                new { CustomerCode = customerCode }, 
                commandType: CommandType.StoredProcedure);
            return Ok(journals);
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            using var connection = new SqlConnection(_connectionString);
            var departments = await connection.QueryAsync("usp_PTS_GetDepartment", commandType: CommandType.StoredProcedure);
            return Ok(departments);
        }

        [HttpGet("articles")]
        public async Task<IActionResult> GetArticles(int customerCode, int journalCode)
        {
            using var connection = new SqlConnection(_connectionString);
            var articles = await connection.QueryAsync("usp_PTS_GetArticle", 
                new { CustomerCode = customerCode, JournalCode = journalCode }, 
                commandType: CommandType.StoredProcedure);
            return Ok(articles);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetProblemHistory(int customerCode, int journalCode, string article)
        {
            using var connection = new SqlConnection(_connectionString);
            var history = await connection.QueryAsync("usp_PTS_GetProblemHistory", 
                new { CustomerCode = customerCode, JournalCode = journalCode, Article = article }, 
                commandType: CommandType.StoredProcedure);
            return Ok(history);
        }

        [HttpPost]
        public async Task<IActionResult> RaiseProblem([FromBody] ProblemRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync("usp_PTS_InsertOrUpdateProblem", 
                new { 
                    CustomerCode = request.CustomerCode,
                    JournalCode = request.JournalCode,
                    Article = request.Article,
                    Remarks = request.Remarks,
                    UserName = request.UserName ?? "MAC014393"
                }, 
                commandType: CommandType.StoredProcedure);
            
            return Ok(new { message = "Problem processed successfully", status = result });
        }
    }

    public class ProblemRequest
    {
        public int CustomerCode { get; set; }
        public int JournalCode { get; set; }
        public string Article { get; set; }
        public string Remarks { get; set; }
        public string UserName { get; set; }
    }
}
