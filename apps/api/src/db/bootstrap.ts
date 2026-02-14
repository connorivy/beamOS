import { sql } from "drizzle-orm";
import { getDb } from "./client";

export const bootstrapDb = async () => {
  await getDb().execute(sql`
    CREATE TABLE IF NOT EXISTS users (
      id UUID PRIMARY KEY NOT NULL,
      name TEXT NOT NULL
    );
  `);

  await getDb().execute(sql`
    CREATE TABLE IF NOT EXISTS models (
      id UUID PRIMARY KEY NOT NULL,
      name TEXT NOT NULL
    );
  `);

  await getDb().execute(sql`
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

  await getDb().execute(sql`
    CREATE TABLE IF NOT EXISTS model_revision_drafts (
      id UUID PRIMARY KEY NOT NULL,
      model_id UUID NOT NULL REFERENCES models(id),
      model_name TEXT NOT NULL,
      parent_revision_id UUID REFERENCES model_revisions(id),
      second_parent_revision_id UUID REFERENCES model_revisions(id),
      author_id UUID NOT NULL,
      message TEXT NOT NULL,
      created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
      updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
    );
  `);

  await getDb().execute(sql`
    CREATE TABLE IF NOT EXISTS revision_changes (
      id UUID PRIMARY KEY NOT NULL,
      revision_id UUID REFERENCES model_revisions(id),
      draft_id UUID REFERENCES model_revision_drafts(id),
      entity_type TEXT NOT NULL,
      entity_id UUID NOT NULL,
      schema_version INTEGER NOT NULL DEFAULT 1,
      op TEXT NOT NULL,
      payload JSONB NOT NULL,
      created_at TIMESTAMPTZ NOT NULL DEFAULT now()
    );
  `);

  await getDb().execute(sql`
    ALTER TABLE revision_changes
    ADD COLUMN IF NOT EXISTS schema_version INTEGER NOT NULL DEFAULT 1;
  `);

  await getDb().execute(sql`
    CREATE TABLE IF NOT EXISTS model_branch_heads (
      model_id UUID NOT NULL REFERENCES models(id),
      branch_name TEXT NOT NULL,
      head_revision_id UUID NOT NULL REFERENCES model_revisions(id),
      updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
      PRIMARY KEY (model_id, branch_name)
    );
  `);

};
