using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure;

internal sealed class BimFirstSourceModelUpdatedEventHandler(
    StructuralAnalysisDbContext dbContext,
    GetModelDiffWithDomainModelsCommandHandler getModelDiffCommandHandler,
    CreateModelProposalFromDiffCommandHandler createModelProposalFromDiffCommandHandler
) : DomainEventHandlerBase<BimFirstSourceModelUpdatedEvent>
{
    public override async Task HandleAsync(
        BimFirstSourceModelUpdatedEvent notification,
        CancellationToken ct = default
    )
    {
        // local version with local changes applied
        var sourceModel =
            dbContext.Models.Local.FirstOrDefault(m => m.Id == notification.SourceModelId)
            ?? throw new InvalidOperationException(
                $"Source model with id {notification.SourceModelId} not found in local DbContext cache"
            );

        // persistent version from database
        var currentBimFirstSourceModel = await dbContext
            .Models.AsNoTracking()
            .Include(e => e.Nodes)
            .Include(e => e.Element1ds)
            .FirstOrDefaultAsync(m => m.Id == notification.SourceModelId, ct);

        // If no persistent version exists, this is the first time the model is created
        // so we just want to diff against an empty model
        currentBimFirstSourceModel ??= new Model(
            sourceModel.Name,
            sourceModel.Description,
            sourceModel.Settings,
            sourceModel.Id
        );

        var diffResult = await getModelDiffCommandHandler.ExecuteAsync(
            (currentBimFirstSourceModel, sourceModel),
            ct
        );
        diffResult.ThrowIfError();

        var filteredDiff = this.RemoveNonGeometryChanges(diffResult.Value);
        if (filteredDiff is null)
        {
            // No geometry changes detected
            return;
        }

        foreach (var bimFirstModelId in notification.SubscribedModelIds)
        {
            var modelProposalResult = await createModelProposalFromDiffCommandHandler.ExecuteAsync(
                new ModelResourceRequest<ModelDiffData>()
                {
                    ModelId = bimFirstModelId,
                    Body = diffResult.Value,
                },
                ct
            );
            modelProposalResult.ThrowIfError();
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
                // just adding or removing a node isn't a geometry change. If an element1d is added or removed,
                // that will be captured there
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
                // added or removed elements are geometry changes
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
