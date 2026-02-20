import type { CreateModelRevisionRequest, CreateProjectRequest } from "@beamos/openapi-client";
import { kassimaliExample3_8Project } from "./Kassimali_Example3_8.fixture";
import {
    kassimaliExample8_4Project,
    kassimaliExample8_4Revision,
} from "./Kassimali_Example8_4.fixture";

export type SampleProjectFixture = {
    project: CreateProjectRequest & { id: string };
    model: CreateModelRevisionRequest;
};

export const sampleProjectFixtures: SampleProjectFixture[] = [
    {
        project: kassimaliExample3_8Project.project,
        model: kassimaliExample3_8Project.model,
    },
    {
        project: kassimaliExample8_4Project,
        model: kassimaliExample8_4Revision,
    },
];

export * from "./Kassimali_Example3_8.fixture";
export * from "./Kassimali_Example8_4.fixture";
