/**
 * Thin wrapper around a toast library.
 * Swap the implementation here without touching any component.
 * Using react-toastify as the modern equivalent of the legacy toastr.js.
 *
 * Install: npm install react-toastify
 * Add <ToastContainer /> once in App.jsx.
 */
import { toast } from 'react-toastify';

export const notify = {
  success: (message) => toast.success(message, { position: 'top-right' }),
  error: (message) => toast.error(message, { position: 'top-right' }),
  info: (message) => toast.info(message, { position: 'top-right' }),
};
