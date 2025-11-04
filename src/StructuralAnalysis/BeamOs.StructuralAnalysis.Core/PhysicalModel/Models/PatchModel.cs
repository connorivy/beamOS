using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithoutTrailingSlash)]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Authenticated)]
internal class PatchModel(PatchModelCommandHandler createModelCommandHandler)
    : BeamOsModelResourceBaseEndpoint<PatchModelRequest, PatchModelResponse>
{
    public override async Task<Result<PatchModelResponse>> ExecuteRequestAsync(
        ModelResourceRequest<PatchModelRequest> req,
        CancellationToken ct = default
    ) => await createModelCommandHandler.ExecuteAsync(req, ct);
}

internal sealed class PatchModelCommandHandler(
    IModelRepository modelRepository,
    INodeDefinitionRepository nodeDefinitionRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<PatchModelRequest>, PatchModelResponse>
{
    public async Task<Result<PatchModelResponse>> ExecuteAsync(
        ModelResourceRequest<PatchModelRequest> req,
        CancellationToken ct = default
    )
    {
        var options = req.Body.Options ?? new PatchOperationOptions();

        var model = await modelRepository.GetSingle(req.ModelId, ct);
        if (model is null)
        {
            return BeamOsError.NotFound(
                description: $"Model with id '{req.ModelId}' was not found."
            );
        }

        var octree = new Octree(model.Id);
        var element1dStore = model.Element1ds?.ToDictionary(e => e.Id) ?? [];
        var nodeStore = (model.Nodes ?? [])
            .Concat<NodeDefinition>(model.InternalNodes ?? [])
            .ToDictionary(n => n.Id);

        foreach (Node node in model.Nodes ?? [])
        {
            octree.Add(node);
        }
        foreach (var internalNode in model.InternalNodes ?? [])
        {
            octree.Add(internalNode, element1dStore, nodeStore);
        }

        var response = new PatchModelResponse() { Element1dsToAddOrUpdateByExternalIdResults = [] };
        var nodeResponses = new List<NodeResponse>();
        foreach (var elementByLoc in req.Body.Element1dsToAddOrUpdateByExternalId ?? [])
        {
            var startNode = this.GetOrAddNodeAtLocation(
                model.Id,
                octree,
                nodeStore,
                elementByLoc.StartNodeLocation.ToDomain()
            );
            var endNode = this.GetOrAddNodeAtLocation(
                model.Id,
                octree,
                nodeStore,
                elementByLoc.EndNodeLocation.ToDomain()
            );
            var existingElement1d = element1dStore.Values.FirstOrDefault(e =>
                e.StartNodeId == startNode.Id && e.EndNodeId == endNode.Id
            );
            Element1d element1d;
            if (existingElement1d is not null)
            {
                element1d = element1dStore[existingElement1d.Id];
                element1d.StartNodeId = startNode.Id;
                element1d.EndNodeId = endNode.Id;
            }
            else
            {
                element1d = new Element1d(model.Id, startNode.Id, endNode.Id, 1, 1);
                element1dStore.Add(element1d.Id, element1d);
            }
            response.Element1dsToAddOrUpdateByExternalIdResults.Add(
                new OperationStatus()
                {
                    ObjectType = BeamOsObjectType.Element1d,
                    ExternalId = elementByLoc.ExternalId,
                    Result = Result.Success,
                }
            );
        }

        await unitOfWork.SaveChangesAsync(ct);

        return response;
    }

    private NodeDefinition GetOrAddNodeAtLocation(
        ModelId modelId,
        Octree octree,
        Dictionary<NodeId, NodeDefinition> nodeStore,
        Point location
    )
    {
        var nodeIds = octree.FindNodeIdsWithin(
            location,
            toleranceMeters: new Length(1, LengthUnit.Inch).Meters
        );
        NodeDefinition node;
        if (nodeIds.Count == 0)
        {
            // Create new start node
            node = new Node(modelId, location, Restraint.Free);
            nodeDefinitionRepository.Add(node);
        }
        else
        {
            node = nodeStore[nodeIds[0]];
        }
        return node;
    }
}
