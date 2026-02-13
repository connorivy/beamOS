import type { Endpoint } from "@beamos/contracts";
import type { AppContext, AppServices } from "../common/types";

export type ApiPlugin = {
  id: string;
  endpoints: Endpoint<any, any, AppContext>[];
  extendServices?: (services: AppServices) => AppServices;
};

export const defineApiPlugin = (plugin: ApiPlugin) => plugin;

export const collectPluginEndpoints = (
  plugins: ApiPlugin[],
): Endpoint<any, any, AppContext>[] => {
  return plugins.flatMap((plugin) => plugin.endpoints);
};

export const buildServices = (
  baseServices: AppServices,
  plugins: ApiPlugin[],
): AppServices => {
  return plugins.reduce((services, plugin) => {
    if (!plugin.extendServices) {
      return services;
    }

    return plugin.extendServices(services);
  }, baseServices);
};
