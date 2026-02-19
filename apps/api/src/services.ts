import { drizzleProjectRepository, ProjectRepository } from "./projects/project-repository";
import {
    drizzleModelRevisionRepository,
    ModelRevisionRepository,
} from "./model-revisions/model-revision-repository";

export const createDefaultServices = (): AppServices => ({
    modelRepository: drizzleProjectRepository,
    modelRevisionRepository: drizzleModelRevisionRepository,
});

export type AppServices = {
    modelRepository: ProjectRepository;
    modelRevisionRepository: ModelRevisionRepository;
};
