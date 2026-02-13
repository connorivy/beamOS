import type { Endpoint } from "@beamos/contracts";
import type { z } from "zod";
import type { AppServices } from "../common/types";

type AnyEndpoint = Endpoint<z.ZodType<any, any, any>, z.ZodType<any, any, any>, any>;

export type ApiPlugin = {
  id: string;
  endpoints: AnyEndpoint[];
  extendServices?: (services: AppServices) => AppServices;
};

export const defineApiPlugin = (plugin: ApiPlugin) => plugin;

export const collectPluginEndpoints = (
  plugins: ApiPlugin[],
): AnyEndpoint[] => {
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
