import { createModel } from "src/models/create-model";
import { getUser } from "../endpoints/get-user";
import { patchModel } from "../models/patch-model";
import { defineApiPlugin } from "./types";
import { createNode } from "src/nodes/create-node";
import { patchNode } from "src/nodes/patch-node";

export const coreApiPlugin = defineApiPlugin({
  id: "core.users",
  endpoints: [getUser, createModel, createNode, patchNode, patchModel],
});
