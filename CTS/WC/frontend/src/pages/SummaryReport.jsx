import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box, Typography, Paper, Button,
  Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, CircularProgress, Alert,
  Dialog, DialogTitle, DialogContent, IconButton,
  TextField,
} from '@mui/material';
import ArrowBackIcon    from '@mui/icons-material/ArrowBack';
import CloseIcon        from '@mui/icons-material/Close';
import SearchIcon       from '@mui/icons-material/Search';
import DownloadIcon     from '@mui/icons-material/Download';
import api              from '../utils/api';
import * as XLSX        from 'xlsx';
import { getCustomers, getSummaryRows } from '../services/raiseProblemsService';
import SearchableSelect from '../components/SearchableSelect';

const CURRENT_USER = sessionStorage.getItem('userName') || 'MAC014393';

// ── Design tokens ─────────────────────────────────────────────────────────────
const FONT_FAMILY    = '"Segoe UI", Arial, sans-serif';
const FONT_SIZE      = '0.8rem';
const COLOR_NAVY     = '#1a3a5c';
const COLOR_TH_BG    = '#c8dff5';
const COLOR_TH_BORDER= '#a8c4e0';
const COLOR_TD_BORDER= '#d0dcea';

// Summary table header
const sumTh = {
  bgcolor:    '#c8dff5',
  color:      '#1a3a5c',
  fontWeight: 700,
  fontSize:   FONT_SIZE,
  fontFamily: FONT_FAMILY,
  border:     '1px solid #a8c4e0',
  textAlign:  'center',
  py: 1, px: 1.5,
  letterSpacing: 0.2,
};
// Summary table cell
const sumTd = {
  border:     '1px solid #d0dcea',
  fontSize:   FONT_SIZE,
  fontFamily: FONT_FAMILY,
  py: 0.75, px: 1.5,
};

// Detail dialog header — matches screenshot blue
const detTh = {
  bgcolor:    '#c8dff5',
  color:      '#1a3a5c',
  fontWeight: 700,
  fontSize:   '0.78rem',
  fontFamily: FONT_FAMILY,
  border:     '1px solid #a8c4e0',
  textAlign:  'center',
  py: 0.9, px: 1.25,
  whiteSpace: 'nowrap',
  letterSpacing: 0.2,
};
// Detail dialog cell
const detTd = {
  border:     '1px solid #d8e4f0',
  fontSize:   '0.78rem',
  fontFamily: FONT_FAMILY,
  py: 0.6, px: 1.25,
  verticalAlign: 'top',
};

// ── Clickable count cell ──────────────────────────────────────────────────────
function CountCell({ count, stageId, stageName, queryType, reportType, customerCode, onExpand }) {
  if (count === 0) {
    return (
      <TableCell sx={{ ...sumTd, textAlign: 'center', color: '#555' }}>0</TableCell>
    );
  }
  return (
    <TableCell sx={{ ...sumTd, textAlign: 'center' }}>
      <Box component="span"
        onClick={() => onExpand({ stageId, stageName, queryType, reportType, customerCode })}
        sx={{
          color: '#1565c0', fontWeight: 700, cursor: 'pointer',
          textDecoration: 'underline', fontSize: FONT_SIZE,
          fontFamily: FONT_FAMILY,
          '&:hover': { color: '#0d47a1' },
        }}
      >
        {count}
      </Box>
    </TableCell>
  );
}

