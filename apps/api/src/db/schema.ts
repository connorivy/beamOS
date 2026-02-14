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

export const nodes = pgTable("nodes", {
  id: uuid("id").primaryKey(),
  revisionId: uuid("revision_id")
    .notNull()
    .references(() => modelRevisions.id),
  locationDiscriminator: text("location_discriminator").notNull(),
  pointX: doublePrecision("point_x"),
  pointY: doublePrecision("point_y"),
  pointZ: doublePrecision("point_z"),
  element1dId: uuid("element1d_id"),
  ratioAlongElement1d: doublePrecision("ratio_along_element1d"),
  restraint: jsonb("restraint").notNull(),
});

export const materials = pgTable("materials", {
  id: uuid("id").primaryKey(),
  revisionId: uuid("revision_id")
    .notNull()
    .references(() => modelRevisions.id),
  pressureESi: doublePrecision("pressure_e_si").notNull(),
  pressureGSi: doublePrecision("pressure_g_si").notNull(),
});

export const sectionProfiles = pgTable("section_profiles", {
  id: uuid("id").primaryKey(),
  revisionId: uuid("revision_id")
    .notNull()
    .references(() => modelRevisions.id),
  name: text("name").notNull(),
  discriminator: text("discriminator").notNull(),
  areaSi: doublePrecision("area_si").notNull(),
  strongAxisMomentOfInertiaSi: doublePrecision(
    "strong_axis_moment_of_inertia_si",
  ).notNull(),
  weakAxisMomentOfInertiaSi: doublePrecision(
    "weak_axis_moment_of_inertia_si",
  ).notNull(),
  torsionalConstantSi: doublePrecision("torsional_constant_si").notNull(),
  warpingConstantSi: doublePrecision("warping_constant_si").notNull(),
  strongAxisPlasticSectionModulusSi: doublePrecision(
    "strong_axis_plastic_section_modulus_si",
  ).notNull(),
  weakAxisPlasticSectionModulusSi: doublePrecision(
    "weak_axis_plastic_section_modulus_si",
  ).notNull(),
  strongAxisElasticSectionModulusSi: doublePrecision(
    "strong_axis_elastic_section_modulus_si",
  ).notNull(),
  weakAxisElasticSectionModulusSi: doublePrecision(
    "weak_axis_elastic_section_modulus_si",
  ).notNull(),
  strongAxisShearAreaSi: doublePrecision("strong_axis_shear_area_si"),
  weakAxisShearAreaSi: doublePrecision("weak_axis_shear_area_si"),
});

export const element1ds = pgTable("element1ds", {
  id: uuid("id").primaryKey(),
  revisionId: uuid("revision_id")
    .notNull()
    .references(() => modelRevisions.id),
  startNodeId: uuid("start_node_id").notNull(),
  endNodeId: uuid("end_node_id").notNull(),
  materialId: uuid("material_id").notNull(),
  sectionProfileId: uuid("section_profile_id").notNull(),
});

export type Model = InferSelectModel<typeof models>;
export type Node = InferSelectModel<typeof nodes>;
export type Material = InferSelectModel<typeof materials>;
export type SectionProfile = InferSelectModel<typeof sectionProfiles>;
export type Element1d = InferSelectModel<typeof element1ds>;
