import { beforeAll, describe, expect, it } from "bun:test";
import { randomUUID } from "node:crypto";
import { sql } from "drizzle-orm";

if (!process.env.DB_URI) {
  process.env.DB_URI = "postgresql://beamos:beamos@127.0.0.1:5432/beamos";
}

const { bootstrapDb } = await import("../../src/db/bootstrap");
const { db } = await import("../../src/db/client");
const { drizzleModelRepository } = await import("../../src/models/model-repository");
const { drizzleModelVersionRepository } = await import(
  "../../src/model-revisions/model-version-repository"
);
const { ModelAggregate } = await import("../../src/models/model-aggregate");

beforeAll(async () => {
  await db.execute(sql`DROP TABLE IF EXISTS model_branch_heads;`);
  await db.execute(sql`DROP TABLE IF EXISTS model_revision_nodes;`);
  await db.execute(sql`DROP TABLE IF EXISTS model_revisions;`);
  await db.execute(sql`DROP TABLE IF EXISTS nodes;`);
  await db.execute(sql`DROP TABLE IF EXISTS models;`);
  await db.execute(sql`DROP TABLE IF EXISTS users;`);
  await bootstrapDb();
});

describe("model version flow integration", () => {
  it("creates, forks, commits branch nodes, and merges back into main", async () => {
    const modelId = randomUUID();
    const authorId = randomUUID();
    const mainBranch = "main";
    const featureBranch = `feature-${randomUUID().slice(0, 8)}`;

    const baseNode = {
      id: randomUUID(),
      modelId,
      name: "Base Node",
    };
    const model = ModelAggregate.create({
      id: modelId,
      name: "HVAC Layout",
      nodes: [baseNode],
    });

    await drizzleModelRepository.save(model);

    const mainRootRevision = await drizzleModelVersionRepository.commitRevision({
      id: randomUUID(),
      modelId,
      branchName: mainBranch,
      name: model.name,
      parentRevisionId: null,
      secondParentRevisionId: null,
      authorId,
      message: "Initial model revision on main",
      nodes: model.nodes.map((node) => node.toSnapshot()),
    });

    await drizzleModelVersionRepository.createBranch({
      modelId,
      branchName: featureBranch,
      headRevisionId: mainRootRevision.id,
    });

    const branchNodes = [
      ...mainRootRevision.nodes.map((node) => node.toSnapshot()),
      {
        id: randomUUID(),
        modelId,
        name: "Supply Duct A",
      },
      {
        id: randomUUID(),
        modelId,
        name: "Return Duct A",
      },
    ];

    const featureRevision = await drizzleModelVersionRepository.commitRevision({
      id: randomUUID(),
      modelId,
      branchName: featureBranch,
      name: model.name,
      parentRevisionId: mainRootRevision.id,
      secondParentRevisionId: null,
      authorId,
      message: "Add branch duct nodes",
      nodes: branchNodes,
    });

    const mainHeadBeforeMerge = await drizzleModelVersionRepository.getBranchHead(
      modelId,
      mainBranch,
    );
    expect(mainHeadBeforeMerge?.headRevisionId).toBe(mainRootRevision.id);

    const mergeRevision = await drizzleModelVersionRepository.commitRevision({
      id: randomUUID(),
      modelId,
      branchName: mainBranch,
      name: model.name,
      parentRevisionId: mainRootRevision.id,
      secondParentRevisionId: featureRevision.id,
      authorId,
      message: `Merge ${featureBranch} into ${mainBranch}`,
      nodes: featureRevision.nodes.map((node) => node.toSnapshot()),
    });

    model.replaceNodes(mergeRevision.nodes.map((node) => node.toSnapshot()));
    await drizzleModelRepository.save(model);

    const mainHeadAfterMerge = await drizzleModelVersionRepository.getBranchHead(
      modelId,
      mainBranch,
    );
    const featureHeadAfterMerge = await drizzleModelVersionRepository.getBranchHead(
      modelId,
      featureBranch,
    );
    const persistedMergeRevision =
      await drizzleModelVersionRepository.getRevisionById(mergeRevision.id);
    const mergedModel = await drizzleModelRepository.getById(modelId);

    expect(mainHeadAfterMerge?.headRevisionId).toBe(mergeRevision.id);
    expect(featureHeadAfterMerge?.headRevisionId).toBe(featureRevision.id);
    expect(persistedMergeRevision?.parentRevisionId).toBe(mainRootRevision.id);
    expect(persistedMergeRevision?.secondParentRevisionId).toBe(featureRevision.id);
    expect(persistedMergeRevision?.nodes).toHaveLength(3);

    const mergedNodeNames = mergedModel?.nodes.map((node) => node.name).sort();
    expect(mergedNodeNames).toEqual(
      ["Base Node", "Return Duct A", "Supply Duct A"].sort(),
    );
  });
});
