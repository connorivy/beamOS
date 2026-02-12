import type { User } from "@beamos/contracts";
import type { ModelAggregate } from "../models/model-aggregate";
import type { ModelBranchHeadAggregate } from "../model-branch-heads/model-branch-head-aggregate";
import type { ModelRevisionAggregate } from "../model-revisions/model-revision-aggregate";
import type { ModelRevisionDraftAggregate } from "../model-revision-drafts/model-revision-draft-aggregate";

export type UserRepository = {
  getById: (id: string) => Promise<User | undefined>;
};

export type ModelRepository = {
  getById: (input: {
    modelId: string;
    revisionId?: string;
    draftId?: string;
  }) => Promise<ModelAggregate | undefined>;
  save: (model: ModelAggregate) => Promise<ModelAggregate>;
};

export type ModelRevisionCommitInput = {
  id: string;
  modelId: string;
  branchName?: string;
  name: string;
  parentRevisionId?: string | null;
  secondParentRevisionId?: string | null;
  authorId: string;
  message: string;
  nodes: { nodeId: string; name: string; op?: "upsert" | "delete" }[];
};

export type ModelRevisionDraftInput = {
  id: string;
  modelId: string;
  name: string;
  parentRevisionId?: string | null;
  secondParentRevisionId?: string | null;
  authorId: string;
  message: string;
  nodes: { nodeId: string; name: string; op?: "upsert" | "delete" }[];
};

export type ModelRevisionRepository = {
  getRevisionById: (
    revisionId: string,
  ) => Promise<ModelRevisionAggregate | undefined>;
  getDraftById: (
    draftId: string,
  ) => Promise<ModelRevisionDraftAggregate | undefined>;
  getBranchHead: (
    modelId: string,
    branchName: string,
  ) => Promise<ModelBranchHeadAggregate | undefined>;
  listBranchHeads: (modelId: string) => Promise<ModelBranchHeadAggregate[]>;
  createBranch: (input: {
    modelId: string;
    branchName: string;
    headRevisionId: string;
  }) => Promise<void>;
  saveDraft: (
    input: ModelRevisionDraftInput,
  ) => Promise<ModelRevisionDraftAggregate>;
  commitDraft: (input: {
    draftId: string;
    branchName?: string;
  }) => Promise<ModelRevisionAggregate>;
  commitRevision: (
    input: ModelRevisionCommitInput,
  ) => Promise<ModelRevisionAggregate>;
};

export type AppServices = {
  userRepository: UserRepository;
  modelRepository: ModelRepository;
  modelRevisionRepository: ModelRevisionRepository;
};

export type AppContext = {
  requestId: string;
  services: AppServices;
};
