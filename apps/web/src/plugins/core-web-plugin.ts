import { HomePage } from "../pages/HomePage";
import { ModelRevisionEditorPage } from "../pages/ModelRevisionEditorPage";
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
      path: "/editor/projects/:projectId/:branchName",
      Component: ModelRevisionEditorPage,
    },
  ],
});
