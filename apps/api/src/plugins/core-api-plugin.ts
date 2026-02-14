import { createModel } from "src/models/create-model";
import { patchModel } from "../models/patch-model";
import { defineApiPlugin } from "./types";
import { createNode } from "src/nodes/create-node";
import { patchNode } from "src/nodes/patch-node";
import { batchCreateMaterial } from "src/materials/batch-create-material";
import { getMaterial } from "src/materials/get-material";
import { batchCreateSectionProfile } from "src/section-profiles/batch-create-section-profile";
import { getSectionProfile } from "src/section-profiles/get-section-profile";
import { batchCreateElement1d } from "src/element1ds/batch-create-element1d";
import { getElement1d } from "src/element1ds/get-element1d";
import { getModelRevision } from "src/model-revisions/get-model-revision";

export const coreApiPlugin = defineApiPlugin({
  id: "core.users",
  endpoints: [
    createModel,
    createNode,
    patchNode,
    patchModel,
    batchCreateMaterial,
    getMaterial,
    batchCreateSectionProfile,
    getSectionProfile,
    batchCreateElement1d,
    getElement1d,
    getModelRevision,
  ],
});
