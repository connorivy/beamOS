import { resolve } from "node:path";
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

const isEnterpriseEdition = process.env.BEAMOS_EDITION === "enterprise";
const apiProxyTarget =
  process.env.VITE_API_PROXY_TARGET ?? "http://127.0.0.1:3001";

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "./additional-plugins": isEnterpriseEdition
        ? resolve(
            __dirname,
            "../../enterprise/api/src/plugins/additional-plugins.ts",
          )
        : resolve(__dirname, "./src/plugins/additional-plugins.ts"),
    },
  },
  server: {
    host: "127.0.0.1",
    port: 5173,
    proxy: {
      "/api": {
        target: apiProxyTarget,
        changeOrigin: true,
      },
    },
  },
});
