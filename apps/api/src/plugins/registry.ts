import { coreApiPlugin } from "./core-api-plugin";
import type { ApiPlugin } from "./types";

const loadAdditionalApiPlugins = async (): Promise<ApiPlugin[]> => {
  const modulePath =
    process.env.BEAMOS_API_ADDITIONAL_PLUGINS_MODULE ?? "./additional-plugins";
  const module = await import(modulePath);
  return module.additionalApiPlugins;
};

const additionalApiPlugins = await loadAdditionalApiPlugins();

export const apiPlugins = [coreApiPlugin, ...additionalApiPlugins];
