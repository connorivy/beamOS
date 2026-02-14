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
    CREATE TABLE IF NOT EXISTS node_revisions (
      revision_id UUID NOT NULL REFERENCES model_revisions(id),
      node_id UUID NOT NULL,
      name TEXT NOT NULL,
      op TEXT NOT NULL DEFAULT 'update',
      PRIMARY KEY (revision_id, node_id)
    );
  `);

  await getDb().execute(sql`
    CREATE TABLE IF NOT EXISTS node_revision_drafts (
      draft_id UUID NOT NULL REFERENCES model_revision_drafts(id),
      node_id UUID NOT NULL,
      name TEXT NOT NULL,
      op TEXT NOT NULL DEFAULT 'update',
      PRIMARY KEY (draft_id, node_id)
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

  await getDb().execute(sql`
    CREATE TABLE IF NOT EXISTS materials (
      id UUID PRIMARY KEY NOT NULL,
      revision_id UUID NOT NULL REFERENCES model_revisions(id),
      pressure_e_si DOUBLE PRECISION NOT NULL,
      pressure_g_si DOUBLE PRECISION NOT NULL
    );
  `);

  await getDb().execute(sql`
    CREATE TABLE IF NOT EXISTS section_profiles (
      id UUID PRIMARY KEY NOT NULL,
      revision_id UUID NOT NULL REFERENCES model_revisions(id),
      name TEXT NOT NULL,
      discriminator TEXT NOT NULL,
      area_si DOUBLE PRECISION NOT NULL,
      strong_axis_moment_of_inertia_si DOUBLE PRECISION NOT NULL,
      weak_axis_moment_of_inertia_si DOUBLE PRECISION NOT NULL,
      torsional_constant_si DOUBLE PRECISION NOT NULL,
      warping_constant_si DOUBLE PRECISION NOT NULL,
      strong_axis_plastic_section_modulus_si DOUBLE PRECISION NOT NULL,
      weak_axis_plastic_section_modulus_si DOUBLE PRECISION NOT NULL,
      strong_axis_elastic_section_modulus_si DOUBLE PRECISION NOT NULL,
      weak_axis_elastic_section_modulus_si DOUBLE PRECISION NOT NULL,
      strong_axis_shear_area_si DOUBLE PRECISION,
      weak_axis_shear_area_si DOUBLE PRECISION
    );
  `);

  await getDb().execute(sql`
    CREATE TABLE IF NOT EXISTS element1ds (
      id UUID PRIMARY KEY NOT NULL,
      revision_id UUID NOT NULL REFERENCES model_revisions(id),
      start_node_id UUID NOT NULL,
      end_node_id UUID NOT NULL,
      material_id UUID NOT NULL,
      section_profile_id UUID NOT NULL
    );
  `);
};
