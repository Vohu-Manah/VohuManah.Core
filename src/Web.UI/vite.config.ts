import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    watch: {
      usePolling: true,
    },
    host: true,
    proxy: {
      '/library': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
      
      
      
      '/uploads': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
      '/reports': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
    },
    strictPort: true,
    port: process.env.PORT
      ? parseInt(process.env.PORT, 10)
      : process.env.VITE_PORT
        ? parseInt(process.env.VITE_PORT, 10)
        : 5173,
  },
  optimizeDeps: {
    include: ['qrcode.react', 'mammoth'],
    esbuildOptions: {
      target: 'esnext',
    },
  },
});
