import { relations } from "drizzle-orm";
import {
  pgTable,
  text,
  timestamp,
  primaryKey,
  uuid,
} from "drizzle-orm/pg-core";
import type { InferSelectModel } from "drizzle-orm";
import type { AnyPgColumn } from "drizzle-orm/pg-core";

export const users = pgTable("users", {
  id: uuid("id").primaryKey(),
  name: text("name").notNull(),
});

export const models = pgTable("models", {
  id: uuid("id").primaryKey(),
  name: text("name").notNull(),
});
export const modelRelations = relations(models, ({ many }) => ({
  nodes: many(nodes),
  revisions: many(modelRevisions),
  branchHeads: many(modelBranchHeads),
}));

export const nodes = pgTable("nodes", {
  id: uuid("id").primaryKey(),
  modelId: uuid("model_id")
    .notNull()
    .references(() => models.id),
  name: text("name").notNull(),
});
export const nodeRelations = relations(nodes, ({ one }) => ({
  model: one(models, {
    fields: [nodes.modelId],
    references: [models.id],
  }),
}));

export const modelRevisions = pgTable("model_revisions", {
  id: uuid("id").primaryKey(),
  modelId: uuid("model_id")
    .notNull()
    .references(() => models.id),
  modelName: text("model_name").notNull(),
  parentRevisionId: uuid("parent_revision_id").references(
    (): AnyPgColumn => modelRevisions.id,
  ),
  secondParentRevisionId: uuid("second_parent_revision_id").references(
    (): AnyPgColumn => modelRevisions.id,
  ),
  authorId: uuid("author_id").notNull(),
  message: text("message").notNull(),
  createdAt: timestamp("created_at", {
    withTimezone: true,
    mode: "date",
  })
    .notNull()
    .defaultNow(),
});
export const modelRevisionRelations = relations(
  modelRevisions,
  ({ one, many }) => ({
    model: one(models, {
      fields: [modelRevisions.modelId],
      references: [models.id],
    }),
    parentRevision: one(modelRevisions, {
      fields: [modelRevisions.parentRevisionId],
      references: [modelRevisions.id],
      relationName: "revision_parent",
    }),
    secondParentRevision: one(modelRevisions, {
      fields: [modelRevisions.secondParentRevisionId],
      references: [modelRevisions.id],
      relationName: "revision_second_parent",
    }),
    nodes: many(modelRevisionNodes),
  }),
);

export const modelRevisionNodes = pgTable(
  "model_revision_nodes",
  {
    revisionId: uuid("revision_id")
      .notNull()
      .references(() => modelRevisions.id),
    nodeId: uuid("node_id").notNull(),
    name: text("name").notNull(),
  },
  (table) => ({
    pk: primaryKey({ columns: [table.revisionId, table.nodeId] }),
  }),
);
export const modelRevisionNodeRelations = relations(
  modelRevisionNodes,
  ({ one }) => ({
    revision: one(modelRevisions, {
      fields: [modelRevisionNodes.revisionId],
      references: [modelRevisions.id],
    }),
  }),
);

export const modelBranchHeads = pgTable(
  "model_branch_heads",
  {
    modelId: uuid("model_id")
      .notNull()
      .references(() => models.id),
    branchName: text("branch_name").notNull(),
    headRevisionId: uuid("head_revision_id")
      .notNull()
      .references(() => modelRevisions.id),
    updatedAt: timestamp("updated_at", {
      withTimezone: true,
      mode: "date",
    })
      .notNull()
      .defaultNow(),
  },
  (table) => ({
    pk: primaryKey({ columns: [table.modelId, table.branchName] }),
  }),
);
export const modelBranchHeadRelations = relations(
  modelBranchHeads,
  ({ one }) => ({
    model: one(models, {
      fields: [modelBranchHeads.modelId],
      references: [models.id],
    }),
    headRevision: one(modelRevisions, {
      fields: [modelBranchHeads.headRevisionId],
      references: [modelRevisions.id],
    }),
  }),
);

export type Model = InferSelectModel<typeof models>;
export type Node = InferSelectModel<typeof nodes>;
