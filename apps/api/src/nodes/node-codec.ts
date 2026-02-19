import { z } from "zod";
import { Ratio } from "unitsnet-js";
import { NodeEntity } from "./node-entity";

const nodePayloadV1Schema = z.object({
    location: z.discriminatedUnion("type", [
        z.object({
            type: z.literal("spatial"),
            point: z.object({
                x: z.number().finite(),
                y: z.number().finite(),
                z: z.number().finite(),
            }),
        }),
        z.object({
            type: z.literal("internal"),
            element1dId: z.string().uuid(),
            ratioAlongElement1d: z.number().finite().min(0).max(1),
        }),
    ]),
    restraint: z
        .object({
            canTranslateAlongX: z.boolean(),
            canTranslateAlongY: z.boolean(),
            canTranslateAlongZ: z.boolean(),
            canRotateAboutX: z.boolean(),
            canRotateAboutY: z.boolean(),
            canRotateAboutZ: z.boolean(),
        })
        .optional(),
});

type NodePayloadV1 = z.infer<typeof nodePayloadV1Schema>;

export const nodeRevisionChangeCodec = {
    entityType: "node" as const,
    currentSchemaVersion: 1,

    toPersistencePayload(entity: NodeEntity): {
        schemaVersion: number;
        payload: NodePayloadV1;
    } {
        const snapshot = entity.toSnapshot();

        return {
            schemaVersion: this.currentSchemaVersion,
            payload: {
                location:
                    snapshot.nodeType === "internalNode"
                        ? {
                              type: "internal",
                              element1dId: snapshot.element1dId as string,
                              ratioAlongElement1d: (
                                  snapshot.distanceAlongElement1d as Ratio
                              ).DecimalFractions,
                          }
                        : {
                              type: "spatial",
                              point: snapshot.point as { x: number; y: number; z: number },
                          },
                restraint: snapshot.restraint,
            },
        };
    },

    toDomain(revisionId: string, entityId: string, schemaVersion: number, payload: unknown): NodeEntity {
        if (schemaVersion > this.currentSchemaVersion) {
            throw new Error(`Unsupported schema version: ${schemaVersion}`);
        }

        const parseResult = nodePayloadV1Schema.safeParse(payload);
        if (!parseResult.success) {
            throw new Error(`Invalid payload: ${parseResult.error.message}`);
        }

        return NodeEntity.rehydrate(
            {
                id: entityId,
                location: parseResult.data.location,
                restraint: parseResult.data.restraint,
            },
            revisionId,
        );
    },
};
