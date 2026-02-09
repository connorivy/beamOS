import { HomePage } from "../routes/HomePage";
import { defineWebPlugin } from "./types";

export const coreWebPlugin = defineWebPlugin({
  id: "core.home",
  routes: [
    {
      path: "/",
      Component: HomePage,
    },
  ],
});
