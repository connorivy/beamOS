import type { User } from "@beamos/contracts";
import type { ModelAggregate } from "../models/model-aggregate";
import type { ModelBranchHeadAggregate } from "../model-branch-heads/model-branch-head-aggregate";
import type { ModelRevisionAggregate } from "../model-revisions/model-revision-aggregate";
import type { NodeAggregate, NodeSnapshot } from "../nodes/node-aggregate";

export type UserRepository = {
  getById: (id: string) => Promise<User | undefined>;
};

export type ModelRepository = {
  getById: (id: string) => Promise<ModelAggregate | undefined>;
  save: (model: ModelAggregate) => Promise<ModelAggregate>;
};

export type NodeRepository = {
  getById: (id: string) => Promise<NodeAggregate | undefined>;
  listByModelId: (modelId: string) => Promise<NodeAggregate[]>;
  save: (node: NodeAggregate) => Promise<NodeAggregate>;
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
  nodes: NodeSnapshot[];
};

export type ModelVersionRepository = {
  getRevisionById: (
    revisionId: string,
  ) => Promise<ModelRevisionAggregate | undefined>;
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
  commitRevision: (
    input: ModelRevisionCommitInput,
  ) => Promise<ModelRevisionAggregate>;
};

export type AppServices = {
  userRepository: UserRepository;
  modelRepository: ModelRepository;
  nodeRepository: NodeRepository;
  modelVersionRepository: ModelVersionRepository;
};

export type AppContext = {
  requestId: string;
  services: AppServices;
};
