import axios from 'axios';

// Determine API base dynamically based on the URL the user is currently visiting
const getApiBase = () => {
  // If running locally in development (Vite dev server), use proxy
  if (window.location.hostname === 'localhost') {
    return '';
  }
  
  // Otherwise, automatically use the same IP/Domain the user typed in the browser
  // Example: visiting http://192.168.60.224/CTSUI -> returns http://192.168.60.224/CTSAPI
  // Example: visiting http://10.42.4.127/CTSUI -> returns http://10.42.4.127/CTSAPI
  return `${window.location.origin}/CTSAPI`;
};

const API_BASE = getApiBase();

const api = axios.create({
  baseURL: API_BASE,
});

export default api;
