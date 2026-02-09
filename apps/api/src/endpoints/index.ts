import { collectPluginEndpoints } from "../plugins/types";
import { apiPlugins } from "../plugins/registry";

export const endpoints = collectPluginEndpoints(apiPlugins);
