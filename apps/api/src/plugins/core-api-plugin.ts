import { createProject } from "src/projects/create-project";
import { getProjects } from "src/projects/get-project";
import { patchModel } from "../projects/patch-project";
import { defineApiPlugin } from "./types";
import { patchNode } from "src/nodes/patch-node";
import { batchCreateNode } from "src/nodes/batch-create-node";
import { batchCreateMaterial } from "src/materials/batch-create-material";
import { batchPutMaterial } from "src/materials/batch-put-material";
import { getMaterial } from "src/materials/get-material";
import { putModelSettings } from "src/model-settings/put-model-settings";
import { batchCreateSectionProfile } from "src/section-profiles/batch-create-section-profile";
import { getSectionProfile } from "src/section-profiles/get-section-profile";
import { batchCreateElement1d } from "src/element1ds/batch-create-element1d";
import { getElement1d } from "src/element1ds/get-element1d";
import { getModelRevision } from "src/model-revisions/get-model-revision";
import { createModelRevision } from "src/model-revisions/create-model-revision";

export const coreApiPlugin = defineApiPlugin({
  id: "core.users",
  endpoints: [
    createProject,
    getProjects,
    patchNode,
    batchCreateNode,
    patchModel,
    batchCreateMaterial,
    batchPutMaterial,
    putModelSettings,
    getMaterial,
    batchCreateSectionProfile,
    getSectionProfile,
    batchCreateElement1d,
    getElement1d,
    getModelRevision,
    createModelRevision,
  ],
});
