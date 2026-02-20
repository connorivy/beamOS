import { defineEndpoint } from "../contracts/endpoint";
import { ProjectEntity } from "./project-aggregate";
import type { AppContext } from "../common/types";
import { z } from "zod";
import { projectMapper as projectMapper } from "./project-mapper";
import { uuidV7Schema } from "src/common/uuid";
import { modelSettingsPropertiesSchema } from "src/model-settings/model-settings-contract-schemas";

const createProjectReqSchema = z.object({
  body: z
    .object({
      id: uuidV7Schema.optional(),
      name: z.string().min(1),
      description: z.string().min(1),
      modelSettings: modelSettingsPropertiesSchema,
    })
    .meta({ id: "CreateProjectRequest" }),
});

export const projectResponseSchema = z
  .object({
    id: uuidV7Schema,
    name: z.string(),
    description: z.string(),
    lastModified: z.string().datetime(),
    role: z.enum(["Owner", "Contributor", "Reviewer"]),
  })
  .meta({ id: "Project" });

export const createProject = defineEndpoint({
  method: "POST",
  path: "/api/projects",
  req: createProjectReqSchema,
  res: projectResponseSchema,
  async handler(req, ctx: AppContext) {
    const project = ProjectEntity.create({
      id: req.body.id,
      name: req.body.name,
      description: req.body.description,
    });
    const createdProject = await ctx.services.modelRepository.create({
      model: project,
      message: req.body.description,
      modelSettings: req.body.modelSettings,
    });
    return projectMapper.toResponse(createdProject);
  },
});
