using QueryTracker.Api.Models;

namespace QueryTracker.Api.Services
{
    public interface ISummaryReportService
    {
        IEnumerable<StageItem>       GetAllStages(int customerCode);
        IEnumerable<SummaryReportRow> GetSummaryRows(int customerCode);
    }
}
