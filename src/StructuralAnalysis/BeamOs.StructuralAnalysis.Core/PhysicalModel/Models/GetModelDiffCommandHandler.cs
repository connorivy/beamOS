using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

internal sealed partial class GetModelDiffCommandHandler(IModelRepository modelRepository)
    : ICommandHandler<ModelResourceRequest<DiffModelRequest>, ModelDiffResponse>
{
    public async Task<Result<ModelDiffResponse>> ExecuteAsync(
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

        // var sourceMapper = ModelToResponseMapper.Create(sourceModel.Settings.UnitSettings);
        // var targetMapper = ModelToResponseMapper.Create(targetModel.Settings.UnitSettings);

        // var sourceResponse = sourceMapper.Map(sourceModel);
        // var targetResponse = targetMapper.Map(targetModel);

        // Compute differences
        var response = new ModelDiffResponse
        {
            Nodes =
            [
                .. ComputeDiff<NodeId, Node>(
                        sourceModel.Nodes ?? [],
                        targetModel.Nodes ?? [],
                        n => n.Id
                    )
                    .Select(diff =>
                    {
                        var (status, node) = diff;
                        return new EntityDiff<NodeResponse>
                        {
                            Status = status,
                            Entity = node.ToResponse(),
                        };
                    }),
            ],
            Element1ds = ComputeDiff(
                sourceModel.Element1ds ?? [],
                targetModel.Element1ds ?? [],
                e => e.Id
            ),
            Materials = ComputeDiff(
                sourceModel.Materials ?? [],
                targetModel.Materials ?? [],
                m => m.Id
            ),
            SectionProfiles = ComputeDiff(
                sourceModel.SectionProfiles ?? [],
                targetModel.SectionProfiles ?? [],
                sp => sp.Id
            ),
            PointLoads = ComputeDiff(
                sourceModel.PointLoads ?? [],
                targetModel.PointLoads ?? [],
                pl => pl.Id
            ),
            MomentLoads = ComputeDiff(
                sourceModel.MomentLoads ?? [],
                targetModel.MomentLoads ?? [],
                ml => ml.Id
            ),
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
