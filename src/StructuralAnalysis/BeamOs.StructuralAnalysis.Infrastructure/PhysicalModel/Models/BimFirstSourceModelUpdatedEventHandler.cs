using BeamOs.Common.Contracts;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Infrastructure;

internal sealed class BimFirstSourceModelUpdatedEventHandler(
    IModelRepository modelRepository,
    CreateModelProposalFromDiffCommandHandler createModelProposalFromDiffCommandHandler
) : DomainEventHandlerBase<BimFirstSourceModelUpdatedEvent>
{
    public override bool HandleAfterChangesSaved => false;

    public override async Task HandleAsync(
        BimFirstSourceModelUpdatedEvent notification,
        CancellationToken ct = default
    )
    {
        // Query the BIM source model (the updated model)
        // Note: This handler computes diffs locally instead of using GetModelDiffCommandHandler.
        // While HandleAfterChangesSaved is false (runs before save), the handler runs in a separate
        // DI scope with a separate DbContext, so this query may not see uncommitted changes.
        // This is a known architectural limitation - to fully access uncommitted changes, the event
        // would need to pass model data in a serializable format, or the event system would need
        // to support shared DbContext for pre-save handlers.
        var sourceModel = await modelRepository.GetSingle(
            notification.SourceModelId,
            ct,
            nameof(Model.Nodes),
            nameof(Model.Element1ds),
            nameof(Model.Materials),
            nameof(Model.SectionProfiles),
            nameof(Model.PointLoads),
            nameof(Model.MomentLoads)
        );

        if (sourceModel is null)
        {
            return;
        }

        foreach (var bimFirstModelId in notification.SubscribedModelIds)
        {
            // Get the subscribed BimFirst model
            var subscribedModel = await modelRepository.GetSingle(
                bimFirstModelId,
                ct,
                nameof(Model.Nodes),
                nameof(Model.Element1ds),
                nameof(Model.Materials),
                nameof(Model.SectionProfiles),
                nameof(Model.PointLoads),
                nameof(Model.MomentLoads)
            );

            if (subscribedModel is null)
            {
                continue;
            }

            // Compute diff: subscribedModel (base) vs sourceModel (target with changes)
            var diffData = this.ComputeModelDiff(subscribedModel, sourceModel);

            var filteredDiff = this.RemoveNonGeometryChanges(diffData);
            if (filteredDiff is null)
            {
                // No geometry changes detected
                continue;
            }

            var modelProposalResult = await createModelProposalFromDiffCommandHandler.ExecuteAsync(
                new ModelResourceRequest<ModelDiffData>()
                {
                    ModelId = bimFirstModelId,
                    Body = filteredDiff,
                },
                ct
            );
            modelProposalResult.ThrowIfError();
        }
    }

    private ModelDiffData ComputeModelDiff(Model sourceModel, Model targetModel)
    {
        // Validate models have required data
        if (sourceModel.Settings?.UnitSettings is null)
        {
            throw new InvalidOperationException(
                $"Source model ({sourceModel.Id}) is missing Settings or UnitSettings"
            );
        }

        if (targetModel.Settings?.UnitSettings is null)
        {
            throw new InvalidOperationException(
                $"Target model ({targetModel.Id}) is missing Settings or UnitSettings"
            );
        }

        return new ModelDiffData
        {
            BaseModelId = sourceModel.Id,
            TargetModelId = targetModel.Id,
            Nodes =
            [
                .. ComputeDiff<NodeId, Node>(
                        sourceModel.Nodes ?? [],
                        targetModel.Nodes ?? [],
                        n => n.Id
                    )
                    .Select(diff => new EntityDiff<NodeResponse>
                    {
                        SourceEntity = diff.SourceEntity?.ToResponse(),
                        TargetEntity = diff.TargetEntity?.ToResponse(),
                    }),
            ],
            Element1ds =
            [
                .. ComputeDiff<Element1dId, Element1d>(
                        sourceModel.Element1ds ?? [],
                        targetModel.Element1ds ?? [],
                        n => n.Id
                    )
                    .Select(diff => new EntityDiff<Element1dResponse>
                    {
                        SourceEntity = diff.SourceEntity?.ToResponse(),
                        TargetEntity = diff.TargetEntity?.ToResponse(),
                    }),
            ],
            Materials =
            [
                .. ComputeDiff<MaterialId, Material>(
                        sourceModel.Materials ?? [],
                        targetModel.Materials ?? [],
                        n => n.Id
                    )
                    .Select(diff => new EntityDiff<MaterialResponse>
                    {
                        SourceEntity = diff.SourceEntity?.ToResponse(
                            sourceModel.Settings.UnitSettings.PressureUnit
                        ),
                        TargetEntity = diff.TargetEntity?.ToResponse(
                            targetModel.Settings.UnitSettings.PressureUnit
                        ),
                    }),
            ],
            SectionProfiles =
            [
                .. ComputeDiff<SectionProfileId, SectionProfile>(
                        sourceModel.SectionProfiles ?? [],
                        targetModel.SectionProfiles ?? [],
                        n => n.Id
                    )
                    .Select(diff => new EntityDiff<SectionProfileResponse>
                    {
                        SourceEntity = diff.SourceEntity?.ToResponse(
                            sourceModel.Settings.UnitSettings.LengthUnit
                        ),
                        TargetEntity = diff.TargetEntity?.ToResponse(
                            targetModel.Settings.UnitSettings.LengthUnit
                        ),
                    }),
            ],
            PointLoads =
            [
                .. ComputeDiff<PointLoadId, PointLoad>(
                        sourceModel.PointLoads ?? [],
                        targetModel.PointLoads ?? [],
                        n => n.Id
                    )
                    .Select(diff => new EntityDiff<PointLoadResponse>
                    {
                        SourceEntity = diff.SourceEntity?.ToResponse(),
                        TargetEntity = diff.TargetEntity?.ToResponse(),
                    }),
            ],
            MomentLoads =
            [
                .. ComputeDiff<MomentLoadId, MomentLoad>(
                        sourceModel.MomentLoads ?? [],
                        targetModel.MomentLoads ?? [],
                        n => n.Id
                    )
                    .Select(diff => new EntityDiff<MomentLoadResponse>
                    {
                        SourceEntity = diff.SourceEntity?.ToResponse(),
                        TargetEntity = diff.TargetEntity?.ToResponse(),
                    }),
            ],
        };
    }

    private static IEnumerable<(T? SourceEntity, T? TargetEntity)> ComputeDiff<TId, T>(
        IEnumerable<T> sourceEntities,
        IEnumerable<T> targetEntities,
        Func<T, int> keySelector
    )
        where T : class, IBeamOsModelEntity<TId, T>
        where TId : struct, IIntBasedId
    {
        var sourceDict = sourceEntities.ToDictionary(keySelector);
        var targetDict = targetEntities.ToDictionary(keySelector);

        // Find removed entities (in source but not in target)
        foreach (var sourceEntity in sourceEntities)
        {
            var key = keySelector(sourceEntity);
            if (!targetDict.ContainsKey(key))
            {
                yield return (sourceEntity, null);
            }
        }

        // Find added and modified entities
        foreach (var targetEntity in targetEntities)
        {
            var key = keySelector(targetEntity);
            if (!sourceDict.TryGetValue(key, out var sourceEntity))
            {
                yield return (null, targetEntity);
            }
            else if (!sourceEntity.MemberwiseEquals(targetEntity))
            {
                yield return (sourceEntity, targetEntity);
            }
        }
    }

    public ModelDiffData? RemoveNonGeometryChanges(ModelDiffData modelDiff)
    {
        List<EntityDiff<NodeResponse>> nodesWithGeometryChanges = [];
        List<EntityDiff<Element1dResponse>> element1dsWithGeometryChanges = [];
        foreach (var nodeDiff in modelDiff.Nodes)
        {
            if (nodeDiff.Status != DiffStatus.Modified)
            {
                // added or removed nodes are geometry changes
                nodesWithGeometryChanges.Add(nodeDiff);
                continue;
            }
            _ =
                nodeDiff.SourceEntity
                ?? throw new InvalidOperationException(
                    "Source node is null, but has modified status"
                );
            _ =
                nodeDiff.TargetEntity
                ?? throw new InvalidOperationException(
                    "Target node is null, but has modified status"
                );

            var sourceLoc = nodeDiff.SourceEntity.LocationPoint.ToDomain();
            var targetLoc = nodeDiff.TargetEntity.LocationPoint.ToDomain();
            var distance = sourceLoc.CalculateDistance(targetLoc);
            if (distance.Meters > 1e-6)
            {
                nodesWithGeometryChanges.Add(nodeDiff);
            }
        }

        foreach (var element1dDiff in modelDiff.Element1ds)
        {
            if (element1dDiff.Status != DiffStatus.Modified)
            {
                // added or removed nodes are geometry changes
                element1dsWithGeometryChanges.Add(element1dDiff);
                continue;
            }

            _ =
                element1dDiff.SourceEntity
                ?? throw new InvalidOperationException(
                    "Source element1d is null, but has modified status"
                );
            _ =
                element1dDiff.TargetEntity
                ?? throw new InvalidOperationException(
                    "Target element1d is null, but has modified status"
                );

            if (
                element1dDiff.SourceEntity.StartNodeId != element1dDiff.TargetEntity.StartNodeId
                || element1dDiff.SourceEntity.EndNodeId != element1dDiff.TargetEntity.EndNodeId
            )
            {
                element1dsWithGeometryChanges.Add(element1dDiff);
            }
        }

        return nodesWithGeometryChanges.Count + element1dsWithGeometryChanges.Count > 0
            ? new ModelDiffData
            {
                BaseModelId = modelDiff.BaseModelId,
                TargetModelId = modelDiff.TargetModelId,
                Nodes = nodesWithGeometryChanges,
                Element1ds = element1dsWithGeometryChanges,
                Materials = [],
                SectionProfiles = [],
                PointLoads = [],
                MomentLoads = [],
            }
            : null;
    }
}
