import { ForceUnits, TorqueUnits } from "unitsnet-js";
import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

const pointLoadVector3dSchema = z.object({
  x: z.number().finite(),
  y: z.number().finite(),
  z: z.number().finite(),
});

const pointLoadForceRequestSchema = z.object({
  forceAlongX: z.number().finite(),
  forceAlongY: z.number().finite(),
  forceAlongZ: z.number().finite(),
  momentAboutX: z.number().finite(),
  momentAboutY: z.number().finite(),
  momentAboutZ: z.number().finite(),
});

const pointLoadForceResponseSchema = z.object({
  forceAlongX: z.number().finite(),
  forceAlongY: z.number().finite(),
  forceAlongZ: z.number().finite(),
  momentAboutX: z.number().finite(),
  momentAboutY: z.number().finite(),
  momentAboutZ: z.number().finite(),
});

const pointLoadSharedPropertiesSchema = z.object({
  force: pointLoadForceRequestSchema,
  direction: pointLoadVector3dSchema,
  units: z.object({
    force: z.enum(ForceUnits),
    torque: z.enum(TorqueUnits),
  }),
});

export const pointLoadPropertiesSchema = pointLoadSharedPropertiesSchema
  .extend({
    nodeId: uuidV7Schema,
    loadCaseId: uuidV7Schema,
  })
  .meta({ id: "PointLoadProperties" });

export const createPointLoadRequestSchema = pointLoadSharedPropertiesSchema
  .extend({
    nodeId: z.string().trim().min(1),
    loadCaseId: z.string().trim().min(1),
    tempId: z.string().trim().min(1).optional(),
  })
  .meta({ id: "CreatePointLoadRequest" });

export const putPointLoadRequestSchema = pointLoadPropertiesSchema
  .extend({
    id: uuidV7Schema,
  })
  .meta({ id: "PutPointLoadRequest" });

export const pointLoadResponseSchema = z
  .object({
    id: uuidV7Schema,
    revisionId: uuidV7Schema,
    nodeId: uuidV7Schema,
    loadCaseId: uuidV7Schema,
    force: pointLoadForceResponseSchema,
    direction: pointLoadVector3dSchema,
    units: z.object({
      force: z.literal(ForceUnits.Newtons),
      torque: z.literal(TorqueUnits.NewtonMeters),
    }),
  })
  .meta({ id: "PointLoad" });
