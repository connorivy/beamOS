import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";
import path from "node:path";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");

  const apiProxyTarget =
    env.VITE_API_PROXY_TARGET ??
    process.env.VITE_API_PROXY_TARGET ??
    "http://127.0.0.1:3001";

  const additionalWebPluginsModule =
    env.BEAMOS_WEB_ADDITIONAL_PLUGINS_MODULE ??
    process.env.BEAMOS_WEB_ADDITIONAL_PLUGINS_MODULE ??
    "./src/plugins/additional-plugins.ts";
  const resolvedAdditionalWebPluginsModule =
    additionalWebPluginsModule.startsWith(".") ||
    additionalWebPluginsModule.startsWith("/")
      ? path.resolve(process.cwd(), additionalWebPluginsModule)
      : additionalWebPluginsModule;

  return {
    plugins: [react()],
    resolve: {
      alias: {
        "./additional-plugins": resolvedAdditionalWebPluginsModule,
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
  };
});
