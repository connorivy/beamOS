import type { ModelAggregate } from "../models/model-aggregate";
import type { AppContext } from "../services/types";

export type ModelVersionTarget = {
  revisionId?: string;
  draftId?: string;
};

export const toVersionRef = (target: ModelVersionTarget) => ({
  revisionId: target.revisionId ?? null,
  draftId: target.draftId ?? null,
});

export const loadModelForTarget = async (input: {
  modelId: string;
  target: ModelVersionTarget;
  ctx: AppContext;
}): Promise<ModelAggregate> => {
  const model = await input.ctx.services.modelRepository.getById({
    modelId: input.modelId,
    revisionId: input.target.revisionId,
    draftId: input.target.draftId,
  });

  if (!model) {
    throw httpError("Model or target revision/draft not found", 404);
  }

  return model;
};

export const httpError = (message: string, status: number): Error => {
  const error = new Error(message) as Error & { status: number };
  error.status = status;
  return error;
};
