namespace QueryTracker.Api.Models
{
    public class ProblemTypeItem
    {
        public string Id   { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class ProblemCategoryItem
    {
        public string Id   { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>New Query form — mirrors CloseButton_Click (Raise Query)</summary>
    public class RaiseQueryRequest
    {
        public int    CustomerCode      { get; set; }
        public int    JournalCode       { get; set; }
        public string Articles          { get; set; } = string.Empty;
        public int    StageId           { get; set; }
        public int    DeptId            { get; set; }
        public string ProblemTypeCode   { get; set; } = string.Empty;
        public string ProblemCategoryId { get; set; } = string.Empty;
        public string Remarks           { get; set; } = string.Empty;
        public string UserName          { get; set; } = string.Empty;
        public string ProceedWithQuery  { get; set; } = "YES";
        public IFormFile? ZipFile       { get; set; }
    }

    /// <summary>New Instruction form — mirrors CloseButton_Click (Add instruction)</summary>
    public class AddInstructionRequest
    {
        public int    CustomerCode  { get; set; }
        public int    JournalCode   { get; set; }
        public string Articles      { get; set; } = string.Empty;
        public int    StageId       { get; set; }
        public int    DeptId        { get; set; }
        public string Instructions  { get; set; } = string.Empty;
        public string UserName      { get; set; } = string.Empty;
        public string ProceedWith   { get; set; } = "YES";
        public IFormFile? ZipFile   { get; set; }
    }

    /// <summary>Resolve an existing query by its PTId</summary>
    public class ResolveByIdRequest
    {
        public int         PtId           { get; set; }
        public string      ResolveRemarks { get; set; } = string.Empty;
        public string      UserName       { get; set; } = string.Empty;
        public string      ProceedWith    { get; set; } = "YES";
        public IFormFile?  ZipFile        { get; set; }
        // Context for building the UNC upload path (mirrors Session values in ASPX)
        public int         CustomerCode   { get; set; }
        public string      CustomerName   { get; set; } = string.Empty;  // e.g. "48-ELS-CHE"
        public string      JournalName    { get; set; } = string.Empty;  // e.g. "ISS-ST-Test1-Nguyen-1634962"
        public string      ArticleCode    { get; set; } = string.Empty;  // e.g. "BOOK"
        public string      StageName      { get; set; } = string.Empty;  // e.g. "S100"
    }
}
