import { createProject } from "src/projects/create-project";
import { getProjects } from "src/projects/get-project";
import { patchModel } from "../projects/patch-project";
import { forkProject } from "src/projects/fork-project";
import { defineApiPlugin } from "./types";
import { getMaterial } from "src/materials/get-material";
import { getSectionProfile } from "src/section-profiles/get-section-profile";
import { getElement1d } from "src/element1ds/get-element1d";
import { getModelRevision } from "src/model-revisions/get-model-revision";
import { createModelRevision } from "src/model-revisions/create-model-revision";

export const coreApiPlugin = defineApiPlugin({
    id: "core.users",
    endpoints: [
        createProject,
        getProjects,
        patchModel,
        forkProject,
        getMaterial,
        getSectionProfile,
        getElement1d,
        getModelRevision,
        createModelRevision,
    ],
});
