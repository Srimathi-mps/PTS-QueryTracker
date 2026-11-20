namespace QueryTracker.Api.Models
{
    // ── Request / Response DTOs ────────────────────────────────────────────────

    public class BarcodeRequest
    {
        public string Barcode { get; set; } = string.Empty;
    }

    public class BarcodeResult
    {
        public int    CustomerCode  { get; set; }
        public int    JournalCode   { get; set; }
        public string ArticleId    { get; set; } = string.Empty;
        public string Isbn         { get; set; } = string.Empty;
        public string JobType      { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class OpenedRequestQuery
    {
        public int    CustomerCode { get; set; }
        public int    JournalCode  { get; set; }
        public string Articles    { get; set; } = string.Empty;  // comma-separated
        public int    StageId     { get; set; }
    }

    public class QueryRow
    {
        public int       PtId           { get; set; }
        public string    PtCode         { get; set; } = string.Empty;
        public string    Article        { get; set; } = string.Empty;
        public string    Remarks        { get; set; } = string.Empty;
        public string    Status         { get; set; } = string.Empty;
        public string    QueryType      { get; set; } = string.Empty;
        public string    Response       { get; set; } = string.Empty;
        public string    CreatedBy      { get; set; } = string.Empty;
        public DateTime? CreatedDate    { get; set; }
        public string    SentBy         { get; set; } = string.Empty;
        public DateTime? SentDate       { get; set; }
        public string    ResolvedBy     { get; set; } = string.Empty;
        public DateTime? ResolvedDate   { get; set; }
        public string    DeptAndPt      { get; set; } = string.Empty;
        public bool      ApproveVisible { get; set; }
        // ── Detail fields for Query Response modal ─────────────────────────
        public string    DeptName       { get; set; } = string.Empty;   // e.g. "JobAnalysis"
        public string    ProblemTypeName{ get; set; } = string.Empty;   // e.g. "100 -- PDF missing"
        public string    CategoryName   { get; set; } = string.Empty;   // e.g. "Category 1"
        public string    PtcRemarks     { get; set; } = string.Empty;   // PTCRemarks
    }

    public class UpdateQueryRequest
    {
        public string Remarks  { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
    }

    public class DropdownItem
    {
        public int    Id      { get; set; }
        public string Name    { get; set; } = string.Empty;
        public string? StrId  { get; set; }  // used for article codes (string keys)
    }

    public class IsbnResult
    {
        public string Isbn { get; set; } = string.Empty;
    }

    public class RoleResult
    {
        public int RoleId { get; set; }
    }

    public class CreateQueryRequest
    {
        public int          CustomerCode  { get; set; }
        public int          JournalCode   { get; set; }
        public string       Articles      { get; set; } = string.Empty;
        public string       Volume        { get; set; } = string.Empty;  // NEW: for Stage-based workflow
        public string       Issue         { get; set; } = string.Empty;   // NEW: for Stage-based workflow
        public int          StageId       { get; set; }
        public int          DeptId        { get; set; }
        public string       QueryType     { get; set; } = "QUERY";
        public int          ProblemTypeId { get; set; }
        public string       Remarks       { get; set; } = string.Empty;
        public string       UserName      { get; set; } = string.Empty;
        public IFormFile?   ZipFile       { get; set; }
    }

    public class ResolveQueryRequest
    {
        public int    CustomerCode { get; set; }
        public int    JournalCode  { get; set; }
        public string Articles     { get; set; } = string.Empty;
        public int    StageId      { get; set; }
        public int    DeptId       { get; set; }
        public string Remarks      { get; set; } = string.Empty;
        public string UserName     { get; set; } = string.Empty;
    }
}
