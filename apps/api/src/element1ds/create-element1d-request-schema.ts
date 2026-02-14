import { uuidV7Schema } from "src/common/uuid";
import { z } from "zod";

export const createElement1dRequestSchema = z.object({
  tempId: z.string().trim().min(1).optional(),
  startNodeId: uuidV7Schema,
  endNodeId: uuidV7Schema,
  materialId: uuidV7Schema,
  sectionProfileId: uuidV7Schema,
});
