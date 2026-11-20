import React, { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import {
  Box, Paper, Typography, TextField, Button, Alert, CircularProgress,
} from '@mui/material';
import api from '../utils/api';

export default function Login() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  // Pre-fill username if passed via slash-style params before the hash:
  //   ?Username=TUFDMDE0Mzkz-.../BookID=461997_2_En/#/login
  const preHashParams = (() => {
    const result = {};
    const raw = (window.location.search || '').replace(/^\?/, '');
    raw.split('/').forEach((part) => {
      const eq = part.indexOf('=');
      if (eq === -1) return;
      const key = part.slice(0, eq).trim();
      const val = decodeURIComponent(part.slice(eq + 1).trim());
      if (key) result[key] = val;
    });
    return result;
  })();

  const [userName, setUserName] = useState(
    searchParams.get('username') ||
    preHashParams['Username'] ||
    preHashParams['username'] ||
    ''
  );
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleLogin = async (e) => {
    e.preventDefault();
    if (!userName.trim()) { setError('Please enter username'); return; }
    setLoading(true);
    setError('');
    try {
      const res = await api.post('/api/auth/login', { userName, password });
      // Use the userName returned by the server (already uppercased)
      const authenticatedUser = res.data?.userName || userName.trim().toUpperCase();
      sessionStorage.setItem('userName', authenticatedUser);
      navigate('/raise-problems');
    } catch (err) {
      setError(err.response?.data?.message || 'Login failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      display="flex" justifyContent="center" alignItems="center"
      minHeight="100vh" sx={{ background: '#dbeaf9' }}
    >
      <Paper elevation={4} sx={{ p: 4, width: 360, borderRadius: 2 }}>
        <Typography variant="h6" fontWeight="bold" mb={3} textAlign="center">
          Communication Tracker
        </Typography>

        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

        <form onSubmit={handleLogin}>
          <TextField
            fullWidth label="Username" size="small" sx={{ mb: 2 }}
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            autoComplete="username"
            autoFocus
          />
          <TextField
            fullWidth label="Password" type="password" size="small" sx={{ mb: 3 }}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete="current-password"
          />
          <Button
            type="submit" variant="contained" fullWidth
            disabled={loading}
          >
            {loading ? <CircularProgress size={20} color="inherit" /> : 'Login'}
          </Button>
        </form>
      </Paper>
    </Box>
  );
}
