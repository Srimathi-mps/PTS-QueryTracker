// import React from 'react';
// import ReactDOM from 'react-dom/client';
// import { BrowserRouter } from 'react-router-dom';
// import { CssBaseline } from '@mui/material';
// import App from './App';
// import 'react-toastify/dist/ReactToastify.css';

// // ── SPA redirect restore (works without IIS URL Rewrite) ──────────────────────
// // When IIS 404s a deep link, 404.html saves the route to sessionStorage
// // and redirects here. We read it back and replace the current history entry.
// (function restoreSpaRedirect() {
//   const redirect = sessionStorage.getItem('spa_redirect');
//   if (redirect) {
//     sessionStorage.removeItem('spa_redirect');
//     // Replace current history entry so the back button works correctly
//     window.history.replaceState(null, '', '/CTSUI' + redirect);
//   }
// })();

// ReactDOM.createRoot(document.getElementById('root')).render(
//   <React.StrictMode>
//     <BrowserRouter basename="/CTSUI">
//       <CssBaseline />
//       <App />
//     </BrowserRouter>
//   </React.StrictMode>
// );
import React from 'react';
import ReactDOM from 'react-dom/client';
import { HashRouter } from 'react-router-dom';
import { CssBaseline } from '@mui/material';
import App from './App';
import 'react-toastify/dist/ReactToastify.css';

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <HashRouter>
      <CssBaseline />
      <App />
    </HashRouter>
  </React.StrictMode>
);