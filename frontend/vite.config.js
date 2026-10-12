import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// Vite dev-server proxy: the React app calls "/api/..." and Vite
// forwards it to json-server on port 5000. In Lecture 03 this same
// pattern points at the real ASP.NET Core API instead.
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/api": {
        target: "http://localhost:5000",
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api/, ""),
      },
    },
  },
});