// ── Detail Dialog ─────────────────────────────────────────────────────────────
function DetailDialog({ params, onClose }) {
  const [data,      setData]      = useState([]);
  const [loading,   setLoading]   = useState(false);
  const [error,     setError]     = useState('');
  const [fromDate,  setFromDate]  = useState('');
  const [toDate,    setToDate]    = useState('');

  // Auto-load when dialog opens (no date filter initially)
  useEffect(() => {
    if (!params) return;
    setFromDate('');
    setToDate('');
    fetchData('', '');
  }, [params]); // eslint-disable-line

  const fetchData = useCallback(async (fDate, tDate) => {
    if (!params) return;
    setLoading(true);
    setError('');
    const endpoint = params.reportType === 'resolved'
      ? '/api/reportdetail/resolved'
      : '/api/reportdetail/awaiting';
    try {
    const res = await api.get(endpoint, {
        params: {
          stageId:      params.stageId,
          customerCode: params.customerCode,
          queryType:    params.queryType,
          fromDate:     fDate || undefined,
          toDate:       tDate || undefined,
        },
      });
      setData(res.data);
    } catch {
      setError('Failed to load report data');
    } finally {
      setLoading(false);
    }
  }, [params]);

  // View button — mirrors btnView_Click
  const handleView = () => {
    if (!fromDate || !toDate) {
      alert('Please enter the From Date and To Date');
      return;
    }
    fetchData(fromDate, toDate);
  };

  // Export to Excel — mirrors btnExportExcel_Click
  const handleExport = () => {
    if (data.length === 0) { alert('No data to export'); return; }
    const isResolved = params?.reportType === 'resolved';

    // Build worksheet rows matching original ASPX BoundColumn headers
    const headers = isResolved
      ? ['Customer Name','Book','CustomerCode','BookCode','Stage','Chapter',
         'Queries Raised to Customer','Resolved Remarks',
         'Reported Date','Reopened Date','QueryResolved Date','TotalTimeTaken(Days)']
      : ['Customer Name','Book','CustomerCode','BookCode','Stage','Chapter',
         'Queries Raised to Customer','Reported Date','Days Pending'];

    const rows = data.map((r) => isResolved
      ? [r.customerName, r.bookName, r.customerCode, r.bookCode, r.stage, r.chapter,
         r.queryRaised, r.resolvedRemarks, r.reportedDate, r.reopenedDate, r.resolvedDate, r.totalTime]
      : [r.customerName, r.bookName, r.customerCode, r.bookCode, r.stage, r.chapter,
         r.queryRaised, r.reportedDate, r.days]
    );

    const ws = XLSX.utils.aoa_to_sheet([headers, ...rows]);
    // Auto column widths
    ws['!cols'] = headers.map((h, i) => ({
      wch: Math.max(h.length, ...rows.map((r) => String(r[i] ?? '').length), 12),
    }));

    const wb = XLSX.utils.book_new();
    const sheetName = isResolved ? 'QueriesResolved' : 'QueriesPending';
    XLSX.utils.book_append_sheet(wb, ws, sheetName);

    const fileName = `${sheetName}_${params?.stageName ?? ''}_${new Date().toISOString().slice(0,10)}.xlsx`;
    XLSX.writeFile(wb, fileName);
  };

  const isResolved = params?.reportType === 'resolved';
  const title = isResolved
    ? `Resolved Queries Archive — ${params?.stageName}`
    : `Awaiting Reply — ${params?.stageName}`;

  return (
    <Dialog
      open={!!params}
      onClose={onClose}
      maxWidth="xl"
      fullWidth
      PaperProps={{ sx: { borderRadius: 1, boxShadow: '0 8px 32px rgba(0,0,0,0.18)', maxHeight: '90vh' } }}
    >
      {/* Title */}
      <DialogTitle sx={{
        bgcolor: '#dbeaf9', borderBottom: `2px solid ${COLOR_TH_BORDER}`,
        display: 'flex', justifyContent: 'space-between', alignItems: 'center',
      }}>
        <span>{title}</span>
        <IconButton size="small" onClick={onClose} sx={{ color: '#555', '&:hover': { bgcolor: '#c8dff5' } }}>
          <CloseIcon sx={{ fontSize: 18 }} />
        </IconButton>
      </DialogTitle>

      {/* Filter bar — From Date | To Date | View | Export to Excel */}
      <Box sx={{
        display: 'flex', alignItems: 'center', gap: 2, flexWrap: 'wrap',
        px: 2, py: 1.25, bgcolor: '#f4f8fd', borderBottom: `1px solid ${COLOR_TD_BORDER}`,
      }}>
        <Typography variant="body2" sx={{ fontWeight: 600, color: '#1a3a5c', whiteSpace: 'nowrap' }}>
          From Date :
        </Typography>
        <TextField
          type="date" size="small" value={fromDate}
          onChange={(e) => setFromDate(e.target.value)}
          InputLabelProps={{ shrink: true }}
          sx={{ width: 160, bgcolor: '#fff' }}
        />
        <Typography variant="body2" sx={{ fontWeight: 600, color: '#1a3a5c', whiteSpace: 'nowrap' }}>
          To Date :
        </Typography>
        <TextField
          type="date" size="small" value={toDate}
          onChange={(e) => setToDate(e.target.value)}
          InputLabelProps={{ shrink: true }}
          sx={{ width: 160, bgcolor: '#fff' }}
        />
        <Button
          variant="outlined" size="small"
          startIcon={<SearchIcon />}
          onClick={handleView}
          disabled={loading}
          sx={{ borderColor: COLOR_TH_BORDER, color: '#1a3a5c', '&:hover': { bgcolor: '#dbeaf9' } }}
        >
          View
        </Button>
        <Button
          variant="outlined" size="small"
          startIcon={<DownloadIcon />}
          onClick={handleExport}
          disabled={loading || data.length === 0}
          sx={{ borderColor: '#2e7d32', color: '#2e7d32', '&:hover': { bgcolor: '#e8f5e9' } }}
        >
          Export to Excel
        </Button>
        {data.length > 0 && (
          <Typography variant="caption" sx={{ color: '#555', ml: 'auto' }}>
            {data.length} record{data.length !== 1 ? 's' : ''}
          </Typography>
        )}
      </Box>

      <DialogContent sx={{ p: 0, bgcolor: '#fff' }}>
        {loading && (
          <Box display="flex" justifyContent="center" alignItems="center" py={5}>
            <CircularProgress size={30} />
          </Box>
        )}
        {error && <Alert severity="error" sx={{ m: 2 }}>{error}</Alert>}

        {!loading && !error && (
          <TableContainer sx={{ maxHeight: 'calc(90vh - 160px)' }}>
            <Table size="small" stickyHeader>
              <TableHead>
                <TableRow>
                  <TableCell sx={{ textAlign: 'left', minWidth: 90 }}>Customer Name</TableCell>
                  <TableCell sx={{ textAlign: 'left', minWidth: 140 }}>Book</TableCell>
                  <TableCell sx={{ minWidth: 80 }}>CustomerCode</TableCell>
                  <TableCell sx={{ minWidth: 90 }}>BookCode</TableCell>
                  <TableCell sx={{ minWidth: 70 }}>Stage</TableCell>
                  <TableCell sx={{ minWidth: 70 }}>Chapter</TableCell>
                  <TableCell sx={{ textAlign: 'left', minWidth: 200 }}>Queries Raised to Customer</TableCell>
                  {isResolved && <TableCell sx={{ textAlign: 'left', minWidth: 200 }}>Resolved Remarks</TableCell>}
                  <TableCell sx={{ minWidth: 100 }}>Reported Date</TableCell>
                  {isResolved && <TableCell sx={{ minWidth: 100 }}>Reopened Date</TableCell>}
                  {isResolved && <TableCell sx={{ minWidth: 120 }}>QueryResolved Date</TableCell>}
                  {isResolved && <TableCell sx={{ minWidth: 70 }}>TotalTime</TableCell>}
                  {!isResolved && <TableCell sx={{ minWidth: 60 }}>Days</TableCell>}
                </TableRow>
              </TableHead>
              <TableBody>
                {data.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={12} align="center" sx={{ py: 5, color: '#888' }}>
                      No records found
                    </TableCell>
                  </TableRow>
                )}
                {data.map((r, i) => (
                  <TableRow key={i}>
                    <TableCell>{r.customerName}</TableCell>
                    <TableCell>{r.bookName}</TableCell>
                    <TableCell sx={{ textAlign: 'center' }}>{r.customerCode}</TableCell>
                    <TableCell sx={{ textAlign: 'center' }}>{r.bookCode}</TableCell>
                    <TableCell sx={{ textAlign: 'center' }}>{r.stage}</TableCell>
                    <TableCell sx={{ textAlign: 'center' }}>{r.chapter}</TableCell>
                    <TableCell sx={{ maxWidth: 260, whiteSpace: 'pre-wrap', wordBreak: 'break-word' }}>{r.queryRaised}</TableCell>
                    {isResolved && <TableCell sx={{ maxWidth: 260, whiteSpace: 'pre-wrap', wordBreak: 'break-word' }}>{r.resolvedRemarks}</TableCell>}
                    <TableCell sx={{ textAlign: 'center', whiteSpace: 'nowrap' }}>{r.reportedDate}</TableCell>
                    {isResolved && <TableCell sx={{ textAlign: 'center', whiteSpace: 'nowrap' }}>{r.reopenedDate}</TableCell>}
                    {isResolved && <TableCell sx={{ textAlign: 'center', whiteSpace: 'nowrap' }}>{r.resolvedDate}</TableCell>}
                    {isResolved && <TableCell sx={{ textAlign: 'center' }}>{r.totalTime}</TableCell>}
                    {!isResolved && <TableCell sx={{ textAlign: 'center' }}>{r.days}</TableCell>}
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </DialogContent>
    </Dialog>
  );
}

// ── Main Summary Report page ──────────────────────────────────────────────────
export default function SummaryReport() {
  const navigate = useNavigate();

  const [customers,        setCustomers]        = useState([]);
  const [selectedCustomer, setSelectedCustomer] = useState('0');
  const [rows,             setRows]             = useState([]);
  const [loading,          setLoading]          = useState(false);
  const [error,            setError]            = useState('');
  const [activeDetail,     setActiveDetail]     = useState(null);

  useEffect(() => {
    getCustomers(CURRENT_USER)
      .then((res) => {
        setCustomers(res.data);
        if (res.data.length === 1) setSelectedCustomer(String(res.data[0].id));
      })
      .catch(() => setError('Failed to load customers'));
  }, []);

  useEffect(() => {
    if (selectedCustomer !== '0') fetchRows(selectedCustomer);
  }, [selectedCustomer]); // eslint-disable-line

  const fetchRows = useCallback(async (custCode) => {
    setLoading(true);
    setError('');
    setActiveDetail(null);
    try {
      const res = await getSummaryRows(custCode);
      setRows(res.data);
    } catch {
      setError('Failed to load summary report');
    } finally {
      setLoading(false);
    }
  }, []);

  const handleCustomerChange = (val) => {
    setSelectedCustomer(val);
    setRows([]);
    setActiveDetail(null);
    if (val !== '0') fetchRows(val);
  };

  const handleExpand = (params) => {
    const key       = `${params.stageId}-${params.reportType}-${params.queryType}`;
    const activeKey = activeDetail
      ? `${activeDetail.stageId}-${activeDetail.reportType}-${activeDetail.queryType}`
      : null;
    setActiveDetail(key === activeKey ? null : params);
  };

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: '#f4f7fb', p: { xs: 2, md: 4 }, fontFamily: FONT_FAMILY }}>
      <Box sx={{ maxWidth: 1000, mx: 'auto' }}>

        {/* ── Title ──────────────────────────────────────────────────────── */}
        <Typography sx={{
          fontSize: '1.15rem', fontWeight: 700, textAlign: 'center',
          textDecoration: 'underline', mb: 3, color: '#1a3a5c',
          fontFamily: FONT_FAMILY, letterSpacing: 0.5,
        }}>
          Query Dashboard
        </Typography>

        {/* ── Controls ───────────────────────────────────────────────────── */}
        <Box display="flex" justifyContent="space-between" alignItems="center"
          mb={3} flexWrap="wrap" gap={2}>
          <Box display="flex" alignItems="center" gap={2}>
            <Typography sx={{ fontSize: FONT_SIZE, fontWeight: 600, color: '#444', fontFamily: FONT_FAMILY }}>
              Customer
            </Typography>
            <Box sx={{ minWidth: 230 }}>
              <SearchableSelect
                options={customers.map((c) => ({ value: String(c.id), label: c.name }))}
                value={selectedCustomer}
                onChange={handleCustomerChange}
                placeholder="-Select-"
                header="Please select Customer"
              />
            </Box>
          </Box>
          <Button
            variant="outlined"
            startIcon={<ArrowBackIcon sx={{ fontSize: 16 }} />}
            onClick={() => navigate('/raise-problems')}
            sx={{
              borderRadius: 1, fontWeight: 600, textTransform: 'none',
              fontSize: FONT_SIZE, fontFamily: FONT_FAMILY,
              borderColor: '#a8c4e0', color: '#1a3a5c',
              '&:hover': { bgcolor: '#dbeaf9', borderColor: '#1565c0' },
            }}
          >
             Back to Tracker
          </Button>
        </Box>

        {error   && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
        {loading && <Box display="flex" justifyContent="center" py={6}><CircularProgress /></Box>}

        {/* ── Summary table ───────────────────────────────────────────────── */}
        {!loading && selectedCustomer !== '0' && (
          <TableContainer component={Paper} variant="outlined"
            sx={{ borderRadius: 1, border: '1px solid #a8c4e0', boxShadow: '0 2px 8px rgba(0,0,0,0.06)' }}>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell sx={{ ...sumTh, width: '25%', textAlign: 'left' }}>Stage</TableCell>
                  <TableCell sx={{ ...sumTh, width: '25%' }}>Instructions Pending</TableCell>
                  <TableCell sx={{ ...sumTh, width: '25%' }}>Queries Awaiting Reply</TableCell>
                  <TableCell sx={{ ...sumTh, width: '25%' }}>Resolved (Last 30 Days)</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {rows.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={4} align="center"
                      sx={{ py: 4, fontWeight: 700, color: '#555', fontSize: FONT_SIZE, fontFamily: FONT_FAMILY }}>
                      No queries found for this customer
                    </TableCell>
                  </TableRow>
                )}
                {rows.map((row, idx) => (
                  <TableRow key={`${row.stageId}-${idx}`} sx={{
                    bgcolor: idx % 2 === 0 ? '#f4f8fd' : '#ffffff',
                    '&:hover': { bgcolor: '#e8f0fb' },
                  }}>
                    <TableCell sx={{ ...sumTd, fontWeight: 600, color: '#1a3a5c' }}>
                      {row.stageName}
                    </TableCell>
                    <CountCell count={row.instruction}    stageId={row.stageId} stageName={row.stageName}
                      queryType="INSTRUCTION" reportType="awaiting" customerCode={selectedCustomer} onExpand={handleExpand} />
                    <CountCell count={row.queriesPending} stageId={row.stageId} stageName={row.stageName}
                      queryType="QUERY"       reportType="awaiting" customerCode={selectedCustomer} onExpand={handleExpand} />
                    <CountCell count={row.queriesResolved}stageId={row.stageId} stageName={row.stageName}
                      queryType="QUERY"       reportType="resolved" customerCode={selectedCustomer} onExpand={handleExpand} />
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}

        {!loading && selectedCustomer === '0' && (
          <Box display="flex" justifyContent="center" py={6}>
            <Typography sx={{ fontSize: FONT_SIZE, color: '#888', fontFamily: FONT_FAMILY }}>
              Select a customer to view the Query Dashboard.
            </Typography>
          </Box>
        )}
      </Box>

      {/* ── Detail popup ─────────────────────────────────────────────────── */}
      <DetailDialog params={activeDetail} onClose={() => setActiveDetail(null)} />
    </Box>
  );
}
