import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import api from '../utils/api';
import { notify } from '../utils/notify';
import SearchableSelect from '../components/SearchableSelect';

const API = '/api/submission';
const USER = sessionStorage.getItem('userName') || 'MAC014393';

// ── tiny helpers ──────────────────────────────────────────────────────────────
const Label = ({ children, required }) => (
  <span style={{ fontSize: 13, fontWeight: 600, color: '#333' }}>
    {children}{required && <span style={{ color: 'red' }}> *</span>}
  </span>
);

const Btn = ({ children, onClick, disabled, variant = 'default', type = 'button' }) => {
  const base = {
    padding: '5px 14px', fontSize: 13, borderRadius: 3,
    cursor: disabled ? 'not-allowed' : 'pointer',
    border: '1px solid #aaa', fontFamily: 'inherit',
    opacity: disabled ? 0.6 : 1,
  };
  const styles = {
    default:  { ...base, background: '#f0f0f0', color: '#333' },
    primary:  { ...base, background: '#1976d2', color: '#fff', border: '1px solid #1565c0' },
    success:  { ...base, background: '#388e3c', color: '#fff', border: '1px solid #2e7d32' },
    danger:   { ...base, background: '#d32f2f', color: '#fff', border: '1px solid #c62828' },
    warning:  { ...base, background: '#f57c00', color: '#fff', border: '1px solid #e65100' },
  };
  return (
    <button type={type} style={styles[variant] || styles.default}
      onClick={onClick} disabled={disabled}>
      {children}
    </button>
  );
};

