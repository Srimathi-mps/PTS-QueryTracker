import api from '../utils/api';

const BASE_URL = '/api/raiseproblems';

// ── Dropdowns ──────────────────────────────────────────────────────────────────

export const getCustomers = (userName) =>
  api.get(`${BASE_URL}/customers`, { params: { userName } });

export const getJournals = (customerCode) =>
  api.get(`${BASE_URL}/journals`, { params: { customerCode } });

export const getArticles = (customerCode, journalCode) =>
  api.get(`${BASE_URL}/articles`, { params: { customerCode, journalCode } });

export const getDepartments = () =>
  api.get(`${BASE_URL}/departments`);

// ── Stage variant ──────────────────────────────────────────────────────────────
// Returns { variant: "standard" | "book" } for the given customer.
// Replaces the hardcoded customerCode === '877' check in the frontend.

export const getStageVariant = (customerCode) =>
  api.get(`${BASE_URL}/stage-variant`, { params: { customerCode } });

// ── Barcode ────────────────────────────────────────────────────────────────────

export const lookupBarcode = (barcode) =>
  api.get(`${BASE_URL}/barcode`, { params: { barcode } });

// ── ISBN ───────────────────────────────────────────────────────────────────────

export const getIsbnByJournal = (journalCode) =>
  api.get(`${BASE_URL}/isbn`, { params: { journalCode } });

// ── Queries (grid) ─────────────────────────────────────────────────────────────

export const getOpenedRequests = (params) =>
  // params: { customerCode, journalCode, articles, stageId }
  api.get(`${BASE_URL}/queries`, { params });

export const updateQuery = (id, payload) =>
  // payload: { remarks, response }
  api.put(`${BASE_URL}/queries/${id}`, payload);

export const deleteQuery = (id) =>
  api.delete(`${BASE_URL}/queries/${id}`);

export const approveInstruction = (id) =>
  api.put(`${BASE_URL}/queries/${id}/approve`);

// ── Role ───────────────────────────────────────────────────────────────────────

export const getRoleByUser = (userName, customerCode) =>
  api.get(`${BASE_URL}/role`, { params: { userName, customerCode } });

// ── Problem types (context-filtered) ──────────────────────────────────────────
// Used by RaiseProblemsSubmission to get problem types filtered by actual selection.

export const getProblemTypesByContext = (customerCode, journalCode, articleId, stageId) =>
  api.get(`${BASE_URL}/problem-types-context`, {
    params: { customerCode, journalCode, articleId, stageId },
  });

// ── Summary Report ─────────────────────────────────────────────────────────────
export const getStages = (customerCode) =>
  api.get('/api/summaryreport/stages', { params: { customerCode } });

export const getSummaryRows = (customerCode) =>
  api.get('/api/summaryreport/rows', { params: { customerCode } });
