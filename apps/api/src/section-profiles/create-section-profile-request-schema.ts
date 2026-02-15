import { z } from "zod";

export const sectionPropertiesInputSchema = z.object({
  area: z.number().finite(),
  strongAxisMomentOfInertia: z.number().finite(),
  weakAxisMomentOfInertia: z.number().finite(),
  torsionalConstant: z.number().finite(),
  warpingConstant: z.number().finite(),
  strongAxisPlasticSectionModulus: z.number().finite(),
  weakAxisPlasticSectionModulus: z.number().finite(),
  strongAxisElasticSectionModulus: z.number().finite(),
  weakAxisElasticSectionModulus: z.number().finite(),
});

const baseSectionInputSchema = z.object({
  tempId: z.string().trim().min(1).optional(),
  name: z.string().trim().min(1),
  ...sectionPropertiesInputSchema.shape,
});

const standardSectionInputSchema = baseSectionInputSchema.extend({
  discriminator: z.literal("STANDARD"),
});

const withShearAreasSectionInputSchema = baseSectionInputSchema.extend({
  discriminator: z.literal("WITH_SHEAR_AREAS"),
  strongAxisShearArea: z.number().finite(),
  weakAxisShearArea: z.number().finite(),
});

export const createSectionProfileRequestSchema = z.discriminatedUnion(
  "discriminator",
  [standardSectionInputSchema, withShearAreasSectionInputSchema],
);
