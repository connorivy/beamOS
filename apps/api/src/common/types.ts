import type { User } from "../contracts/user";
import type { NodeSnapshot } from "../nodes/node-entity";
import type { MaterialSnapshot } from "../materials/material-entity";
import type { SectionProfileSnapshot } from "../section-profiles/section-profile-entity";
import type { Element1dSnapshot } from "../element1ds/element1d-entity";
import type { ModelSettingsSnapshot } from "../model-settings/model-settings-entity";
import type { LoadCaseSnapshot } from "../load-cases/load-case-entity";
import type { LoadCombinationSnapshot } from "../load-combinations/load-combination-entity";
import type { PointLoadSnapshot } from "../point-loads/point-load-entity";
import type { AppServices } from "src/services";

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
      }
    | {
          type: "load_case_created";
          payload: LoadCaseSnapshot;
      }
    | {
          type: "load_combination_created";
          payload: LoadCombinationSnapshot;
      }
    | {
          type: "point_load_created";
          payload: PointLoadSnapshot;
      };

export type UserRepository = {
    getById: (id: string) => Promise<User | undefined>;
};

export type ModelRevisionCommitInput = {
    id: string;
    projectId: string;
    branchName?: string;
    parentRevisionId?: string | null;
    secondParentRevisionId?: string | null;
    authorId: string;
    message: string;
    nodes: NodeSnapshot[];
    includeModelChange?: boolean;
};

export type AppContext = {
    requestId: string;
    services: AppServices;
};
