import type { ComponentType } from "react";

export type WebRoute = {
  path: string;
  Component: ComponentType;
};

export type WebPlugin = {
  id: string;
  routes: WebRoute[];
};

export const defineWebPlugin = (plugin: WebPlugin) => plugin;

export const collectPluginRoutes = (plugins: WebPlugin[]): WebRoute[] => {
  return plugins.flatMap((plugin) => plugin.routes);
};
