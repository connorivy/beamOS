import { z } from "zod";
import type { AppServices } from "src/common/types";

export const modelsListResultSchema = z.object({
  models: z.array(
    z.object({
      id: z.string().uuid(),
      name: z.string(),
      description: z.string(),
      lastModified: z.string().datetime().nullable(),
      role: z.enum(["Owner", "Contributor", "Reviewer"]),
    }),
  ),
});

export class TrpcProcedureNotFoundError extends Error {
  readonly status = 404;
}

export async function handleTrpcProcedure(
  procedure: string,
  services: AppServices,
) {
  const handlers = {
    "models.list": async () => {
      const models = await services.modelRepository.getUserModels();
      return modelsListResultSchema.parse({
        models: models.map((model) => ({
          id: model.id,
          name: model.name,
          description: model.description,
          lastModified: model.lastModified?.toISOString() ?? null,
          role: model.role,
        })),
      });
    },
  } as const;

  const handler = handlers[procedure as keyof typeof handlers];
  if (!handler) {
    throw new TrpcProcedureNotFoundError("Not found");
  }
  return handler();
}
