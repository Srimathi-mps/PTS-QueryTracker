import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  // Base path must match the IIS virtual directory for the UI
  base: '/CTSUI/',

  plugins: [react()],

  server: {
    port: 5173,
    proxy: {
      '/api': {
        // During local dev, proxy to your local dotnet run port
        target: 'https://localhost:61496',
        changeOrigin: true,
        secure: false,                      // allow self-signed dev cert
        rewrite: (path) => path,
      },
    },
  },
});
