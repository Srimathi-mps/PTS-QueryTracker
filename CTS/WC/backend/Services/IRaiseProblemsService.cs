using System.Collections.Generic;
using QueryTracker.Api.Models;

namespace QueryTracker.Api.Services
{
    public interface IRaiseProblemsService
    {
        // ── Dropdowns ──────────────────────────────────────────────────────────
        IEnumerable<DropdownItem> GetCustomers(string userName);
        IEnumerable<DropdownItem> GetJournals(int customerCode);
        IEnumerable<DropdownItem> GetArticles(int customerCode, int journalCode);
        IEnumerable<DropdownItem> GetDepartments();

        // ── Stage-based workflow: Volumes & Issues ─────────────────────────────
        IEnumerable<DropdownItem> GetVolumes(int customerCode, int journalCode, string articleId);
        IEnumerable<DropdownItem> GetIssues(int customerCode, int journalCode, string articleId, string volume);
        int GetStageByVolumeIssue(int customerCode, int journalCode, string articleId, string volume, string issue);

        // ── Barcode ────────────────────────────────────────────────────────────
        BarcodeResult LookupBarcode(string barcode);

        // ── ISBN ───────────────────────────────────────────────────────────────
        IsbnResult GetIsbnByJournal(int journalCode);

        // ── Queries ────────────────────────────────────────────────────────────
        IEnumerable<QueryRow> GetOpenedRequests(OpenedRequestQuery query);
        void UpdateQuery(int id, UpdateQueryRequest request);
        void DeleteQuery(int id);
        void ApproveInstruction(int id);

        // ── Role ───────────────────────────────────────────────────────────────
        RoleResult GetRoleByUser(string userName, string customerCode);

        // ── Problem types ──────────────────────────────────────────────────────
        IEnumerable<DropdownItem> GetProblemTypes(string userName);
        IEnumerable<DropdownItem> GetProblemTypesByContext(int customerCode, int journalCode, string articleId, int stageId);

        // ── Stage variant ──────────────────────────────────────────────────────
        string GetStageVariant(int customerCode);

        // ── Create / Resolve ───────────────────────────────────────────────────
        void CreateQuery(CreateQueryRequest request);
        void ResolveQuery(ResolveQueryRequest request);
    }
}
