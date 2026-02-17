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

const normalizePath = (path: string) => path.replace(/\/+$/, "") || "/";

export const routePathMatches = (routePath: string, pathname: string): boolean => {
  const normalizedRoutePath = normalizePath(routePath);
  const normalizedPathname = normalizePath(pathname);

  if (normalizedRoutePath === normalizedPathname) {
    return true;
  }

  const routeSegments = normalizedRoutePath.split("/");
  const pathSegments = normalizedPathname.split("/");
  if (routeSegments.length !== pathSegments.length) {
    return false;
  }

  return routeSegments.every((segment, index) => {
    return segment.startsWith(":") || segment === pathSegments[index];
  });
};
