import { z } from "zod";
import type { AppServices } from "src/common/types";

export const modelsListResultSchema = z.object({
  models: z.array(
    z.object({
      id: z.uuid(),
      name: z.string(),
      description: z.string(),
      lastModified: z.string().datetime().nullable(),
      role: z.enum(["Owner", "Contributor", "Reviewer"]),
    }),
  ),
});

export async function handleTrpcProcedure(
  procedure: string,
  services: AppServices,
) {
  if (procedure !== "models.list") {
    const error = new Error("Not found");
    (error as Error & { status?: number }).status = 404;
    throw error;
  }

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
}
