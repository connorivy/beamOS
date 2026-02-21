import { pgTable, text, timestamp, primaryKey, uuid, jsonb, integer } from "drizzle-orm/pg-core";
import type { AnyPgColumn } from "drizzle-orm/pg-core";

export const users = pgTable("users", {
    id: uuid("id").primaryKey(),
    name: text("name").notNull(),
});

export const projects = pgTable("projects", {
    id: uuid("id").primaryKey(),
    name: text("name").notNull(),
    description: text("description").notNull().default(""),
});

export const modelRevisions = pgTable("model_revisions", {
    id: uuid("id").primaryKey(),
    parentRevisionId: uuid("parent_revision_id").references((): AnyPgColumn => modelRevisions.id),
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

export const revisionChanges = pgTable("revision_changes", {
    id: uuid("id").primaryKey(),
    revisionId: uuid("revision_id")
        .notNull()
        .references(() => modelRevisions.id),
    entityType: text("entity_type").notNull(),
    entityId: uuid("entity_id").notNull(),
    schemaVersion: integer("schema_version").notNull(),
    op: text("op").notNull(),
    payload: jsonb("payload"),
});

export const modelBranchHeads = pgTable(
    "model_branch_heads",
    {
        projectId: uuid("project_id")
            .notNull()
            .references(() => projects.id),
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
        pk: primaryKey({ columns: [table.projectId, table.branchName] }),
    }),
);
