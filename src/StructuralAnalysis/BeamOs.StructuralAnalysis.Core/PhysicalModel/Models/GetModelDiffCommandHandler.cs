using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
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
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

internal sealed partial class GetModelDiffCommandHandler(IModelRepository modelRepository)
    : ICommandHandler<ModelResourceRequest<DiffModelRequest>, ModelDiffData>
{
    public async Task<Result<ModelDiffData>> ExecuteAsync(
        ModelResourceRequest<DiffModelRequest> req,
        CancellationToken ct = default
    )
    {
        var sourceModelId = req.ModelId;
        var targetModelId = req.Body.TargetModelId;

        // Get both models
        var sourceModel = await modelRepository.GetSingle(
            sourceModelId,
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
            return BeamOsError.NotFound(
                description: $"Source model with id {sourceModelId} not found"
            );
        }

        var targetModel = await modelRepository.GetSingle(
            targetModelId,
            ct,
            nameof(Model.Nodes),
            nameof(Model.Element1ds),
            nameof(Model.Materials),
            nameof(Model.SectionProfiles),
            nameof(Model.PointLoads),
            nameof(Model.MomentLoads)
        );

        if (targetModel is null)
        {
            return BeamOsError.NotFound(
                description: $"Target model with id {targetModelId} not found"
            );
        }

        var response = new ModelDiffData
        {
            BaseModelId = sourceModelId,
            TargetModelId = targetModelId,
            Nodes =
            [
                .. ComputeDiff<NodeId, Node>(
                        sourceModel.Nodes ?? [],
                        targetModel.Nodes ?? [],
                        n => n.Id
                    )
                    .Select(diff => new EntityDiff<NodeResponse>
                    {
                        Status = diff.Status,
                        Entity = diff.Entity.ToResponse(),
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
                        Status = diff.Status,
                        Entity = diff.Entity.ToResponse(),
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
                        Status = diff.Status,
                        Entity = diff.Entity.ToResponse(
                            sourceModel.Settings.UnitSettings.PressureUnit
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
                        Status = diff.Status,
                        Entity = diff.Entity.ToResponse(
                            sourceModel.Settings.UnitSettings.LengthUnit
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
                        Status = diff.Status,
                        Entity = diff.Entity.ToResponse(),
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
                        Status = diff.Status,
                        Entity = diff.Entity.ToResponse(),
                    }),
            ],
        };

        return response;
    }

    private static IEnumerable<(DiffStatus Status, T Entity)> ComputeDiff<TId, T>(
        IEnumerable<T> sourceEntities,
        IEnumerable<T> targetEntities,
        Func<T, int> keySelector
    )
        where T : IBeamOsModelEntity<TId, T>
        where TId : struct, IIntBasedId
    {
        var diffs = new List<EntityDiff<T>>();

        var sourceDict = sourceEntities.ToDictionary(keySelector);
        var targetDict = targetEntities.ToDictionary(keySelector);

        // Find removed entities (in source but not in target)
        foreach (var sourceEntity in sourceEntities)
        {
            var key = keySelector(sourceEntity);
            if (!targetDict.ContainsKey(key))
            {
                yield return (DiffStatus.Removed, sourceEntity);
            }
        }

        // Find added and modified entities
        foreach (var targetEntity in targetEntities)
        {
            var key = keySelector(targetEntity);
            if (!sourceDict.TryGetValue(key, out var sourceEntity))
            {
                yield return (DiffStatus.Added, targetEntity);
            }
            else if (!sourceEntity.MemberwiseEquals(targetEntity))
            {
                yield return (DiffStatus.Modified, targetEntity);
            }
        }
    }
}
