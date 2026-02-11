import { runEndpoints } from "./lib/endpoint-runtime";
import { apiPlugins } from "./plugins/registry";
import { buildServices, collectPluginEndpoints } from "./plugins/types";
import { createDefaultServices } from "./services/default-services";

const endpoints = collectPluginEndpoints(apiPlugins);
const services = buildServices(createDefaultServices(), apiPlugins);

export const createServer = () => {
  return Bun.serve({
    port: Number(process.env.PORT ?? 3001),
    fetch: async (req) => {
      const url = new URL(req.url);

      if (url.pathname === "/health") {
        return Response.json({
          ok: true,
          plugins: apiPlugins.map((plugin) => plugin.id),
        });
      }

      const requestId = crypto.randomUUID();
      return runEndpoints(req, endpoints, { requestId, services });
    },
  });
};
