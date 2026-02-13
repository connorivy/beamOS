import {
  pgTable,
  text,
  timestamp,
  primaryKey,
  uuid,
  jsonb,
  integer,
  doublePrecision,
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

export const modelRevisionDrafts = pgTable("model_revision_drafts", {
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
  updatedAt: timestamp("updated_at", {
    withTimezone: true,
    mode: "date",
  })
    .notNull()
    .defaultNow(),
});

export const nodeRevisions = pgTable(
  "node_revisions",
  {
    revisionId: uuid("revision_id")
      .notNull()
      .references(() => modelRevisions.id),
    nodeId: uuid("node_id").notNull(),
    name: text("name").notNull(),
    op: text("op").notNull().default("update"),
  },
  (table) => ({
    pk: primaryKey({ columns: [table.revisionId, table.nodeId] }),
  }),
);

export const nodeRevisionDrafts = pgTable(
  "node_revision_drafts",
  {
    draftId: uuid("draft_id")
      .notNull()
      .references(() => modelRevisionDrafts.id),
    nodeId: uuid("node_id").notNull(),
    name: text("name").notNull(),
    op: text("op").notNull().default("update"),
  },
  (table) => ({
    pk: primaryKey({ columns: [table.draftId, table.nodeId] }),
  }),
);

export const revisionChanges = pgTable("revision_changes", {
  id: uuid("id").primaryKey(),
  revisionId: uuid("revision_id").references(() => modelRevisions.id),
  draftId: uuid("draft_id").references(() => modelRevisionDrafts.id),
  entityType: text("entity_type").notNull(),
  entityId: uuid("entity_id").notNull(),
  schemaVersion: integer("schema_version").notNull().default(1),
  op: text("op").notNull(),
  payload: jsonb("payload").notNull(),
  createdAt: timestamp("created_at", {
    withTimezone: true,
    mode: "date",
  })
    .notNull()
    .defaultNow(),
});

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

export const materials = pgTable("materials", {
  id: uuid("id").primaryKey(),
  revisionId: uuid("revision_id")
    .notNull()
    .references(() => modelRevisions.id),
  pressureESi: doublePrecision("pressure_e_si").notNull(),
  pressureGSi: doublePrecision("pressure_g_si").notNull(),
});

export type Model = InferSelectModel<typeof models>;
export type Material = InferSelectModel<typeof materials>;
