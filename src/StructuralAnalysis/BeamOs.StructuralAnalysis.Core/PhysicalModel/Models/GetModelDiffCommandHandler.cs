using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

internal sealed class GetModelDiffCommandHandler(IModelRepository modelRepository)
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

        // Create mappers
        var sourceMapper = ModelToResponseMapper.Create(sourceModel.Settings.UnitSettings);
        var targetMapper = ModelToResponseMapper.Create(targetModel.Settings.UnitSettings);

        // Map to responses
        var sourceResponse = sourceMapper.Map(sourceModel);
        var targetResponse = targetMapper.Map(targetModel);

        // Compute differences
        var response = new ModelDiffResponse
        {
            Nodes = ComputeDiff(
                sourceResponse.Nodes ?? [],
                targetResponse.Nodes ?? [],
                n => n.Id
            ),
            Element1ds = ComputeDiff(
                sourceResponse.Element1ds ?? [],
                targetResponse.Element1ds ?? [],
                e => e.Id
            ),
            Materials = ComputeDiff(
                sourceResponse.Materials ?? [],
                targetResponse.Materials ?? [],
                m => m.Id
            ),
            SectionProfiles = ComputeDiff(
                sourceResponse.SectionProfiles ?? [],
                targetResponse.SectionProfiles ?? [],
                sp => sp.Id
            ),
            PointLoads = ComputeDiff(
                sourceResponse.PointLoads ?? [],
                targetResponse.PointLoads ?? [],
                pl => pl.Id
            ),
            MomentLoads = ComputeDiff(
                sourceResponse.MomentLoads ?? [],
                targetResponse.MomentLoads ?? [],
                ml => ml.Id
            ),
        };

        return response;
    }

    private static List<EntityDiff<T>> ComputeDiff<T>(
        List<T> sourceEntities,
        List<T> targetEntities,
        Func<T, int> keySelector
    )
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
                diffs.Add(
                    new EntityDiff<T> { Status = DiffStatus.Removed, Entity = sourceEntity }
                );
            }
        }

        // Find added and modified entities
        foreach (var targetEntity in targetEntities)
        {
            var key = keySelector(targetEntity);
            if (!sourceDict.TryGetValue(key, out var sourceEntity))
            {
                // Added entity (in target but not in source)
                diffs.Add(
                    new EntityDiff<T> { Status = DiffStatus.Added, Entity = targetEntity }
                );
            }
            else
            {
                // Check if modified
                if (!EqualityComparer<T>.Default.Equals(sourceEntity, targetEntity))
                {
                    diffs.Add(
                        new EntityDiff<T> { Status = DiffStatus.Modified, Entity = targetEntity }
                    );
                }
            }
        }

        return diffs;
    }
}
