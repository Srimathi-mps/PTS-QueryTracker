import axios from 'axios';

// ── Base URL: update port if dotnet run picks a different one ──────────────────
const API_BASE_URL = 'https://localhost:61496/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});

const USER_NAME = sessionStorage.getItem('userName') || 'MAC014393';

export const ProblemService = {

  // GET /api/raiseproblems/customers?userName=MAC014393
  getCustomers: async () => {
    const r = await api.get('/raiseproblems/customers', { params: { userName: USER_NAME } });
    return r.data; // [{ id, name }]
  },

  // GET /api/raiseproblems/journals?customerCode=163
  getJournals: async (customerCode: number) => {
    const r = await api.get('/raiseproblems/journals', { params: { customerCode } });
    return r.data; // [{ id, name }]
  },

  // GET /api/raiseproblems/articles?customerCode=163&journalCode=1
  getArticles: async (customerCode: number, journalCode: number) => {
    const r = await api.get('/raiseproblems/articles', { params: { customerCode, journalCode } });
    return r.data; // [{ id, name }]
  },

  // GET /api/raiseproblems/departments
  getDepartments: async () => {
    const r = await api.get('/raiseproblems/departments');
    return r.data; // [{ id, name }]
  },

  // GET /api/raiseproblems/isbn?journalCode=1
  getIsbn: async (journalCode: number) => {
    const r = await api.get('/raiseproblems/isbn', { params: { journalCode } });
    return r.data.isbn ?? '';
  },

  // GET /api/raiseproblems/queries?customerCode=163&journalCode=1&articles=ART001&stageId=1
  getProblemHistory: async (customerCode: number, journalCode: number, articles: string, stageId = 0) => {
    const r = await api.get('/raiseproblems/queries', { params: { customerCode, journalCode, articles, stageId } });
    return r.data; // QueryRow[]
  },

  // POST /api/raiseproblems/queries
  raiseProblem: async (data: any) => {
    const form = new FormData();
    Object.entries(data).forEach(([k, v]) => {
      if (v !== null && v !== undefined)
        form.append(k, v instanceof File ? v : String(v));
    });
    const r = await api.post('/raiseproblems/queries', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return r.data;
  },

  // PUT /api/raiseproblems/queries/{id}
  updateQuery: async (id: number, remarks: string, response: string) => {
    const r = await api.put(`/raiseproblems/queries/${id}`, { remarks, response });
    return r.data;
  },

  // DELETE /api/raiseproblems/queries/{id}
  deleteQuery: async (id: number) => {
    const r = await api.delete(`/raiseproblems/queries/${id}`);
    return r.data;
  },

  // PUT /api/raiseproblems/queries/{id}/approve
  approveInstruction: async (id: number) => {
    const r = await api.put(`/raiseproblems/queries/${id}/approve`);
    return r.data;
  },

  // GET /api/raiseproblems/problem-types
  getProblemTypes: async () => {
    const r = await api.get('/raiseproblems/problem-types', { params: { userName: USER_NAME } });
    return r.data;
  },

  // GET /api/raiseproblems/barcode?barcode=CUST|PROJ|ART
  lookupBarcode: async (barcode: string) => {
    const r = await api.get('/raiseproblems/barcode', { params: { barcode } });
    return r.data;
  },
};

export default api;
