export type ModelVersionTarget = {
  revisionId?: string;
  draftId?: string;
};

export const toVersionRef = (target: ModelVersionTarget) => ({
  revisionId: target.revisionId ?? null,
  draftId: target.draftId ?? null,
});
