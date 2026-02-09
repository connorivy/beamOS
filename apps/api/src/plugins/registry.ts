import { additionalApiPlugins } from "./additional-plugins";
import { coreApiPlugin } from "./core-api-plugin";

export const apiPlugins = [coreApiPlugin, ...additionalApiPlugins];
