import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

export const element1dResponseSchema = z.object({
  id: uuidV7Schema,
  revisionId: uuidV7Schema,
  startNodeId: uuidV7Schema,
  endNodeId: uuidV7Schema,
  materialId: uuidV7Schema,
  sectionProfileId: uuidV7Schema,
});
