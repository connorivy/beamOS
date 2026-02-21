import { and, eq } from "drizzle-orm";
import { getDb } from "./client";
import { modelBranchHeads, modelRevisions, projects, users } from "./schema";

const USER_ID = "00000000-0000-4000-8000-000000000001";
const PROJECT_ID = "10000000-0000-4000-8000-000000000001";
const INITIAL_REVISION_ID = "30000000-0000-4000-8000-000000000001";
const SYSTEM_AUTHOR_ID = "40000000-0000-4000-8000-000000000001";

export const ensureSeedData = async () => {
  const existing = await getDb()
    .select()
    .from(users)
    .where(eq(users.id, USER_ID))
    .limit(1);
  if (existing.length === 0) {
    await getDb().insert(users).values({ id: USER_ID, name: "Ada Lovelace" });
  }

  const existingModel = await getDb()
    .select()
    .from(projects)
    .where(eq(projects.id, PROJECT_ID))
    .limit(1);

  if (existingModel.length === 0) {
    await getDb().insert(projects).values({
      id: PROJECT_ID,
      name: "Default Model",
      description: "",
    });
  }

  const mainHead = await getDb()
    .select()
    .from(modelBranchHeads)
    .where(
      and(
        eq(modelBranchHeads.projectId, PROJECT_ID),
        eq(modelBranchHeads.branchName, "main"),
      ),
    )
    .limit(1);

  if (mainHead.length > 0) {
    return;
  }

  const [projectRow] = await getDb()
    .select()
    .from(projects)
    .where(eq(projects.id, PROJECT_ID))
    .limit(1);

  if (!projectRow) {
    return;
  }

  await getDb().insert(modelRevisions).values({
    id: INITIAL_REVISION_ID,
    parentRevisionId: null,
    secondParentRevisionId: null,
    authorId: SYSTEM_AUTHOR_ID,
    message: "Initialize main branch",
  });

  await getDb().insert(modelBranchHeads).values({
    projectId: projectRow.id,
    branchName: "main",
    headRevisionId: INITIAL_REVISION_ID,
  });
};
