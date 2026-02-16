import type { User } from "../contracts/user";
import type { ModelBranchHeadAggregate } from "../model-branch-heads/model-branch-head-aggregate";
import type { ModelRevisionAggregate } from "../model-revisions/model-revision-aggregate";
import type { NodeSnapshot } from "../nodes/node-entity";
import { ModelRepository } from "src/models/model-repository";
import type { MaterialRepository } from "../materials/material-repository";
import type { SectionProfileRepository } from "../section-profiles/section-profile-repository";
import type { Element1dRepository } from "../element1ds/element1d-repository";
import type { RevisionChangeRepository } from "../revision-changes/revision-change-repository";
import type { DbTransaction } from "../db/client";
import type { MaterialSnapshot } from "../materials/material-entity";
import type { SectionProfileSnapshot } from "../section-profiles/section-profile-entity";
import type { Element1dSnapshot } from "../element1ds/element1d-entity";
import type { ModelSettingsSnapshot } from "../model-settings/model-settings-entity";

export type DomainEvent =
  | {
      type: "node_created";
      payload: NodeSnapshot;
    }
  | {
      type: "node_deleted";
      payload: NodeSnapshot;
    }
  | {
      type: "material_created";
      payload: MaterialSnapshot;
    }
  | {
      type: "section_profile_created";
      payload: SectionProfileSnapshot;
    }
  | {
      type: "element1d_created";
      payload: Element1dSnapshot;
    }
  | {
      type: "model_settings_created";
      payload: ModelSettingsSnapshot;
    };

export type UserRepository = {
  getById: (id: string) => Promise<User | undefined>;
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
  includeModelChange?: boolean;
};

export type ModelRevisionRepository = {
  getRevisionById: (
    revisionId: string,
  ) => Promise<ModelRevisionAggregate | undefined>;
  save: (input: {
    revision: ModelRevisionAggregate;
    newRevision?: boolean;
    tx?: DbTransaction;
  }) => Promise<ModelRevisionAggregate>;
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
  modelRevisionRepository: ModelRevisionRepository;
  materialRepository: MaterialRepository;
  sectionProfileRepository: SectionProfileRepository;
  element1dRepository: Element1dRepository;
  revisionChangeRepository: RevisionChangeRepository;
};

export type AppContext = {
  requestId: string;
  services: AppServices;
};
