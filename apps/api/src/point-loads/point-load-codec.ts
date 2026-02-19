import { z } from "zod";
import { Force, Torque } from "unitsnet-js";
import { PointLoadEntity } from "./point-load-entity";

const pointLoadPayloadV1Schema = z.object({
    nodeId: z.string().uuid(),
    loadCaseId: z.string().uuid(),
    force: z.object({
        forceAlongX: z.number().finite(),
        forceAlongY: z.number().finite(),
        forceAlongZ: z.number().finite(),
        momentAboutX: z.number().finite(),
        momentAboutY: z.number().finite(),
        momentAboutZ: z.number().finite(),
    }),
    direction: z.object({
        x: z.number().finite(),
        y: z.number().finite(),
        z: z.number().finite(),
    }),
});

type PointLoadPayloadV1 = z.infer<typeof pointLoadPayloadV1Schema>;

export const pointLoadRevisionChangeCodec = {
    entityType: "pointload" as const,
    currentSchemaVersion: 1,

    toPersistencePayload(entity: PointLoadEntity): {
        schemaVersion: number;
        payload: PointLoadPayloadV1;
    } {
        return {
            schemaVersion: this.currentSchemaVersion,
            payload: {
                nodeId: entity.nodeId,
                loadCaseId: entity.loadCaseId,
                force: {
                    forceAlongX: entity.force.forceAlongX.Newtons,
                    forceAlongY: entity.force.forceAlongY.Newtons,
                    forceAlongZ: entity.force.forceAlongZ.Newtons,
                    momentAboutX: entity.force.momentAboutX.NewtonMeters,
                    momentAboutY: entity.force.momentAboutY.NewtonMeters,
                    momentAboutZ: entity.force.momentAboutZ.NewtonMeters,
                },
                direction: { ...entity.direction },
            },
        };
    },

    toDomain(
        revisionId: string,
        entityId: string,
        schemaVersion: number,
        payload: unknown,
    ): PointLoadEntity {
        if (schemaVersion > this.currentSchemaVersion) {
            throw new Error(`Unsupported schema version: ${schemaVersion}`);
        }

        const parseResult = pointLoadPayloadV1Schema.safeParse(payload);
        if (!parseResult.success) {
            throw new Error(`Invalid payload: ${parseResult.error.message}`);
        }

        return PointLoadEntity.rehydrate({
            id: entityId,
            revisionId,
            nodeId: parseResult.data.nodeId,
            loadCaseId: parseResult.data.loadCaseId,
            force: {
                forceAlongX: Force.FromNewtons(parseResult.data.force.forceAlongX),
                forceAlongY: Force.FromNewtons(parseResult.data.force.forceAlongY),
                forceAlongZ: Force.FromNewtons(parseResult.data.force.forceAlongZ),
                momentAboutX: Torque.FromNewtonMeters(parseResult.data.force.momentAboutX),
                momentAboutY: Torque.FromNewtonMeters(parseResult.data.force.momentAboutY),
                momentAboutZ: Torque.FromNewtonMeters(parseResult.data.force.momentAboutZ),
            },
            direction: parseResult.data.direction,
        });
    },
};
