using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Infrastructure;

internal sealed class BimFirstSourceModelUpdatedEventHandler(
    GetModelDiffCommandHandler getModelDiffCommandHandler,
    CreateModelProposalFromDiffCommandHandler createModelProposalFromDiffCommandHandler
) : DomainEventHandlerBase<BimFirstSourceModelUpdatedEvent>
{
    public override bool HandleAfterChangesSaved => true;

    public override async Task HandleAsync(
        BimFirstSourceModelUpdatedEvent notification,
        CancellationToken ct = default
    )
    {
        var diffRequest = new DiffModelRequest { TargetModelId = notification.SourceModelId };
        foreach (var bimFirstModelId in notification.SubscribedModelIds)
        {
            var diffResult = await getModelDiffCommandHandler.ExecuteAsync(
                new ModelResourceRequest<DiffModelRequest>()
                {
                    ModelId = bimFirstModelId,
                    Body = diffRequest,
                },
                ct
            );
            diffResult.ThrowIfError();

            var filteredDiff = this.RemoveNonGeometryChanges(diffResult.Value);
            if (filteredDiff is null)
            {
                // No geometry changes detected
                continue;
            }

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
