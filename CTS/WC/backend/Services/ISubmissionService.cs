using QueryTracker.Api.Models;

namespace QueryTracker.Api.Services
{
    public interface ISubmissionService
    {
        IEnumerable<ProblemTypeItem>     GetProblemTypes(int customerCode, int deptId, int stageId);
        IEnumerable<ProblemCategoryItem> GetProblemCategories(string problemTypeCode, int customerCode, int deptId, int stageId);
        bool   CheckIsPM(string userName);
        string RaiseQuery(RaiseQueryRequest request);
        string AddInstruction(AddInstructionRequest request);
        string ResolveQueryById(ResolveByIdRequest request);
    }
}
