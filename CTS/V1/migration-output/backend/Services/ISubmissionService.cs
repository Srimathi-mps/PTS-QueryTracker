using QueryTracker.Api.Models;

namespace QueryTracker.Api.Services
{
    public interface ISubmissionService
    {
        IEnumerable<ProblemTypeItem>     GetProblemTypes(int customerCode, int deptId, int stageId);
        IEnumerable<ProblemCategoryItem> GetProblemCategories(string problemTypeCode, int customerCode, int deptId, int stageId);
        bool   CheckIsPM(string userName);
        (string Message, string? UploadPath) RaiseQuery(RaiseQueryRequest request);
        (string Message, string? UploadPath) AddInstruction(AddInstructionRequest request);
        (string Message, string? UploadPath) ResolveQueryById(ResolveByIdRequest request);
        void UploadBackgroundFiles(List<IFormFile> files, string targetPath);
    }
}
