import { and, eq } from "drizzle-orm";
import { db } from "./client";
import {
  modelBranchHeads,
  modelRevisions,
  models,
  users,
} from "./schema";

const USER_ID = "00000000-0000-4000-8000-000000000001";
const MODEL_ID = "10000000-0000-4000-8000-000000000001";
const INITIAL_REVISION_ID = "30000000-0000-4000-8000-000000000001";
const SYSTEM_AUTHOR_ID = "40000000-0000-4000-8000-000000000001";

export const ensureSeedData = async () => {
  const existing = await db
    .select()
    .from(users)
    .where(eq(users.id, USER_ID))
    .limit(1);
  if (existing.length === 0) {
    await db.insert(users).values({ id: USER_ID, name: "Ada Lovelace" });
  }

  const existingModel = await db
    .select()
    .from(models)
    .where(eq(models.id, MODEL_ID))
    .limit(1);

  if (existingModel.length === 0) {
    await db.insert(models).values({
      id: MODEL_ID,
      name: "Default Model",
    });
  }


  const mainHead = await db
    .select()
    .from(modelBranchHeads)
    .where(
      and(
        eq(modelBranchHeads.modelId, MODEL_ID),
        eq(modelBranchHeads.branchName, "main"),
      ),
    )
    .limit(1);

  if (mainHead.length > 0) {
    return;
  }

  const [modelRow] = await db
    .select()
    .from(models)
    .where(eq(models.id, MODEL_ID))
    .limit(1);

  if (!modelRow) {
    return;
  }

  await db.insert(modelRevisions).values({
    id: INITIAL_REVISION_ID,
    modelId: modelRow.id,
    modelName: modelRow.name,
    parentRevisionId: null,
    secondParentRevisionId: null,
    authorId: SYSTEM_AUTHOR_ID,
    message: "Initialize main branch",
  });


  await db.insert(modelBranchHeads).values({
    modelId: modelRow.id,
    branchName: "main",
    headRevisionId: INITIAL_REVISION_ID,
  });
};
