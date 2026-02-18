import { defineEndpoint } from "../contracts/endpoint";
import type { AppContext } from "../common/types";
import { z } from "zod";
import { projectResponseSchema } from "./create-project";
import { projectMapper } from "./project-mapper";

const getProjectsReqSchema = z.object({});

const getProjectsResSchema = z
  .array(projectResponseSchema)
  .meta({ id: "ProjectsArray" });

export const getProjects = defineEndpoint({
  method: "GET",
  path: "/api/projects",
  req: getProjectsReqSchema,
  res: getProjectsResSchema,
  async handler(_req, ctx: AppContext) {
    const projects = await ctx.services.modelRepository.getProjects();
    return projects.map((project) => projectMapper.toResponse(project));
  },
});
