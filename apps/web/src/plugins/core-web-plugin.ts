import { HomePage } from "../pages/HomePage";
import { ModelRevisionPage } from "../pages/ModelRevisionPage";
import { ModelsPage } from "../pages/ModelsPage";
import { defineWebPlugin } from "./types";

export const coreWebPlugin = defineWebPlugin({
  id: "core.home",
  routes: [
    {
      path: "/",
      Component: HomePage,
    },
    {
      path: "/models",
      Component: ModelsPage,
    },
    {
      path: "/models/:modelId/:branchName",
      Component: ModelRevisionPage,
    },
  ],
});
