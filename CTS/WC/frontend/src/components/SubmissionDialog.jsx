import React, { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  Box, Typography, IconButton, Button, CircularProgress,
  Table, TableHead, TableBody, TableRow, TableCell,
  Select, MenuItem, FormControl, TextField,
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import AttachFileIcon from '@mui/icons-material/AttachFile';
import api from '../utils/api';
import { notify } from '../utils/notify';

const API = '/api/submission';
const USER = sessionStorage.getItem('userName') || 'MAC014393';

/**
 * SubmissionDialog — New Query / New Instruction popup.
 * Uses MUI components throughout — inherits global theme (font, colours, table headers).
 */
export default function SubmissionDialog({
  open, onClose, onSuccess,
  queryType = 'QUERY',
  context = {},
}) {
  const isQuery = queryType === 'QUERY';
  const articleStr = Array.isArray(context.articles)
    ? context.articles.join(',')
    : (context.articles ?? '');

  const headerTitle = isQuery
    ? `Query Details —   ${context.customerName ?? ''}-${context.journalName ?? ''}-${articleStr}-${context.stageName ?? ''}`
    : `Instruction Details —   ${context.customerName ?? ''}-${context.journalName ?? ''}-${articleStr}-${context.stageName ?? ''}`;

  // ── Form state ──────────────────────────────────────────────────────────────
  const [deptName, setDeptName] = useState(context.deptName ?? '');
  const [problemTypes, setProblemTypes] = useState([]);
  const [selectedPType, setSelectedPType] = useState('0');
  const [categories, setCategories] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState('0');
  const [remarks, setRemarks] = useState('');
  const [proceedWith, setProceedWith] = useState('YES');
  const [zipFile, setZipFile] = useState(null);
  const [loading, setLoading] = useState(false);

  // Reset on open
  useEffect(() => {
    if (open) {
      setSelectedPType('0');
      setSelectedCategory('0');
      setRemarks('');
      setProceedWith('YES');
      setZipFile(null);
      setDeptName(context.deptName ?? '');
    }
  }, [open, context.deptName]);

  // Load problem types (QUERY only)
  useEffect(() => {
    if (!open || !isQuery || !context.deptId || context.deptId === '0') return;
    api.get(`${API}/problem-types`, {
      params: { customerCode: context.customerCode, deptId: context.deptId, stageId: context.stageId },
    }).then((r) => setProblemTypes(r.data)).catch(() => { });
  }, [open, isQuery, context.customerCode, context.deptId, context.stageId]);

  // Load categories
  useEffect(() => {
    if (!selectedPType || selectedPType === '0') { setCategories([]); return; }
    const pt = problemTypes.find((p) => p.id === selectedPType);
    const code = pt ? (pt.name.includes('--') ? pt.name.split('--')[0].trim() : pt.id) : selectedPType;
    api.get(`${API}/problem-categories`, {
      params: { problemTypeCode: code, customerCode: context.customerCode, deptId: context.deptId, stageId: context.stageId },
    }).then((r) => setCategories(r.data)).catch(() => setCategories([]));
  }, [selectedPType, context.customerCode, context.deptId, context.stageId, problemTypes]);

  const showProceed = isQuery ||
    (queryType === 'INSTRUCTION' &&
      (context.stageName?.toUpperCase() === 'S200' || context.stageName?.toUpperCase() === 'S300'));

  // ── Submit ──────────────────────────────────────────────────────────────────
  const handleSubmit = async () => {
    if (!remarks.trim()) { notify.error(`Please enter ${isQuery ? 'Remarks' : 'Instructions'}`); return; }
    setLoading(true);
    try {
      const fd = new FormData();
      fd.append('customerCode', context.customerCode ?? '');
      fd.append('journalCode', context.journalCode ?? '');
      fd.append('articles', articleStr);
      fd.append('stageId', context.stageId ?? '');
      fd.append('deptId', context.deptId ?? '');
      fd.append('userName', USER);
      if (zipFile) fd.append('zipFile', zipFile);

      let res;
      if (isQuery) {
        fd.append('remarks', remarks);
        fd.append('problemTypeCode', selectedPType !== '0' ? selectedPType : '');
        fd.append('problemCategoryId', selectedCategory !== '0' ? selectedCategory : '');
        fd.append('proceedWithQuery', proceedWith);
        res = await api.post(`${API}/raise-query`, fd, { headers: { 'Content-Type': 'multipart/form-data' } });
      } else {
        fd.append('instructions', remarks);
        fd.append('proceedWith', proceedWith);
        res = await api.post(`${API}/add-instruction`, fd, { headers: { 'Content-Type': 'multipart/form-data' } });
      }

      notify.success(res.data.message || (isQuery ? 'Task Raised successfully' : 'Insertion successful.'));
      onClose();
      onSuccess?.();
    } catch (err) {
      notify.error(err.response?.data?.message || 'Failed to save');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="md"
      fullWidth
      PaperProps={{ sx: { borderRadius: 1, boxShadow: '0 8px 32px rgba(0,0,0,0.18)' } }}
    >
      {/* ── Title — inherits MuiDialogTitle theme styles ── */}
      <DialogTitle sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <span>{headerTitle}</span>
        <IconButton size="small" onClick={onClose} sx={{ color: '#555', '&:hover': { bgcolor: '#c8dff5' } }}>
          <CloseIcon sx={{ fontSize: 18 }} />
        </IconButton>
      </DialogTitle>

      {/* ── Content ── */}
      <DialogContent sx={{ p: 2, bgcolor: '#f8fbff' }}>
        {/* Form table — inherits MuiTableCell head theme (blue header) */}
        <Table size="small" sx={{ mb: 1 }}>
          <TableHead>
            <TableRow>
              <TableCell sx={{ width: '18%' }}>Departments</TableCell>
              {isQuery && <TableCell sx={{ width: '22%' }}>Problem Type</TableCell>}
              {isQuery && <TableCell sx={{ width: '18%' }}>Problem Category</TableCell>}
              <TableCell>{isQuery ? 'Remarks' : 'Instructions'}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            <TableRow>
              {/* Department — read-only */}
              <TableCell sx={{ verticalAlign: 'top' }}>
                <Typography variant="body2" sx={{ color: '#1a3a5c', fontWeight: 500 }}>
                  {deptName || context.deptId}
                </Typography>
              </TableCell>

              {/* Problem Type */}
              {isQuery && (
                <TableCell sx={{ verticalAlign: 'top' }}>
                  <FormControl fullWidth size="small">
                    <Select
                      value={selectedPType}
                      onChange={(e) => { setSelectedPType(e.target.value); setSelectedCategory('0'); }}
                      displayEmpty
                    >
                      <MenuItem value="0"><em style={{ color: '#aaa' }}>---Select---</em></MenuItem>
                      {problemTypes.map((pt) => (
                        <MenuItem key={pt.id} value={pt.id}>{pt.name}</MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                </TableCell>
              )}

              {/* Problem Category */}
              {isQuery && (
                <TableCell sx={{ verticalAlign: 'top' }}>
                  <FormControl fullWidth size="small" disabled={categories.length === 0}>
                    <Select
                      value={selectedCategory}
                      onChange={(e) => setSelectedCategory(e.target.value)}
                      displayEmpty
                    >
                      <MenuItem value="0"><em style={{ color: '#aaa' }}>-Select-</em></MenuItem>
                      {categories.map((c) => (
                        <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                </TableCell>
              )}

              {/* Remarks / Instructions */}
              <TableCell sx={{ verticalAlign: 'top' }}>
                <TextField
                  multiline
                  minRows={4}
                  fullWidth
                  size="small"
                  value={remarks}
                  onChange={(e) => setRemarks(e.target.value)}
                  placeholder={isQuery ? 'Enter remarks…' : 'Enter instructions…'}
                />
              </TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </DialogContent>

      {/* ── Actions — inherits MuiDialogActions theme ── */}
      <DialogActions sx={{ justifyContent: 'flex-start', gap: 1, flexWrap: 'wrap' }}>

        {/* Attachment */}
        <Button
          component="label"
          variant="outlined"
          size="small"
          startIcon={<AttachFileIcon />}
          sx={{ borderColor: '#a8c4e0', color: '#1a3a5c', '&:hover': { bgcolor: '#dbeaf9' } }}
        >
          Attachment
          <input type="file" hidden onChange={(e) => setZipFile(e.target.files[0] || null)} />
        </Button>
        {zipFile && (
          <Typography variant="caption" sx={{ color: '#555', alignSelf: 'center' }}>
            {zipFile.name}
          </Typography>
        )}

        {/* Raise Query / Add instruction */}
        <Button
          variant="outlined"
          size="small"
          onClick={handleSubmit}
          disabled={loading}
          sx={{ borderColor: '#a8c4e0', color: '#1a3a5c', fontWeight: 700, '&:hover': { bgcolor: '#dbeaf9', borderColor: '#1565c0' } }}
        >
          {loading ? <CircularProgress size={14} /> : isQuery ? 'Raise Query' : 'Add Instruction'}
        </Button>

        {/* Cancel */}
        <Button
          variant="outlined"
          size="small"
          onClick={onClose}
          disabled={loading}
          sx={{ borderColor: '#a8c4e0', color: '#555', '&:hover': { bgcolor: '#f5f5f5' } }}
        >
          Cancel
        </Button>

        {/* Proceed with Query/Instruction */}
        {showProceed && (
          <Box display="flex" alignItems="center" gap={1} ml={0.5}>
            <Typography variant="body2" sx={{ fontWeight: 700, color: '#1a3a5c' }}>
              {isQuery ? 'Proceed with Query' : 'Proceed with Instruction'}
            </Typography>
            <FormControl size="small" sx={{ minWidth: 70 }}>
              <Select
                value={proceedWith}
                onChange={(e) => setProceedWith(e.target.value)}
              >
                <MenuItem value="YES">YES</MenuItem>
                <MenuItem value="NO">NO</MenuItem>
              </Select>
            </FormControl>
          </Box>
        )}
      </DialogActions>
    </Dialog>
  );
}
