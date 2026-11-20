import axios from 'axios';

// In dev:  VITE_API_BASE is empty → calls go to Vite proxy → localhost dotnet
// In prod: VITE_API_BASE = 'http://10.42.4.127/CTSAPI' → calls go directly to IIS API app
const API_BASE = import.meta.env.VITE_API_BASE ?? '';

const api = axios.create({
  baseURL: API_BASE,
});

export default api;
