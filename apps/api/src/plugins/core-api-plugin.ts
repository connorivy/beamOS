import { getUser } from "../endpoints/get-user";
import { defineApiPlugin } from "./types";

export const coreApiPlugin = defineApiPlugin({
  id: "core.users",
  endpoints: [getUser],
});
