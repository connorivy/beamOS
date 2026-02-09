import type { Endpoint } from "@beamos/contracts";

export type ApiPlugin = {
  id: string;
  endpoints: Endpoint<any, any>[];
};

export const defineApiPlugin = (plugin: ApiPlugin) => plugin;

export const collectPluginEndpoints = (
  plugins: ApiPlugin[],
): Endpoint<any, any>[] => {
  return plugins.flatMap((plugin) => plugin.endpoints);
};