// ── Main component ────────────────────────────────────────────────────────────
export default function RaiseProblemsSubmission() {
  const { state } = useLocation();
  const navigate  = useNavigate();

  const {
    queryType    = 'QUERY',
    customerCode = 0,
    journalCode  = 0,
    articles     = [],
    stageId      = 0,
    deptId       = 0,
    deptName     = '',
    customerName = '',
    journalName  = '',
    stageName    = '',
    ptId         = null,   // set when editing an existing query
    departments: stateDepts = [],
  } = state || {};

  const articleStr = Array.isArray(articles) ? articles.join(',') : articles;
  const isQuery    = queryType === 'QUERY';

  // ── Form state ───────────────────────────────────────────────────────────────
  const [selectedDept,     setSelectedDept]     = useState(String(deptId));
  const [problemTypes,     setProblemTypes]     = useState([]);
  const [selectedPType,    setSelectedPType]    = useState('0');
  const [categories,       setCategories]       = useState([]);
  const [selectedCategory, setSelectedCategory] = useState('0');
  const [remarks,          setRemarks]          = useState('');
  const [proceedWith,      setProceedWith]      = useState('YES');
  const [zipFiles,         setZipFiles]         = useState([]);
  const [loading,          setLoading]          = useState(false);
  const [isPM,             setIsPM]             = useState(false);
  const [departments,      setDepartments]      = useState([]);

  // ── Header title — mirrors lblHeader.Text in ASPX ───────────────────────────
  const headerTitle = isQuery
    ? `Query details of   ${customerName}-${journalName}-${articleStr}-${stageName}`
    : `Communication details of   ${customerName}-${journalName}-${articleStr}-${stageName}`;

  // ── Load departments if not passed in navigation state ────────────────────────
  useEffect(() => {
    if (stateDepts && stateDepts.length > 0) {
      setDepartments(stateDepts);
    } else {
      api.get('/api/raiseproblems/departments')
        .then((r) => setDepartments(Array.isArray(r.data) ? r.data : []))
        .catch(() => notify.error('Failed to load departments'));
    }
  }, [stateDepts]);

  // ── Load problem types when dept changes ─────────────────────────────────────
  useEffect(() => {
    if (!selectedDept || selectedDept === '0') { setProblemTypes([]); return; }
    api.get(`${API}/problem-types`, {
      params: { customerCode, deptId: selectedDept, stageId },
    }).then((r) => setProblemTypes(Array.isArray(r.data) ? r.data : []))
      .catch(() => notify.error('Failed to load problem types'));
  }, [selectedDept, customerCode, stageId]);

  // ── Load categories when problem type changes ────────────────────────────────
  useEffect(() => {
    if (!selectedPType || selectedPType === '0') { setCategories([]); return; }
    // Extract code before "--" (e.g. "100 -- PDF missing" → "100")
    const pt = Array.isArray(problemTypes) ? problemTypes.find((p) => p.id === selectedPType) : undefined;
    const code = pt ? (pt.name.includes('--') ? pt.name.split('--')[0].trim() : pt.id) : selectedPType;
    api.get(`${API}/problem-categories`, {
      params: { problemTypeCode: code, customerCode, deptId: selectedDept, stageId },
    }).then((r) => setCategories(Array.isArray(r.data) ? r.data : []))
      .catch(() => setCategories([]));
  }, [selectedPType, selectedDept, customerCode, stageId, problemTypes]);

  // ── Check PM on mount ────────────────────────────────────────────────────────
  useEffect(() => {
    api.get(`${API}/check-pm`, { params: { userName: USER } })
      .then((r) => {
        setIsPM(r.data.isPM);
        if (r.data.isPM) setProceedWith('NO');
      })
      .catch(() => {});
  }, []);

  // ── Submit handlers ──────────────────────────────────────────────────────────
  const buildForm = () => {
    const fd = new FormData();
    fd.append('customerCode', customerCode);
    fd.append('customerName', customerName);
    fd.append('journalCode',  journalCode);
    fd.append('journalName',  journalName);
    fd.append('articles',     articleStr);
    fd.append('stageId',      stageId);
    fd.append('stageName',    stageName);
    fd.append('deptId',       selectedDept);
    const deptObj = departments?.find(d => String(d.id) === String(selectedDept));
    fd.append('deptName',     deptObj ? deptObj.name : deptName);
    fd.append('userName',     USER);
    if (zipFiles.length > 0) {
      fd.append('fileName', zipFiles[0].name);
      zipFiles.forEach((file) => {
        fd.append('zipFiles', file);
      });
    }
    return fd;
  };

  const triggerBackgroundUpload = (uploadPath) => {
    if (zipFiles.length === 0 || !uploadPath) return;
    notify.info(`Uploading attachments in the background... Please do not close the page.`);
    const bgFd = new FormData();
    zipFiles.forEach((file) => {
      bgFd.append('files', file);
    });
    bgFd.append('targetPath', uploadPath);
    
    api.post(`${API}/upload-background`, bgFd, { 
      headers: { 'Content-Type': 'multipart/form-data' },
      timeout: 0 
    })
    .then(() => notify.success(`Attachments finished uploading!`))
    .catch(() => notify.error(`Failed to upload attachments in the background.`));
  };

  const handleRaiseQuery = async () => {
    if (!remarks.trim()) { notify.error('Please enter Remarks'); return; }
    setLoading(true);
    try {
      const fd = buildForm();
      fd.append('remarks',           remarks);
      fd.append('problemTypeCode',   selectedPType !== '0' ? selectedPType : '');
      fd.append('problemCategoryId', selectedCategory !== '0' ? selectedCategory : '');
      fd.append('proceedWithQuery',  proceedWith);
      const res = await api.post(`${API}/raise-query`, fd, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      notify.success(res.data.message || 'Task Raised successfully');
      triggerBackgroundUpload(res.data.uploadPath);
      navigate('/raise-problems');
    } catch (err) {
      notify.error(err.response?.data?.message || 'Failed to raise query');
    } finally { setLoading(false); }
  };

  const handleAddInstruction = async () => {
    if (!remarks.trim()) { notify.error('Please enter Instructions'); return; }
    setLoading(true);
    try {
      const fd = buildForm();
      fd.append('instructions', remarks);
      fd.append('proceedWith',  proceedWith);
      const res = await api.post(`${API}/add-instruction`, fd, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      notify.success(res.data.message || 'Insertion successful.');
      triggerBackgroundUpload(res.data.uploadPath);
      navigate('/raise-problems');
    } catch (err) {
      notify.error(err.response?.data?.message || 'Failed to add instruction');
    } finally { setLoading(false); }
  };

  const handleQueryResponse = async () => {
    if (!ptId) { notify.error('No query selected'); return; }
    if (!remarks.trim()) { notify.error('Please enter response remarks'); return; }
    setLoading(true);
    try {
      const fd = new FormData();
      fd.append('ptId', ptId);
      fd.append('resolveRemarks', remarks);
      fd.append('userName', USER);
      fd.append('proceedWith', proceedWith);
      
      // Context fields required for building the UNC path
      fd.append('customerCode', customerCode);
      fd.append('customerName', customerName);
      fd.append('journalName', journalName);
      fd.append('articleCode', articleStr);
      fd.append('stageName', stageName);
      if (zipFiles.length > 0) {
        fd.append('fileName', zipFiles[0].name);
        zipFiles.forEach((file) => {
          fd.append('zipFiles', file);
        });
      }

      const res = await api.post(`${API}/resolve-query`, fd, { headers: { 'Content-Type': 'multipart/form-data' } });
      notify.success(res.data.message || 'QUERY RESOLVED SUCCESSFULLY');
      triggerBackgroundUpload(res.data.uploadPath);
      navigate('/raise-problems');
    } catch (err) {
      notify.error(err.response?.data?.message || 'Failed to resolve query');
    } finally { setLoading(false); }
  };

  // ── Show/hide "Proceed with" dropdown ────────────────────────────────────────
  // Mirrors: ProceedWithQuery_DropDownList1.Visible logic in ASPX
  const showProceed = isQuery ||
    (queryType === 'INSTRUCTION' &&
      (stageName?.toUpperCase() === 'S200' || stageName?.toUpperCase() === 'S300'));

  // ── Dept options ─────────────────────────────────────────────────────────────
  const deptOptions = departments.map((d) => ({ value: String(d.id), label: d.name }));

  // ── Table cell style ─────────────────────────────────────────────────────────
  const th = {
    background: '#e8e8e8', border: '1px solid #ccc',
    padding: '6px 10px', fontSize: 13, fontWeight: 700,
    textAlign: 'left', whiteSpace: 'nowrap',
  };
  const td = {
    border: '1px solid #ccc', padding: '8px 10px',
    verticalAlign: 'top', background: '#fff',
  };

  return (
    <div style={{ padding: 16, fontFamily: 'Arial, sans-serif', maxWidth: 1100, margin: '0 auto' }}>

      {/* ── Header box ─────────────────────────────────────────────────────── */}
      <div style={{
        border: '1px solid #aaa', padding: '6px 10px',
        background: '#f5f5f5', marginBottom: 12,
        fontSize: 13, fontWeight: 600, color: '#333',
      }}>
        {headerTitle}
      </div>

      {/* ── Form table ─────────────────────────────────────────────────────── */}
      <table style={{ width: '100%', borderCollapse: 'collapse', marginBottom: 12 }}>
        <thead>
          <tr>
            <th style={{ ...th, width: '18%' }}>Departments</th>
            {isQuery && <th style={{ ...th, width: '20%' }}>Problem Type</th>}
            {isQuery && <th style={{ ...th, width: '18%' }}>Problem Category</th>}
            <th style={th}>
              {isQuery ? 'Remarks' : 'Instructions'}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr>
            {/* Department */}
            <td style={td}>
              <SearchableSelect
                options={deptOptions}
                value={String(selectedDept)}
                onChange={setSelectedDept}
                placeholder="-Select-"
                header="Please select Department"
                disabled={deptOptions.length === 0}
              />
            </td>

            {/* Problem Type — only for QUERY */}
            {isQuery && (
              <td style={td}>
                <select
                  value={selectedPType}
                  onChange={(e) => { setSelectedPType(e.target.value); setSelectedCategory('0'); }}
                  style={{
                    width: '100%', height: 28, fontSize: 13,
                    border: '1px solid #aaa', borderRadius: 2, padding: '0 4px',
                  }}
                >
                  <option value="0">---Select---</option>
                  {problemTypes.map((pt) => (
                    <option key={pt.id} value={pt.id}>{pt.name}</option>
                  ))}
                </select>
              </td>
            )}

            {/* Problem Category — only for QUERY */}
            {isQuery && (
              <td style={td}>
                <select
                  value={selectedCategory}
                  onChange={(e) => setSelectedCategory(e.target.value)}
                  disabled={categories.length === 0}
                  style={{
                    width: '100%', height: 28, fontSize: 13,
                    border: '1px solid #aaa', borderRadius: 2, padding: '0 4px',
                  }}
                >
                  <option value="0">-Select-</option>
                  {categories.map((c) => (
                    <option key={c.id} value={c.id}>{c.name}</option>
                  ))}
                </select>
              </td>
            )}

            {/* Remarks / Instructions */}
            <td style={td}>
              <textarea
                value={remarks}
                onChange={(e) => setRemarks(e.target.value)}
                rows={4}
                style={{
                  width: '100%', fontSize: 13, border: '1px solid #aaa',
                  borderRadius: 2, padding: 4, resize: 'vertical',
                  boxSizing: 'border-box',
                }}
                placeholder={isQuery ? 'Enter remarks…' : 'Enter instructions…'}
              />
            </td>
          </tr>
        </tbody>
      </table>

      {/* ── Action buttons row ─────────────────────────────────────────────── */}
      <div style={{ display: 'flex', alignItems: 'center', gap: 6, flexWrap: 'wrap' }}>

        {/* Attachment */}
        <label style={{
          padding: '5px 14px', fontSize: 13, borderRadius: 3,
          border: '1px solid #aaa', background: '#f0f0f0',
          cursor: 'pointer', color: '#333',
        }}>
          Attachment
          <input
            type="file" multiple hidden
            onChange={(e) => setZipFiles((prev) => [...prev, ...Array.from(e.target.files)])}
          />
        </label>
        {zipFiles.map((file, idx) => (
          <span
            key={idx}
            style={{
              display: 'inline-flex',
              alignItems: 'center',
              gap: 4,
              fontSize: 12,
              color: '#1a3a5c',
              background: '#eff6ff',
              border: '1px solid #bae6fd',
              padding: '2px 8px',
              borderRadius: 3,
              marginLeft: 6,
            }}
          >
            {file.name}
            <span
              onClick={() => setZipFiles((prev) => prev.filter((_, i) => i !== idx))}
              style={{ color: '#d32f2f', cursor: 'pointer', fontWeight: 'bold', padding: '0 2px' }}
            >
              ×
            </span>
          </span>
        ))}

        {/* Raise Query / Add instruction */}
        {isQuery ? (
          <Btn variant="primary" onClick={handleRaiseQuery} disabled={loading}>
            Raise Query
          </Btn>
        ) : (
          <Btn variant="primary" onClick={handleAddInstruction} disabled={loading}>
            Add instruction
          </Btn>
        )}

        {/* Query Response — only for QUERY when editing existing */}
        {isQuery && ptId && (
          <Btn variant="success" onClick={handleQueryResponse} disabled={loading}>
            Query Response
          </Btn>
        )}

        {/* Cancel */}
        <Btn onClick={() => navigate('/raise-problems')} disabled={loading}>
          Cancel
        </Btn>

        {/* Proceed with Query/Instruction */}
        {showProceed && (
          <>
            <span style={{ fontSize: 13, fontWeight: 600, marginLeft: 4 }}>
              {isQuery ? 'Proceed with Query' : 'Proceed with Instruction'}
            </span>
            <select
              value={proceedWith}
              onChange={(e) => setProceedWith(e.target.value)}
              style={{
                height: 26, fontSize: 13, border: '1px solid #aaa',
                borderRadius: 2, padding: '0 4px',
              }}
            >
              <option value="YES">YES</option>
              <option value="NO">NO</option>
            </select>
          </>
        )}
      </div>

    </div>
  );
}
