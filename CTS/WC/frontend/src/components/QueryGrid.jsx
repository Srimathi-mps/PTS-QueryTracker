import React, { useState, useMemo } from 'react';
import {
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
  TablePagination, TableSortLabel, Paper, IconButton,
  TextField, Tooltip, Chip, Box, Typography, Button,
  Dialog, DialogTitle, DialogContent, DialogActions,
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import SaveIcon from '@mui/icons-material/Save';
import CancelIcon from '@mui/icons-material/Cancel';
import QuestionAnswerIcon from '@mui/icons-material/QuestionAnswer';
import AttachFileIcon from '@mui/icons-material/AttachFile';
import CloseIcon from '@mui/icons-material/Close';
import api from '../utils/api';
import { updateQuery, deleteQuery, approveInstruction } from '../services/raiseProblemsService';
import { notify } from '../utils/notify';

// ── Status colours — matches original ASPX gvwOpenedRequest_RowDataBound ──────
const STATUS_BG = {
  raised: '#FFA500',
  resolved: '#90EE90',
  saved: '#ffffff',
};
const CHIP_STYLE = {
  raised: { bgcolor: '#e65100', color: '#fff' },
  resolved: { bgcolor: '#2e7d32', color: '#fff' },
  saved: { bgcolor: '#757575', color: '#fff' },
};

const COLUMNS = [
  { id: 'ptCode', label: 'Query Code', width: '6%' },
  { id: 'article', label: 'Chapter / Job', width: '7%' },
  { id: 'remarks', label: 'Remarks', width: '28%' },
  { id: 'status', label: 'Status', width: '7%' },
  { id: 'queryType', label: 'Query Type', width: '7%' },
  { id: 'response', label: 'Resolution', width: '28%' },
  { id: 'createdDate', label: 'Date', width: '10%' },
  { id: 'actions', label: '', width: '10%', sortable: false },
];

const USER = sessionStorage.getItem('userName') || 'MAC014393';

// ── Info row helper for the modal header ──────────────────────────────────────
function InfoCell({ label, value }) {
  return (
    <>
      <TableCell sx={{ fontWeight: 700, fontSize: '0.78rem', color: '#555', bgcolor: '#f0f4f8', border: '1px solid #d0dcea', py: 0.75, px: 1.5, whiteSpace: 'nowrap' }}>
        {label}
      </TableCell>
      <TableCell sx={{ fontSize: '0.78rem', bgcolor: '#f0f4f8', border: '1px solid #d0dcea', py: 0.75, px: 1.5 }}>
        {value || ''}
      </TableCell>
    </>
  );
}

export default function QueryGrid({ rows = [], onRefresh, gridContext = {} }) {
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [orderBy, setOrderBy] = useState('createdDate');
  const [order, setOrder] = useState('desc');

  // ── Inline edit (remarks only) ────────────────────────────────────────────
  const [editRowId, setEditRowId] = useState(null);
  const [editRemarks, setEditRemarks] = useState('');

  // ── Query Response modal ──────────────────────────────────────────────────
  const [modalRow, setModalRow] = useState(null);
  const [responseText, setResponseText] = useState('');
  const [responseFile, setResponseFile] = useState(null);
  const [proceedWith, setProceedWith] = useState('YES');
  const [respLoading, setRespLoading] = useState(false);

  // ── Sorting ───────────────────────────────────────────────────────────────
  const handleSort = (col) => {
    const isAsc = orderBy === col && order === 'asc';
    setOrder(isAsc ? 'desc' : 'asc');
    setOrderBy(col);
  };

  const sorted = useMemo(() => [...rows].sort((a, b) => {
    const av = a[orderBy] ?? '';
    const bv = b[orderBy] ?? '';
    return order === 'asc'
      ? String(av).localeCompare(String(bv))
      : String(bv).localeCompare(String(av));
  }), [rows, orderBy, order]);

  const paginated = sorted.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage);

  // ── Edit remarks ──────────────────────────────────────────────────────────
  const startEdit = (row) => {
    setEditRowId(row.ptId);
    setEditRemarks(row.remarks ?? '');
  };
  const cancelEdit = () => setEditRowId(null);

  const saveEdit = async (ptId, currentResponse) => {
    try {
      await updateQuery(ptId, { remarks: editRemarks, response: currentResponse });
      notify.success('Remarks updated');
      setEditRowId(null);
      onRefresh();
    } catch {
      notify.error('Failed to update remarks');
    }
  };

  // ── Open Query Response modal ─────────────────────────────────────────────
  const openModal = (row) => {
    setModalRow(row);
    setResponseText('');
    setResponseFile(null);
    setProceedWith('YES');
    setEditRowId(null);
  };
  const closeModal = () => {
    setModalRow(null);
    setResponseText('');
    setResponseFile(null);
    setProceedWith('YES');
  };

  // ── Submit Query Response (mirrors btnsubmit_Click) ───────────────────────
  const submitResponse = async () => {
    if (!responseText.trim()) { notify.error('Please enter response remarks'); return; }
    setRespLoading(true);
    try {
      const fd = new FormData();
      fd.append('ptId', modalRow.ptId);
      fd.append('resolveRemarks', responseText);
      fd.append('userName', USER);
      fd.append('proceedWith', proceedWith);
      // Context for building the UNC upload path (mirrors Session values in ASPX)
      fd.append('customerCode', gridContext.customerCode ?? '');
      fd.append('customerName', gridContext.customerName ?? '');
      fd.append('journalName', gridContext.journalName ?? '');
      fd.append('articleCode', Array.isArray(gridContext.articles)
        ? gridContext.articles.join(',')
        : (gridContext.articles ?? ''));
      fd.append('stageName', gridContext.stageName ?? '');
      if (responseFile) fd.append('zipFile', responseFile);

      await api.post('/api/submission/resolve-query', fd, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      notify.success('QUERY RESOLVED SUCCESSFULLY');
      closeModal();
      onRefresh();
    } catch (err) {
      notify.error(err.response?.data?.message || 'Failed to resolve query');
    } finally {
      setRespLoading(false);
    }
  };

  // ── Delete ────────────────────────────────────────────────────────────────
  const handleDelete = async (ptId) => {
    if (!window.confirm('Delete this query?')) return;
    try {
      await deleteQuery(ptId);
      notify.success('Query deleted');
      onRefresh();
    } catch {
      notify.error('Failed to delete query');
    }
  };

  // ── Approve instruction ───────────────────────────────────────────────────
  const handleApprove = async (row) => {
    try {
      await approveInstruction(row.ptId);
      notify.success('Updated');
      onRefresh();
    } catch {
      notify.error('Failed to approve');
    }
  };

  const rowBg = (s) => STATUS_BG[s?.toLowerCase()] ?? '#ffffff';
  const chipStyle = (s) => CHIP_STYLE[s?.toLowerCase()] ?? CHIP_STYLE.saved;
  const formatDate = (v) => {
    if (!v) return '';
    const d = new Date(v);
    // If time is exactly midnight (00:00:00), show date only — SP returns date-only for some fields
    if (d.getHours() === 0 && d.getMinutes() === 0 && d.getSeconds() === 0) {
      return d.toLocaleDateString('en-US', { month: '2-digit', day: '2-digit', year: 'numeric' });
    }
    return d.toLocaleString('en-US', { month: '2-digit', day: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
  };

  // Format dates in the modal header — same logic
  const formatModalDate = (v) => {
    if (!v) return '—';
    const d = new Date(v);
    if (d.getHours() === 0 && d.getMinutes() === 0 && d.getSeconds() === 0) {
      return d.toLocaleDateString('en-US', { month: '2-digit', day: '2-digit', year: 'numeric' });
    }
    return d.toLocaleString('en-US', { month: '2-digit', day: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
  };

  // ── Build modal header title (mirrors lblHeader.Text in ASPX) ────────────
  const modalTitle = modalRow
    ? `Query details of   ${gridContext.customerName ?? ''}-${gridContext.journalName ?? ''}-${Array.isArray(gridContext.articles) ? gridContext.articles.join(',') : (gridContext.articles ?? '')
    }-${gridContext.stageName ?? ''}`
    : '';

  return (
    <>
      {/* ── Grid ─────────────────────────────────────────────────────────── */}
      <Paper elevation={2} sx={{ width: '100%', overflow: 'hidden' }}>
        <TableContainer sx={{ maxHeight: 600 }}>
          <Table stickyHeader size="small">
            <TableHead>
              <TableRow>
                {COLUMNS.map((col) => (
                  <TableCell key={col.id} style={{
                    width: col.width, fontWeight: 'bold',
                    color: 'navy', background: '#dbeaf9',
                  }}>
                    {col.sortable !== false ? (
                      <TableSortLabel
                        active={orderBy === col.id}
                        direction={orderBy === col.id ? order : 'asc'}
                        onClick={() => handleSort(col.id)}
                      >
                        {col.label}
                      </TableSortLabel>
                    ) : col.label}
                  </TableCell>
                ))}
              </TableRow>
            </TableHead>

            <TableBody>
              {paginated.length === 0 && (
                <TableRow>
                  <TableCell colSpan={COLUMNS.length} align="center" sx={{ color: 'gray', py: 4 }}>
                    No records found!
                  </TableCell>
                </TableRow>
              )}

              {paginated.map((row) => {
                const isEditing = editRowId === row.ptId;
                const bg = rowBg(row.status);
                const statusLower = row.status?.toLowerCase() ?? '';
                const isResolved = statusLower === 'resolved';
                const isRaised = statusLower === 'raised';
                const isQuery = row.queryType?.toUpperCase() === 'QUERY';
                const isInstr = row.queryType?.toUpperCase() === 'INSTRUCTION';

                return (
                  <TableRow key={row.ptId} sx={{ background: bg }}>

                    <TableCell sx={{ fontSize: '0.8rem' }}>{row.ptCode}</TableCell>
                    <TableCell sx={{ fontSize: '0.8rem' }}>{row.article}</TableCell>

                    {/* Remarks — inline edit */}
                    <TableCell>
                      {isEditing ? (
                        <TextField multiline minRows={2} size="small" fullWidth
                          value={editRemarks}
                          onChange={(e) => setEditRemarks(e.target.value)}
                        />
                      ) : (
                        <span style={{ whiteSpace: 'pre-wrap', fontSize: '0.8rem' }}>{row.remarks}</span>
                      )}
                    </TableCell>

                    {/* Status chip */}
                    <TableCell>
                      <Chip
                        label={
                          row.status?.toLowerCase() === 'saved' ? 'Draft' :
                            row.status?.toLowerCase() === 'raised' ? 'Open' :
                              row.status
                        }
                        size="small"
                        sx={{ ...chipStyle(row.status), fontWeight: 700, fontSize: '0.7rem', height: 22 }} />
                    </TableCell>

                    <TableCell sx={{ fontSize: '0.8rem' }}>{row.queryType}</TableCell>

                    {/* Response — read-only */}
                    <TableCell>
                      <span style={{ whiteSpace: 'pre-wrap', fontSize: '0.8rem' }}>{row.response}</span>
                    </TableCell>

                    <TableCell sx={{ fontSize: '0.75rem' }}>{formatDate(row.createdDate)}</TableCell>

                    {/* Actions */}
                    <TableCell>
                      {isEditing ? (
                        <Box display="flex" flexDirection="column" gap={0.5}>
                          <Tooltip title="Save">
                            <IconButton size="small" color="primary"
                              onClick={() => saveEdit(row.ptId, row.response)}>
                              <SaveIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                          <Tooltip title="Cancel">
                            <IconButton size="small" onClick={cancelEdit}>
                              <CancelIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        </Box>
                      ) : (
                        <Box display="flex" flexDirection="row" alignItems="center" gap={0.25} flexWrap="nowrap">

                          {/* Query Response icon — QUERY type only (mirrors QueryResolveButton.Visible in ASPX) */}
                          {isQuery && (
                            <Tooltip title="Respond to Query">
                              <IconButton size="small" sx={{ color: '#1565c0', p: 0.25 }}
                                onClick={() => openModal(row)}>
                                <QuestionAnswerIcon sx={{ fontSize: 16 }} />
                              </IconButton>
                            </Tooltip>
                          )}

                          {/* Delete */}
                          <Tooltip title="Delete">
                            <IconButton size="small" color="error" sx={{ p: 0.25 }}
                              onClick={() => handleDelete(row.ptId)}>
                              <DeleteIcon sx={{ fontSize: 16 }} />
                            </IconButton>
                          </Tooltip>

                          {/* Edit — not for Resolved */}
                          {!isResolved && (
                            <Tooltip title="Edit">
                              <IconButton size="small" color="primary" sx={{ p: 0.25 }}
                                onClick={() => startEdit(row)}>
                                <EditIcon sx={{ fontSize: 16 }} />
                              </IconButton>
                            </Tooltip>
                          )}

                          {/* Approve instruction */}
                          {isInstr && row.approveVisible && (
                            <Tooltip title="Approve">
                              <IconButton size="small" color="success" sx={{ p: 0.25 }}
                                onClick={() => handleApprove(row)}>
                                <CheckCircleIcon sx={{ fontSize: 16 }} />
                              </IconButton>
                            </Tooltip>
                          )}
                        </Box>
                      )}
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>

        <TablePagination
          component="div"
          count={rows.length}
          page={page}
          rowsPerPage={rowsPerPage}
          onPageChange={(_, p) => setPage(p)}
          onRowsPerPageChange={(e) => { setRowsPerPage(+e.target.value); setPage(0); }}
          rowsPerPageOptions={[5, 10, 25, 50]}
        />
      </Paper>

      {/* ── Query Response Modal (mirrors ModalPopupExtender in ASPX) ──────── */}
      <Dialog
        open={!!modalRow}
        onClose={closeModal}
        maxWidth="md"
        fullWidth
        PaperProps={{
          sx: {
            borderRadius: 1,
            border: '1px solid #aaa',
            boxShadow: '4px 4px 12px rgba(0,0,0,0.3)',
          }
        }}
      >
        {/* ── Title bar — matches "Query details of ..." header ── */}
        <DialogTitle sx={{
          bgcolor: '#fff',
          borderBottom: '2px solid #1a1a2e',
          py: 1, px: 2,
          display: 'flex', justifyContent: 'space-between', alignItems: 'center',
        }}>
          <Typography sx={{ fontSize: '0.9rem', fontWeight: 700, color: '#1a1a2e' }}>
            {modalTitle}
          </Typography>
          <IconButton size="small" onClick={closeModal} sx={{ color: '#555' }}>
            <CloseIcon fontSize="small" />
          </IconButton>
        </DialogTitle>

        <DialogContent sx={{ p: 0 }}>
          {modalRow && (
            <>
              {/* ── dvHeader — Created By / Sent By / Resolved By ── */}
              <Box sx={{ bgcolor: '#dbeaf9', px: 2, py: 1.25, borderBottom: '1px solid #ccc' }}>
                {/* Use CSS grid: 3 columns, 2 rows — labels top, values bottom */}
                <Box sx={{
                  display: 'grid',
                  gridTemplateColumns: '1fr 1fr 1fr',
                  rowGap: '2px',
                  columnGap: '8px',
                }}>
                  {/* Row 1 — Labels */}
                  {[
                    { label: 'Created By', value: modalRow.createdBy },
                    { label: 'Sent By', value: modalRow.sentBy },
                    { label: 'Resolved By', value: modalRow.resolvedBy },
                  ].map(({ label, value }) => (
                    <Box key={label} sx={{ display: 'flex', alignItems: 'baseline', gap: '6px', minWidth: 0 }}>
                      <Typography component="span" sx={{
                        fontSize: '0.78rem', fontWeight: 700, color: '#333',
                        whiteSpace: 'nowrap', flexShrink: 0,
                      }}>
                        {label}
                      </Typography>
                      <Typography component="span" sx={{
                        fontSize: '0.78rem', color: '#1565c0',
                        overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap',
                      }}>
                        {value || '—'}
                      </Typography>
                    </Box>
                  ))}

                  {/* Row 2 — Dates */}
                  {[
                    { label: 'Created Date', value: formatModalDate(modalRow.createdDate) },
                    { label: 'Sent Date', value: formatModalDate(modalRow.sentDate) },
                    { label: 'Resolved Date', value: formatModalDate(modalRow.resolvedDate) },
                  ].map(({ label, value }) => (
                    <Box key={label} sx={{ display: 'flex', alignItems: 'baseline', gap: '6px', minWidth: 0 }}>
                      <Typography component="span" sx={{
                        fontSize: '0.78rem', fontWeight: 700, color: '#333',
                        whiteSpace: 'nowrap', flexShrink: 0,
                      }}>
                        {label}
                      </Typography>
                      <Typography component="span" sx={{
                        fontSize: '0.78rem', color: '#1565c0',
                        whiteSpace: 'nowrap',
                      }}>
                        {value || '—'}
                      </Typography>
                    </Box>
                  ))}
                </Box>
              </Box>

              {/* ── gvwPTInsEdit — same MUI Table style as query results grid ── */}
              <Box sx={{ borderBottom: '1px solid #a8c4e0' }}>
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>Departments</TableCell>
                      <TableCell>Problem Type</TableCell>
                      <TableCell>Problem Category</TableCell>
                      <TableCell>Remarks</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    <TableRow>
                      <TableCell sx={{ verticalAlign: 'top' }}>
                        {modalRow.deptName || gridContext.deptName || ''}
                      </TableCell>
                      <TableCell sx={{ verticalAlign: 'top' }}>
                        {modalRow.problemTypeName || ''}
                      </TableCell>
                      <TableCell sx={{ verticalAlign: 'top' }}>
                        {modalRow.categoryName || ''}
                      </TableCell>
                      <TableCell sx={{ verticalAlign: 'top', whiteSpace: 'pre-wrap', wordBreak: 'break-word', minWidth: 200 }}>
                        {modalRow.ptcRemarks || modalRow.remarks || ''}
                      </TableCell>
                    </TableRow>
                  </TableBody>
                </Table>
              </Box>

              {/* ── Query Response input section — hidden when Resolved ── */}
              {modalRow.status?.toLowerCase() !== 'resolved' && (
                <Box sx={{ p: 2 }}>
                  {/* Attachment */}
                  <Box display="flex" alignItems="center" gap={1.5} mb={1.5}>
                    <Typography sx={{ fontSize: '0.82rem', color: '#333', minWidth: 80 }}>
                      Attachment
                    </Typography>
                    <label style={{
                      display: 'inline-flex', alignItems: 'center', gap: 4,
                      padding: '3px 10px', border: '1px solid #aaa',
                      borderRadius: 2, cursor: 'pointer', fontSize: 12,
                      background: '#f0f0f0',
                    }}>
                      <AttachFileIcon style={{ fontSize: 13 }} />
                      Choose File
                      <input type="file" style={{ display: 'none' }}
                        onChange={(e) => setResponseFile(e.target.files[0] || null)} />
                    </label>
                    <Typography variant="caption" color="text.secondary">
                      {responseFile ? responseFile.name : 'No file chosen'}
                    </Typography>
                  </Box>

                  {/* Remarks textarea */}
                  <Box display="flex" alignItems="flex-start" gap={1.5} mb={1.5}>
                    <Typography sx={{ fontSize: '0.82rem', color: '#333', minWidth: 80, pt: 0.5 }}>
                      Remarks:
                    </Typography>
                    <TextField
                      multiline rows={5} size="small" fullWidth
                      value={responseText}
                      onChange={(e) => setResponseText(e.target.value)}
                      placeholder="Enter response remarks…"
                      sx={{ '& .MuiOutlinedInput-root': { fontSize: '0.82rem', bgcolor: '#fff' } }}
                    />
                  </Box>
                </Box>
              )}
            </>
          )}
        </DialogContent>

        {/* ── Action buttons — matches Attachment | Raise Query | Query Response | Cancel | Proceed with Query ── */}
        <DialogActions sx={{
          px: 2, py: 1.25,
          borderTop: '1px solid #ccc',
          justifyContent: 'flex-start',
          gap: 1,
          flexWrap: 'wrap',
        }}>
          {modalRow?.status?.toLowerCase() !== 'resolved' ? (
            <>
              <Button variant="outlined" size="small"
                onClick={submitResponse} disabled={respLoading}
                sx={{ textTransform: 'none', fontSize: '0.8rem', borderColor: '#aaa', color: '#333', '&:hover': { bgcolor: '#f0f0f0' } }}>
                Submit Response
              </Button>
              <Button variant="outlined" size="small"
                onClick={closeModal} disabled={respLoading}
                sx={{ textTransform: 'none', fontSize: '0.8rem', borderColor: '#aaa', color: '#333', '&:hover': { bgcolor: '#f0f0f0' } }}>
                Cancel
              </Button>
              {/* Proceed with Query — mirrors ProceedWithQuery_DropDownList1 */}
              <Box display="flex" alignItems="center" gap={0.75} ml={0.5}>
                <Typography sx={{ fontSize: '0.82rem', fontWeight: 700, color: '#333' }}>
                  Proceed with Query
                </Typography>
                <select
                  value={proceedWith}
                  onChange={(e) => setProceedWith(e.target.value)}
                  style={{
                    height: 26, fontSize: 12, border: '1px solid #aaa',
                    borderRadius: 2, padding: '0 4px', background: '#fff',
                  }}
                >
                  <option value="YES">YES</option>
                  <option value="NO">NO</option>
                </select>
              </Box>
            </>
          ) : (
            <Button variant="outlined" size="small"
              onClick={closeModal}
              sx={{ textTransform: 'none', fontSize: '0.8rem', borderColor: '#aaa', color: '#333' }}>
              Close
            </Button>
          )}
        </DialogActions>
      </Dialog>
    </>
  );
}
