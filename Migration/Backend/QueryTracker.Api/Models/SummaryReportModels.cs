namespace QueryTracker.Api.Models
{
    public class StageItem
    {
        public int    StageId             { get; set; }
        public string StageName           { get; set; } = string.Empty;
        public string StageDescription    { get; set; } = string.Empty;
    }

    public class SummaryReportRow
    {
        public int    StageId             { get; set; }
        public string StageName           { get; set; } = string.Empty;
        public string StageDescription    { get; set; } = string.Empty;
        /// <summary>Instructions awaiting reply (usp_PTS_Instruction_GetAwaitingReplyRecordCountByCustomerNew)</summary>
        public int    Instruction         { get; set; }
        /// <summary>Queries pending / awaiting reply (usp_PTS_GetAwaitingReplyRecordCountByCustomerNew)</summary>
        public int    QueriesPending      { get; set; }
        /// <summary>Queries resolved in last 30 days (usp_PTS_GetPTSResolvedRecordCountByCustomerNew)</summary>
        public int    QueriesResolved     { get; set; }
    }
}
