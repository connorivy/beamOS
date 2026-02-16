import { additionalWebPlugins } from "./additional-plugins";
import { coreWebPlugin } from "./core-web-plugin";

export const webPlugins = [coreWebPlugin, ...additionalWebPlugins];