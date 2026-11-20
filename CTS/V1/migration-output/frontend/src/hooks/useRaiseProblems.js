import { useState, useEffect, useCallback } from 'react';
import {
  getCustomers,
  getJournals,
  getArticles,
  getDepartments,
  getIsbnByJournal,
  getOpenedRequests,
  lookupBarcode,
  getRoleByUser,
  getStageVariant,
  getStages,
} from '../services/raiseProblemsService';

/**
 * Central state hook for the RaiseProblems page.
 * Mirrors the session/ViewState logic from RaiseProblems.aspx.cs.
 */
export function useRaiseProblems(userName) {
  // ── Dropdown data ────────────────────────────────────────────────────────────
  const [customers, setCustomers] = useState([]);
  const [journals, setJournals] = useState([]);
  const [articles, setArticles] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [stages, setStages] = useState([]);

  // ── Selected values ──────────────────────────────────────────────────────────
  const [selectedCustomer, setSelectedCustomer] = useState('0');
  const [selectedJournal, setSelectedJournal] = useState('0');
  const [selectedArticles, setSelectedArticles] = useState([]); // multi-select
  const [selectedStage, setSelectedStage] = useState('0');
  const [selectedDept, setSelectedDept] = useState('0');
  const [isbn, setIsbn] = useState('');
  const [barcode, setBarcode] = useState('');

  // ── UI state ─────────────────────────────────────────────────────────────────
  const [showNewQueryBtn, setShowNewQueryBtn] = useState(false);
  const [showGoBtn, setShowGoBtn] = useState(false);
  const [roleId, setRoleId] = useState(0);
  const [message, setMessage] = useState({ type: '', text: '' });
  const [loading, setLoading] = useState(false);

  // ── Grid data ────────────────────────────────────────────────────────────────
  const [queries, setQueries] = useState([]);

  // ── Stage dropdown variant (customer 163 vs 877) ─────────────────────────────
  const [stageVariant, setStageVariant] = useState('standard'); // 'standard' | 'book'

  // ── Load customers on mount ──────────────────────────────────────────────────
  useEffect(() => {
    if (!userName) return;
    getCustomers(userName)
      .then((res) => setCustomers(Array.isArray(res.data) ? res.data : []))
      .catch(() => setMessage({ type: 'error', text: 'Failed to load customers' }));
  }, [userName]);

  // ── Customer change ──────────────────────────────────────────────────────────
  const handleCustomerChange = useCallback(
    async (custCode) => {
      setSelectedCustomer(custCode);
      setSelectedJournal('0');
      setSelectedArticles([]);
      setSelectedStage('0');
      setSelectedDept('0');
      setIsbn('');
      setJournals([]);
      setArticles([]);
      setDepartments([]);
      setQueries([]);
      setShowNewQueryBtn(false);
      setShowGoBtn(false);
      setMessage({ type: '', text: '' });

      if (custCode === '0') return;

      try {
        // Determine which stage dropdown variant to show via API — no hardcoded customer codes.
        // Original ASPX: customerCode == 163 → standard, customerCode == 877 → book.
        // The API checks the Customer table and returns "standard" or "book".
        const variantRes = await getStageVariant(custCode);
        setStageVariant(variantRes.data.variant || 'standard');

        const role = await getRoleByUser(userName, custCode);
        setRoleId(role.data.roleId);

        const res = await getJournals(custCode);
        setJournals(Array.isArray(res.data) ? res.data : []);

        const stageRes = await getStages(custCode);
        // Map DB StageItem { StageId, StageName } to { value, label }
        const stagesData = Array.isArray(stageRes.data) ? stageRes.data : [];
        const mappedStages = stagesData.map((s) => ({
          value: String(s.stageId),
          label: s.stageName,
        }));
        setStages(mappedStages);
      } catch {
        setMessage({ type: 'error', text: 'Failed to load journals/stages' });
      }
    },
    [userName]
  );

  // ── Journal change ───────────────────────────────────────────────────────────
  const handleJournalChange = useCallback(
    async (jourCode) => {
      setSelectedJournal(jourCode);
      setSelectedArticles([]);
      setSelectedStage('0');
      setArticles([]);
      setIsbn('');
      setQueries([]);
      setShowNewQueryBtn(false);
      setShowGoBtn(false);

      if (jourCode === '0') return;

      try {
        const [artRes, isbnRes, deptRes] = await Promise.all([
          getArticles(selectedCustomer, jourCode),
          getIsbnByJournal(jourCode),
          getDepartments(),
        ]);
        // Articles use strId (article code string), not numeric id
        const artData = Array.isArray(artRes.data) ? artRes.data : [];
        setArticles(artData.map(a => ({ id: a.strId || a.name, name: a.name })));
        setIsbn(isbnRes.data.isbn || '');
        setDepartments(Array.isArray(deptRes.data) ? deptRes.data : []);
        setShowGoBtn(true);
      } catch {
        setMessage({ type: 'error', text: 'Failed to load journal details' });
      }
    },
    [selectedCustomer]
  );

  // ── Stage change → auto-refresh grid ────────────────────────────────────────
  const handleStageChange = useCallback(
    async (stageId) => {
      setSelectedStage(stageId);
      if (stageId === '0' || selectedArticles.length === 0) return;
      await fetchQueries(selectedCustomer, selectedJournal, selectedArticles, stageId);
    },
    [selectedCustomer, selectedJournal, selectedArticles]
  );

  // ── Go button ────────────────────────────────────────────────────────────────
  const handleGo = useCallback(async () => {
    if (!selectedCustomer || selectedCustomer === '0') {
      setMessage({ type: 'error', text: 'Please select Customer' });
      return;
    }
    if (!selectedJournal || selectedJournal === '0') {
      setMessage({ type: 'error', text: 'Please select Project' });
      return;
    }
    if (selectedArticles.length === 0) {
      setMessage({ type: 'error', text: 'Please select at least one Job' });
      return;
    }
    if (!selectedDept || selectedDept === '0') {
      setMessage({ type: 'error', text: 'Please select Department' });
      return;
    }
    await fetchQueries(selectedCustomer, selectedJournal, selectedArticles, selectedStage);
    setShowNewQueryBtn(true);
  }, [selectedCustomer, selectedJournal, selectedArticles, selectedDept, selectedStage]);

  // ── Barcode lookup ───────────────────────────────────────────────────────────
  const handleBarcodeGo = useCallback(async () => {
    if (!barcode.trim()) {
      setMessage({ type: 'error', text: 'Please enter barcode' });
      return;
    }
    setLoading(true);
    try {
      const res = await lookupBarcode(barcode.trim());
      const data = res.data;
      // Populate dropdowns from barcode result
      setSelectedCustomer(String(data.customerCode));
      const [jRes, aRes, dRes] = await Promise.all([
        getJournals(data.customerCode),
        getArticles(data.customerCode, data.journalCode),
        getDepartments(),
      ]);
      setJournals(Array.isArray(jRes.data) ? jRes.data : []);
      setSelectedJournal(String(data.journalCode));
      const artData2 = Array.isArray(aRes.data) ? aRes.data : [];
      setArticles(artData2.map(a => ({ id: a.strId || a.name, name: a.name })));
      setDepartments(Array.isArray(dRes.data) ? dRes.data : []);
      if (data.articleId) setSelectedArticles([data.articleId]);
      setIsbn(data.isbn || '');
      setShowNewQueryBtn(true);
      setMessage({ type: '', text: '' });
    } catch (err) {
      const msg = err.response?.data?.message || 'Invalid barcode';
      setMessage({ type: 'error', text: msg });
    } finally {
      setLoading(false);
    }
  }, [barcode]);

  // ── Internal: fetch grid data ────────────────────────────────────────────────
  const fetchQueries = useCallback(async (custCode, jourCode, arts, stageId) => {
    setLoading(true);
    try {
      const res = await getOpenedRequests({
        customerCode: custCode,
        journalCode: jourCode,
        articles: arts.join(','),
        stageId,
      });
      setQueries(Array.isArray(res.data) ? res.data : []);
    } catch {
      setMessage({ type: 'error', text: 'Failed to load queries' });
    } finally {
      setLoading(false);
    }
  }, []);

  return {
    // data
    customers, journals, articles, departments, stages, queries,
    // selections
    selectedCustomer, selectedJournal, selectedArticles, setSelectedArticles,
    selectedStage, selectedDept, setSelectedDept,
    isbn, barcode, setBarcode,
    // ui
    showNewQueryBtn, showGoBtn, stageVariant, roleId, message, loading,
    // handlers
    handleCustomerChange, handleJournalChange, handleStageChange,
    handleGo, handleBarcodeGo,
    // reset all filters
    handleReset: () => {
      setSelectedCustomer('0');
      setSelectedJournal('0');
      setSelectedArticles([]);
      setSelectedStage('0');
      setSelectedDept('0');
      setIsbn('');
      setBarcode('');
      setJournals([]);
      setArticles([]);
      setDepartments([]);
      setStages([]);
      setQueries([]);
      setShowNewQueryBtn(false);
      setShowGoBtn(false);
      setMessage({ type: '', text: '' });
      setStageVariant('standard');
    },
    // grid refresh helper (used by child components after update/delete)
    refreshQueries: () =>
      fetchQueries(selectedCustomer, selectedJournal, selectedArticles, selectedStage),
  };
}
