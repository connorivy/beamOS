export type ModelVersionTarget = {
  revisionId?: string;
};

export const toVersionRef = (target: ModelVersionTarget) => ({
  revisionId: target.revisionId ?? null,
});
