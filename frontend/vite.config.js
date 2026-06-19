import { defineConfig } from 'vite';

export default defineConfig({
  root: 'app',
  server: {
    fs: {
      allow: ['..'],
    },
  },
  build: {
    outDir: '../dist',
    emptyOutDir: true,
  },
});