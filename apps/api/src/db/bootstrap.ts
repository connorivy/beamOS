import { sql } from "drizzle-orm";
import { db } from "./client";

export const bootstrapDb = async () => {
  await db.execute(sql`
    CREATE TABLE IF NOT EXISTS users (
      id UUID PRIMARY KEY NOT NULL,
      name TEXT NOT NULL
    );
  `);

  await db.execute(sql`
    CREATE TABLE IF NOT EXISTS models (
      id UUID PRIMARY KEY NOT NULL,
      name TEXT NOT NULL
    );
  `);

  await db.execute(sql`
    CREATE TABLE IF NOT EXISTS nodes (
      id UUID PRIMARY KEY NOT NULL,
      model_id UUID NOT NULL REFERENCES models(id),
      name TEXT NOT NULL
    );
  `);

  await db.execute(sql`
    CREATE TABLE IF NOT EXISTS model_revisions (
      id UUID PRIMARY KEY NOT NULL,
      model_id UUID NOT NULL REFERENCES models(id),
      model_name TEXT NOT NULL,
      parent_revision_id UUID REFERENCES model_revisions(id),
      second_parent_revision_id UUID REFERENCES model_revisions(id),
      author_id UUID NOT NULL,
      message TEXT NOT NULL,
      created_at TIMESTAMPTZ NOT NULL DEFAULT now()
    );
  `);

  await db.execute(sql`
    CREATE TABLE IF NOT EXISTS model_revision_nodes (
      revision_id UUID NOT NULL REFERENCES model_revisions(id),
      node_id UUID NOT NULL,
      name TEXT NOT NULL,
      PRIMARY KEY (revision_id, node_id)
    );
  `);

  await db.execute(sql`
    CREATE TABLE IF NOT EXISTS model_branch_heads (
      model_id UUID NOT NULL REFERENCES models(id),
      branch_name TEXT NOT NULL,
      head_revision_id UUID NOT NULL REFERENCES model_revisions(id),
      updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
      PRIMARY KEY (model_id, branch_name)
    );
  `);
};
