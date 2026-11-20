import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import RaiseProblems from './pages/RaiseProblems';
import RaiseProblemsSubmission from './pages/RaiseProblemsSubmission';
import SummaryReport from './pages/SummaryReport';
import Login from './pages/Login';

// ── Global design tokens ──────────────────────────────────────────────────────
export const FONT_FAMILY = '"Segoe UI", Arial, sans-serif';
export const FONT_SIZE   = '0.8rem';
export const COLOR_NAVY  = '#1a3a5c';
export const COLOR_TH_BG = '#c8dff5';
export const COLOR_TH_BORDER = '#a8c4e0';
export const COLOR_TD_BORDER = '#d0dcea';
export const COLOR_ROW_EVEN  = '#f4f8fd';
export const COLOR_ROW_ODD   = '#ffffff';
export const COLOR_ROW_HOVER = '#e8f0fb';
export const COLOR_PAGE_BG   = '#f4f7fb';

const theme = createTheme({
  palette: {
    primary:    { main: '#1565c0' },
    secondary:  { main: '#e53935' },
    background: { default: COLOR_PAGE_BG },
    text:       { primary: '#1a1a2e', secondary: '#555' },
  },

  typography: {
    fontFamily: FONT_FAMILY,
    fontSize:   13,
    body1:  { fontFamily: FONT_FAMILY, fontSize: FONT_SIZE },
    body2:  { fontFamily: FONT_FAMILY, fontSize: FONT_SIZE },
    caption:{ fontFamily: FONT_FAMILY, fontSize: '0.75rem' },
    subtitle1: { fontFamily: FONT_FAMILY, fontWeight: 600 },
    subtitle2: { fontFamily: FONT_FAMILY, fontWeight: 600 },
    h5: { fontFamily: FONT_FAMILY, fontWeight: 700 },
    h6: { fontFamily: FONT_FAMILY, fontWeight: 700 },
  },

  components: {
    // ── Global body font ──────────────────────────────────────────────────
    MuiCssBaseline: {
      styleOverrides: {
        '*, *::before, *::after': {
          fontFamily: `${FONT_FAMILY} !important`,
          boxSizing: 'border-box',
        },
        body: {
          fontFamily: FONT_FAMILY,
          fontSize: FONT_SIZE,
          backgroundColor: COLOR_PAGE_BG,
          color: '#1a1a2e',
        },
        // Native HTML elements used in popups (select, textarea, input, table)
        'select, textarea, input, button': {
          fontFamily: `${FONT_FAMILY} !important`,
          fontSize: `${FONT_SIZE} !important`,
        },
        'table': { borderCollapse: 'collapse', width: '100%' },
        'th': {
          backgroundColor: `${COLOR_TH_BG} !important`,
          color:            `${COLOR_NAVY} !important`,
          fontWeight:       '700 !important',
          fontSize:         `${FONT_SIZE} !important`,
          fontFamily:       `${FONT_FAMILY} !important`,
          letterSpacing:    '0.2px !important',
          border:           `1px solid ${COLOR_TH_BORDER} !important`,
          padding:          '7px 12px !important',
        },
        'td': {
          fontFamily: `${FONT_FAMILY} !important`,
          fontSize:   `${FONT_SIZE} !important`,
          border:     `1px solid ${COLOR_TD_BORDER} !important`,
          padding:    '7px 12px !important',
        },
      },
    },

    // ── MUI Button ────────────────────────────────────────────────────────
    MuiButton: {
      styleOverrides: {
        root: {
          textTransform: 'none',
          borderRadius: 4,
          fontFamily: FONT_FAMILY,
          fontSize: FONT_SIZE,
          fontWeight: 600,
        },
      },
    },

    // ── MUI Typography ────────────────────────────────────────────────────
    MuiTypography: {
      styleOverrides: {
        root: { fontFamily: FONT_FAMILY },
      },
    },

    // ── MUI Table ─────────────────────────────────────────────────────────
    MuiTableCell: {
      styleOverrides: {
        root: {
          fontFamily: FONT_FAMILY,
          fontSize: FONT_SIZE,
          padding: '7px 12px',
          borderColor: COLOR_TD_BORDER,
        },
        // Table header cells
        head: {
          backgroundColor: COLOR_TH_BG,
          color:           COLOR_NAVY,
          fontWeight:      700,
          fontSize:        FONT_SIZE,
          fontFamily:      FONT_FAMILY,
          letterSpacing:   '0.2px',
          borderColor:     COLOR_TH_BORDER,
        },
      },
    },

    MuiTableRow: {
      styleOverrides: {
        root: {
          '&:nth-of-type(even)': { backgroundColor: COLOR_ROW_EVEN },
          '&:nth-of-type(odd)':  { backgroundColor: COLOR_ROW_ODD  },
          '&:hover':             { backgroundColor: `${COLOR_ROW_HOVER} !important` },
        },
        // Don't stripe the header row
        head: {
          '&:nth-of-type(even)': { backgroundColor: COLOR_TH_BG },
          '&:nth-of-type(odd)':  { backgroundColor: COLOR_TH_BG },
          '&:hover':             { backgroundColor: `${COLOR_TH_BG} !important` },
        },
      },
    },

    MuiTableSortLabel: {
      styleOverrides: {
        root: {
          color:      `${COLOR_NAVY} !important`,
          fontFamily: FONT_FAMILY,
          fontSize:   FONT_SIZE,
          fontWeight: 700,
          '&:hover':  { color: '#1565c0 !important' },
          '&.Mui-active': { color: `${COLOR_NAVY} !important` },
        },
        icon: { color: `${COLOR_NAVY} !important` },
      },
    },

    // ── MUI Input / Select ────────────────────────────────────────────────
    MuiInputBase: {
      styleOverrides: {
        root: { fontFamily: FONT_FAMILY, fontSize: FONT_SIZE },
        input: { fontFamily: FONT_FAMILY, fontSize: FONT_SIZE },
      },
    },
    MuiOutlinedInput: {
      styleOverrides: {
        root: { fontFamily: FONT_FAMILY, fontSize: FONT_SIZE },
      },
    },
    MuiMenuItem: {
      styleOverrides: {
        root: { fontFamily: FONT_FAMILY, fontSize: FONT_SIZE },
      },
    },
    MuiInputLabel: {
      styleOverrides: {
        root: { fontFamily: FONT_FAMILY, fontSize: FONT_SIZE },
      },
    },

    // ── MUI Chip ──────────────────────────────────────────────────────────
    MuiChip: {
      styleOverrides: {
        root:  { fontFamily: FONT_FAMILY, fontSize: '0.75rem' },
        label: { fontFamily: FONT_FAMILY },
      },
    },

    // ── MUI Dialog ────────────────────────────────────────────────────────
    MuiDialogTitle: {
      styleOverrides: {
        root: {
          // No background-color here — each dialog sets its own via sx prop
          fontFamily: FONT_FAMILY,
          fontSize:   '0.9rem',
          fontWeight: 700,
          color:      COLOR_NAVY,
          padding:    '10px 16px',
        },
      },
    },
    MuiDialogContent: {
      styleOverrides: {
        root: { fontFamily: FONT_FAMILY, fontSize: FONT_SIZE },
      },
    },
    MuiDialogActions: {
      styleOverrides: {
        root: {
          fontFamily: FONT_FAMILY,
          // No background-color here — each dialog sets its own via sx prop
          borderTop:  `1px solid ${COLOR_TD_BORDER}`,
          padding:    '10px 16px',
        },
      },
    },

    // ── MUI Paper ─────────────────────────────────────────────────────────
    MuiPaper: {
      styleOverrides: {
        root: { fontFamily: FONT_FAMILY },
      },
    },

    // ── MUI Alert ─────────────────────────────────────────────────────────
    MuiAlert: {
      styleOverrides: {
        root: { fontFamily: FONT_FAMILY, fontSize: FONT_SIZE },
      },
    },

    // ── MUI Tooltip ───────────────────────────────────────────────────────
    MuiTooltip: {
      styleOverrides: {
        tooltip: { fontFamily: FONT_FAMILY, fontSize: '0.75rem' },
      },
    },

    // ── MUI Pagination ────────────────────────────────────────────────────
    MuiTablePagination: {
      styleOverrides: {
        root:        { fontFamily: FONT_FAMILY, fontSize: FONT_SIZE },
        selectLabel: { fontFamily: FONT_FAMILY, fontSize: FONT_SIZE },
        displayedRows:{ fontFamily: FONT_FAMILY, fontSize: FONT_SIZE },
      },
    },
  },
});

// ── Route guard — wraps protected routes ─────────────────────────────────
// Redirects to /login if no userName is found in sessionStorage.
// /login and /reports/summary remain public (no wrapper needed).
function ProtectedRoute({ children }) {
  const userName = sessionStorage.getItem('userName');
  return userName ? children : <Navigate to="/login" replace />;
}

export default function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <ToastContainer
        position="top-right"
        autoClose={3000}
        hideProgressBar={false}
        style={{ fontFamily: FONT_FAMILY, fontSize: FONT_SIZE }}
      />
      <Routes>
        <Route path="/"                          element={<Navigate to="/raise-problems" replace />} />
        <Route path="/login"                     element={<Login />} />
        <Route path="/raise-problems"            element={<ProtectedRoute><RaiseProblems /></ProtectedRoute>} />
        <Route path="/raise-problems/submission" element={<ProtectedRoute><RaiseProblemsSubmission /></ProtectedRoute>} />
        <Route path="/reports/summary"           element={<SummaryReport />} />
      </Routes>
    </ThemeProvider>
  );
}
