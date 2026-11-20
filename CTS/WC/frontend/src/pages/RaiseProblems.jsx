import React, { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import {
  Box, Typography, Paper, Grid, FormControl, Select, MenuItem,
  TextField, Button, Chip, Divider, CircularProgress, Alert,
} from '@mui/material';
import FilterListIcon from '@mui/icons-material/FilterList';
import SearchIcon from '@mui/icons-material/Search';
import CloseIcon from '@mui/icons-material/Close';
import MessageSquareIcon from '@mui/icons-material/ChatBubbleOutline';
import InboxIcon from '@mui/icons-material/Inbox';
import BarChartIcon from '@mui/icons-material/BarChart';
import Autocomplete from '@mui/material/Autocomplete';
import { useRaiseProblems } from '../hooks/useRaiseProblems';
import QueryGrid from '../components/QueryGrid';
import SearchableSelect from '../components/SearchableSelect';
import SubmissionDialog from '../components/SubmissionDialog';
import { notify } from '../utils/notify';

// Count how many filters are actively set
function countActiveFilters({ selectedCustomer, selectedJournal, selectedArticles, selectedStage, selectedDept, isbn }) {
  let n = 0;
  if (selectedCustomer && selectedCustomer !== '0') n++;
  if (selectedJournal && selectedJournal !== '0') n++;
  if (selectedArticles && selectedArticles.length > 0) n++;
  if (selectedStage && selectedStage !== '0') n++;
  if (selectedDept && selectedDept !== '0') n++;
  if (isbn) n++;
  return n;
}

export default function RaiseProblems() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  // ── Route guard — redirect to /login if no session user ─────────────────
  // Handles deep-link from Springer Dashboard CTS link:
  //   http://10.42.4.127/CTSUI/?Username=TUFDMDE0Mzkz-.../BookID=461997_2_En/#/raise-problems
  //
  // The params use slash-separators (not &), placed BEFORE the # so they land
  // in window.location.search. We parse them with parseSlashParams().
  //
  // parseSlashParams: splits "?Username=X/Password=Y/BookID=Z/" into key=value pairs
  function parseSlashParams(search) {
    const result = {};
    // Remove leading '?' then split on '/'
    const raw = (search || '').replace(/^\?/, '');
    raw.split('/').forEach((part) => {
      const eq = part.indexOf('=');
      if (eq === -1) return;
      const key = part.slice(0, eq).trim();
      const val = decodeURIComponent(part.slice(eq + 1).trim());
      if (key) result[key] = val;
    });
    return result;
  }

  const userName = (() => {
    const session = sessionStorage.getItem('userName');
    if (session) return session;

    // Parse slash-style params from window.location.search
    const slashParams = parseSlashParams(window.location.search);
    const urlUser = slashParams['Username'] || slashParams['username'];
    const urlPass = slashParams['Password'] || slashParams['password'];
    const urlBookId = slashParams['BookID'] || slashParams['bookId'];

    if (urlUser) {
      // Store the raw token — AuthService will decode it on the backend.
      // The decoded userCode is returned by POST /api/auth/login and stored
      // in sessionStorage by the login flow. For deep-links, we store the
      // token here and let the backend decode it via getCustomers(userName).
      // The backend's GetCustomers calls USP_PTS_GETUSERROLEBYUSERNAME which
      // expects the plain userCode — so we must decode first via the auth endpoint.
      // We store the raw token temporarily; the useEffect below will call login.
      sessionStorage.setItem('_pendingToken', urlUser.trim());
      if (urlPass)   sessionStorage.setItem('password', urlPass);
      if (urlBookId) sessionStorage.setItem('bookId',   urlBookId.trim());
      // Return null so the useEffect triggers the auth flow
      return null;
    }
    return null;
  })();

  // ── Deep-link auto-login: decode token via auth API ─────────────────────
  const [authLoading, setAuthLoading] = useState(false);
  useEffect(() => {
    const pendingToken = sessionStorage.getItem('_pendingToken');
    if (!pendingToken || authLoading) return;

    setAuthLoading(true);
    import('../utils/api').then(({ default: api }) => {
      api.post('/api/auth/login', { userName: pendingToken, password: '' })
        .then((res) => {
          const decoded = res.data?.userName || pendingToken.trim().toUpperCase();
          sessionStorage.setItem('userName', decoded);
          sessionStorage.removeItem('_pendingToken');
          // Force re-render by reloading — the page will now have a valid session
          window.location.replace(window.location.href);
        })
        .catch(() => {
          sessionStorage.removeItem('_pendingToken');
          navigate('/login', { replace: true });
        })
        .finally(() => setAuthLoading(false));
    });
  }, [navigate]);

  useEffect(() => {
    if (!userName && !sessionStorage.getItem('_pendingToken') && !authLoading) {
      const slashParams = parseSlashParams(window.location.search);
      const urlUser = slashParams['Username'] || slashParams['username'];
      if (urlUser) {
        navigate(`/login?username=${encodeURIComponent(urlUser)}`, { replace: true });
      } else {
        navigate('/login', { replace: true });
      }
    }
  }, [userName, navigate, searchParams, authLoading]);

  // ── Submission dialog state ──────────────────────────────────────────────
  const [submissionOpen, setSubmissionOpen] = useState(false);
  const [submissionQueryType, setSubmissionQueryType] = useState('QUERY');

  const {
    customers, journals, articles, departments, stages, queries,
    selectedCustomer, selectedJournal, selectedArticles, setSelectedArticles,
    selectedStage, selectedDept, setSelectedDept,
    isbn, barcode, setBarcode,
    showNewQueryBtn, showGoBtn, stageVariant, message, loading,
    handleCustomerChange, handleJournalChange, handleStageChange,
    handleGo, handleBarcodeGo,
    refreshQueries,
    // reset helper — we'll add it below
    handleReset,
  } = useRaiseProblems(userName);

  useEffect(() => {
    if (!message.text) return;
    if (message.type === 'error') notify.error(message.text);
    else if (message.type === 'success') notify.success(message.text);
  }, [message]);

  const goToSubmission = (queryType) => {
    if (selectedStage === '0') { notify.error('Please select Stage'); return; }
    if (selectedDept === '0') { notify.error('Please select Department'); return; }
    if (selectedArticles.length === 0) { notify.error('Please select at least one Job'); return; }
    setSubmissionQueryType(queryType);
    setSubmissionOpen(true);
  };

  const stageOptions = stages;
  const activeCount = countActiveFilters({ selectedCustomer, selectedJournal, selectedArticles, selectedStage, selectedDept, isbn });
  const hasResults = queries.length > 0;

  // Show New Query / New Instruction as soon as Customer + Project + Job + Stage are filled
  const canRaiseNew =
    selectedCustomer !== '0' &&
    selectedJournal !== '0' &&
    selectedArticles.length > 0 &&
    selectedStage !== '0';

  // Build context object for the dialog — must be after stageOptions
  const submissionContext = {
    customerCode: selectedCustomer,
    journalCode: selectedJournal,
    articles: selectedArticles,
    stageId: selectedStage,
    deptId: selectedDept,
    customerName: customers.find((c) => String(c.id) === selectedCustomer)?.name ?? '',
    journalName: journals.find((j) => String(j.id) === selectedJournal)?.name ?? '',
    stageName: stageOptions.find((s) => s.value === selectedStage)?.label ?? '',
    deptName: departments.find((d) => String(d.id) === selectedDept)?.name ?? '',
  };

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: '#f8fafc', p: { xs: 2, md: 4 } }}>
      <Box sx={{ maxWidth: 1100, mx: 'auto' }}>

        {/* ── Page Header ──────────────────────────────────────────────────── */}
        <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={3}>
          <Box display="flex" gap={2} alignItems="center">
            <Box sx={{
              width: 48, height: 48, borderRadius: 2,
              bgcolor: 'primary.main', display: 'flex',
              alignItems: 'center', justifyContent: 'center',
              boxShadow: '0 4px 14px rgba(25,118,210,0.3)',
            }}>
              <MessageSquareIcon sx={{ color: '#fff', fontSize: 24 }} />
            </Box>
            <Box>
              <Typography variant="h6" fontWeight={700} color="text.primary">
                Query Tracker
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Track and manage queries across projects, jobs and production stages.
              </Typography>
            </Box>
          </Box>
          <Box display="flex" alignItems="center" gap={1.5}>
            <Box sx={{
              bgcolor: '#fff', border: '1px solid', borderColor: 'divider',
              borderRadius: 5, px: 2, py: 0.5, boxShadow: 1,
            }}>
              <Typography variant="body2" fontWeight={700} color="text.primary">
                {hasResults ? `${queries.length} open quer${queries.length !== 1 ? 'ies' : 'y'}` : '0 open queries'}
              </Typography>
            </Box>
            {/* Report button — navigates to Summary Report page */}
            <Button
              variant="outlined"
              startIcon={<BarChartIcon />}
              onClick={() => navigate('/reports/summary')}
              sx={{
                borderRadius: 2, fontWeight: 600, textTransform: 'none',
                borderColor: 'divider', color: 'text.secondary',
                '&:hover': { borderColor: 'primary.main', color: 'primary.main', bgcolor: '#f0f7ff' },
              }}
            >
              Query Dashboard
            </Button>
          </Box>
        </Box>
        {/* ── Filters Card ─────────────────────────────────────────────────── */}
        <Paper variant="outlined" sx={{ borderRadius: 3, mb: 3, overflow: 'hidden' }}>

          {/* Card header */}
          <Box sx={{
            px: 3, py: 1.5, bgcolor: '#f8fafc',
            borderBottom: '1px solid', borderColor: 'divider',
            display: 'flex', justifyContent: 'space-between', alignItems: 'center',
          }}>
            <Box display="flex" alignItems="center" gap={1}>
              <FilterListIcon sx={{ fontSize: 18, color: 'text.secondary' }} />
              <Typography variant="body2" fontWeight={700} color="text.primary">
                Search Filters
              </Typography>
            </Box>
            {activeCount > 0 && (
              <Box sx={{
                bgcolor: '#eff6ff', color: 'primary.main',
                px: 1.5, py: 0.25, borderRadius: 1,
                fontSize: '0.7rem', fontWeight: 900, letterSpacing: 1,
                textTransform: 'uppercase',
              }}>
                {activeCount} active
              </Box>
            )}
          </Box>

          {/* Filter body */}
          <Box sx={{ p: 3 }}>
            <Typography variant="body2" color="text.secondary" mb={3}>
              Choose any combination, then press Search.
            </Typography>

            {/* Row 1: Customer | Project | Job */}
            <Grid container spacing={3} mb={3}>
              <Grid item xs={12} md={4}>
                <Typography variant="caption" fontWeight={700} color="text.secondary"
                  sx={{ textTransform: 'uppercase', letterSpacing: 0.8, display: 'block', mb: 0.75 }}>
                  Customer
                </Typography>
                <SearchableSelect
                  options={customers.map((c, i) => ({ value: String(c.id), label: c.name }))}
                  value={selectedCustomer}
                  onChange={handleCustomerChange}
                  placeholder="-Select-"
                  header="Please select Customer"
                />
              </Grid>

              <Grid item xs={12} md={4}>
                <Typography variant="caption" fontWeight={700} color="text.secondary"
                  sx={{ textTransform: 'uppercase', letterSpacing: 0.8, display: 'block', mb: 0.75 }}>
                  Project
                </Typography>
                <SearchableSelect
                  options={journals.map((j, i) => ({ value: String(j.id), label: j.name }))}
                  value={selectedJournal}
                  onChange={handleJournalChange}
                  placeholder="-Select-"
                  header="Please select Book"
                  disabled={!journals.length}
                />
              </Grid>

              <Grid item xs={12} md={4}>
                <Typography variant="caption" fontWeight={700} color="text.secondary"
                  sx={{ textTransform: 'uppercase', letterSpacing: 0.8, display: 'block', mb: 0.75 }}>
                  Job
                </Typography>
                <Autocomplete
                  multiple
                  size="small"
                  options={articles.map((a) => a.name)}
                  value={selectedArticles}
                  onChange={(_, val) => setSelectedArticles(val)}
                  disabled={!articles.length}
                  renderTags={(val, getTagProps) =>
                    val.map((opt, idx) => (
                      <Chip key={opt} label={opt} size="small"
                        sx={{ borderRadius: 1, height: 22, fontSize: '0.75rem' }}
                        {...getTagProps({ index: idx })} />
                    ))
                  }
                  renderInput={(params) => (
                    <TextField {...params} placeholder={articles.length ? 'Select job…' : '-Select-'}
                      sx={{ '& .MuiOutlinedInput-root': { borderRadius: 2, bgcolor: '#fff' } }} />
                  )}
                />
              </Grid>
            </Grid>

            {/* Row 2: ISBN | Stage | Department */}
            <Grid container spacing={3} mb={3}>
              <Grid item xs={12} md={4}>
                <Typography variant="caption" fontWeight={700} color="text.secondary"
                  sx={{ textTransform: 'uppercase', letterSpacing: 0.8, display: 'block', mb: 0.75 }}>
                  ISBN
                </Typography>
                <TextField
                  fullWidth size="small" value={isbn}
                  InputProps={{ readOnly: true }}
                  placeholder="Auto-filled from Project"
                  sx={{ '& .MuiOutlinedInput-root': { borderRadius: 2, bgcolor: '#f8fafc' } }}
                />
              </Grid>

              <Grid item xs={12} md={4}>
                <Typography variant="caption" fontWeight={700} color="text.secondary"
                  sx={{ textTransform: 'uppercase', letterSpacing: 0.8, display: 'block', mb: 0.75 }}>
                  Stage
                </Typography>
                <SearchableSelect
                  options={stageOptions.map((s) => ({ value: s.value, label: s.label }))}
                  value={selectedStage}
                  onChange={handleStageChange}
                  placeholder="-Select-"
                  header="Please select Stage"
                  disabled={stageVariant === 'none'}
                />
              </Grid>

              <Grid item xs={12} md={4}>
                <Typography variant="caption" fontWeight={700} color="text.secondary"
                  sx={{ textTransform: 'uppercase', letterSpacing: 0.8, display: 'block', mb: 0.75 }}>
                  Department
                </Typography>
                <SearchableSelect
                  options={departments.map((d) => ({ value: String(d.id), label: d.name }))}
                  value={selectedDept}
                  onChange={setSelectedDept}
                  placeholder="-Select-"
                  header="Please select Department"
                  disabled={!departments.length}
                />
              </Grid>
            </Grid>

            {/* Go / Reset buttons */}
            <Box display="flex" gap={1.5} alignItems="center">
              <Button
                variant="contained"
                startIcon={loading ? <CircularProgress size={16} color="inherit" /> : <SearchIcon />}
                onClick={handleGo}
                disabled={loading}
                sx={{ borderRadius: 2, px: 3, fontWeight: 700, textTransform: 'none' }}
              >
                Search
              </Button>
              <Button
                variant="outlined"
                startIcon={<CloseIcon />}
                onClick={handleReset}
                disabled={loading}
                sx={{
                  borderRadius: 2, px: 3, fontWeight: 700, textTransform: 'none',
                  borderColor: 'divider', color: 'text.secondary',
                  '&:hover': { borderColor: 'text.secondary', bgcolor: '#f8fafc' },
                }}
              >
                Clear Filters
              </Button>

              {/* New Query / New Instruction — shown when all required fields are filled */}
              {canRaiseNew && (
                <>
                  <Box sx={{ flex: 1 }} />
                  <Button
                    variant="contained"
                    color="error"
                    size="small"
                    sx={{ borderRadius: 2, fontWeight: 700, textTransform: 'none', px: 2 }}
                    onClick={() => goToSubmission('QUERY')}
                  >
                    Raise Query
                  </Button>
                  <Button
                    variant="contained"
                    size="small"
                    sx={{
                      borderRadius: 2, fontWeight: 700, textTransform: 'none', px: 2,
                      bgcolor: '#f57c00', '&:hover': { bgcolor: '#e65100' },
                    }}
                    onClick={() => goToSubmission('INSTRUCTION')}
                  >
                    Add Instruction
                  </Button>
                </>
              )}
            </Box>
          </Box>
        </Paper>

        {/* ── Results Card ─────────────────────────────────────────────────── */}
        <Paper variant="outlined" sx={{ borderRadius: 3, minHeight: 300 }}>
          <Box sx={{ px: 3, py: 2, borderBottom: '1px solid', borderColor: 'divider' }}>
            <Typography variant="body1" fontWeight={700} color="text.primary">
              Open Queries
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {loading
                ? 'Loading…'
                : `${queries.length} quer${queries.length !== 1 ? 'ies' : 'y'} found`}
            </Typography>
          </Box>

          {loading && (
            <Box display="flex" justifyContent="center" alignItems="center" py={8}>
              <CircularProgress />
            </Box>
          )}

          {!loading && queries.length === 0 && (
            <Box display="flex" flexDirection="column" alignItems="center"
              justifyContent="center" py={10} px={3} textAlign="center">
              <Box sx={{
                width: 72, height: 72, borderRadius: '50%',
                bgcolor: '#f1f5f9', border: '1px solid #e2e8f0',
                display: 'flex', alignItems: 'center', justifyContent: 'center', mb: 2.5,
              }}>
                <InboxIcon sx={{ fontSize: 32, color: '#cbd5e1' }} />
              </Box>
              <Typography variant="subtitle1" fontWeight={700} color="text.primary" mb={0.5}>
                No open queries found
              </Typography>
              <Typography variant="body2" color="text.secondary" maxWidth={360}>
                Try adjusting your filters or clearing them to start over.
              </Typography>
            </Box>
          )}

          {!loading && queries.length > 0 && (
            <QueryGrid
              rows={queries}
              onRefresh={refreshQueries}
              gridContext={submissionContext}
            />
          )}
        </Paper>

      </Box>

      {/* ── New Query / New Instruction popup ─────────────────────────────── */}
      <SubmissionDialog
        open={submissionOpen}
        onClose={() => setSubmissionOpen(false)}
        onSuccess={refreshQueries}
        queryType={submissionQueryType}
        context={submissionContext}
      />

    </Box>
  );
}