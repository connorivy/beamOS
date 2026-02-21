import { z } from "zod";
import { NodeEntity } from "./node-entity";

const nodePayloadV1Schema = z.object({
    location: z.discriminatedUnion("type", [
        z.object({
            type: z.literal("spatial"),
            point: z.object({
                x: z.number(),
                y: z.number(),
                z: z.number(),
            }),
        }),
        z.object({
            type: z.literal("internal"),
            element1dId: z.string().uuid(),
            ratioAlongElement1d: z.number().min(0).max(1),
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
    applicationId: z.string().trim().min(1).optional(),
});

type NodePayloadV1 = z.infer<typeof nodePayloadV1Schema>;

export const nodeRevisionChangeCodec = {
    entityType: "node" as const,
    currentSchemaVersion: 1,

    toPersistencePayload(entity: NodeEntity): {
        schemaVersion: number;
        payload: NodePayloadV1;
    } {
        return {
            schemaVersion: this.currentSchemaVersion,
            payload: {
                location: entity.location,
                restraint: entity.restraint,
            },
        };
    },

    toDomain(
        revisionId: string,
        entityId: string,
        schemaVersion: number,
        payload: unknown,
    ): NodeEntity {
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
