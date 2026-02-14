import { createModel } from "src/models/create-model";
import { patchModel } from "../models/patch-model";
import { defineApiPlugin } from "./types";
import { createNode } from "src/nodes/create-node";
import { patchNode } from "src/nodes/patch-node";
import { batchCreateMaterial } from "src/materials/batch-create-material";
import { getMaterial } from "src/materials/get-material";
import { batchCreateSectionProfile } from "src/section-profiles/batch-create-section-profile";
import { getSectionProfile } from "src/section-profiles/get-section-profile";

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
  ],
});
