import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  // Base path must match the IIS virtual directory for the UI
  base: '/CTSUI/',

  plugins: [
    react(),
    {
      name: 'handle-post-to-spa',
      configureServer(server) {
        server.middlewares.use((req, res, next) => {
          // If a POST request is sent to static frontend routes (e.g. from legacy dashboard postbacks),
          // rewrite the method to GET so Vite serves the index.html page instead of returning HTTP 405.
          if (req.method === 'POST' && req.url.startsWith('/CTSUI') && !req.url.includes('/api/')) {
            req.method = 'GET';
          }
          next();
        });
      }
    }
  ],

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
