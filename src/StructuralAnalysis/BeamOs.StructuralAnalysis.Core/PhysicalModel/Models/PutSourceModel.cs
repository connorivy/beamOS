using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithoutTrailingSlash + "/source")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class PutSourceModel(PutSourceModelCommandHandler createModelCommandHandler)
    : BeamOsModelResourceBaseEndpoint<PutModelRequest, PutModelResponse>
{
    public override async Task<Result<PutModelResponse>> ExecuteRequestAsync(
        ModelResourceRequest<PutModelRequest> req,
        CancellationToken ct = default
    ) => await createModelCommandHandler.ExecuteAsync(req, ct);
}

internal sealed class PutSourceModelCommandHandler(
    IModelRepository modelRepository,
    INodeDefinitionRepository nodeDefinitionRepository,
    IElement1dRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<PutModelRequest>, PutModelResponse>
{
    public async Task<Result<PutModelResponse>> ExecuteAsync(
        ModelResourceRequest<PutModelRequest> req,
        CancellationToken ct = default
    )
    {
        var options = req.Body.Options ?? new PatchOperationOptions();

        var model = await modelRepository.GetSingle(
            req.ModelId,
            ct,
            nameof(Model.Nodes),
            nameof(Model.InternalNodes),
            nameof(Model.Element1ds),
            nameof(Model.Materials),
            nameof(Model.SectionProfiles),
            nameof(Model.SectionProfilesFromLibrary)
        );

        if (model is null)
        {
            return BeamOsError.NotFound(
                description: $"Model with id '{req.ModelId}' was not found."
            );
        }
        modelRepository.Attach(model);

        var octree = new Octree(model.Id);
        var element1dStore = model.Element1ds?.ToDictionary(e => e.Id) ?? [];
        var nodeStore = (model.Nodes ?? [])
            .Concat<NodeDefinition>(model.InternalNodes ?? [])
            .ToDictionary(n => n.Id);
        var materialStore = (model.Materials ?? []).ToDictionary(m => m.Id);
        var sectionProfileStore = (model.SectionProfiles ?? [])
            .Concat<SectionProfileInfoBase>(model.SectionProfilesFromLibrary ?? [])
            .ToDictionary(sp => sp.Id);

        foreach (var node in model.Nodes ?? [])
        {
            octree.Add(node);
        }
        foreach (var internalNode in model.InternalNodes ?? [])
        {
            octree.Add(internalNode, element1dStore, nodeStore);
        }

        var response = new PutModelResponse();
        var nodeResponses = new List<NodeResponse>();
        HashSet<Element1dId> existingElement1dIds = [];
        HashSet<NodeId> existingNodeIds = [];
        foreach (var elementByLoc in req.Body.Element1dsToAddOrUpdateByExternalId ?? [])
        {
            var startNode = await this.GetOrAddNodeAtLocation(
                model.Id,
                octree,
                nodeStore,
                elementByLoc.StartNodeLocation.ToDomain(),
                existingNodeIds
            );
            var endNode = await this.GetOrAddNodeAtLocation(
                model.Id,
                octree,
                nodeStore,
                elementByLoc.EndNodeLocation.ToDomain(),
                existingNodeIds
            );

            Element1d? existingElement1d;
            if (startNode.Id != 0 && endNode.Id != 0)
            {
                existingElement1d = element1dStore.Values.FirstOrDefault(e =>
                    e.StartNodeId == startNode.Id && e.EndNodeId == endNode.Id
                );
                if (existingElement1d is not null)
                {
                    existingElement1dIds.Add(existingElement1d.Id);
                }
            }
            else
            {
                existingElement1d = null;
            }

            Element1d element1d;
            if (existingElement1d is not null)
            {
                element1d = element1dStore[existingElement1d.Id];
                element1d.StartNodeId = startNode.Id;
                element1d.EndNodeId = endNode.Id;
                element1d.StartNode = startNode;
                element1d.EndNode = endNode;
                await element1dRepository.Put(element1d);
            }
            else
            {
                element1d = new Element1d(model.Id, startNode.Id, endNode.Id, 1, 1)
                {
                    StartNode = startNode,
                    EndNode = endNode,
                };
                element1dRepository.Add(element1d);
            }
        }

        foreach (var nodeId in nodeStore.Keys.Where(e => !existingNodeIds.Contains(e)))
        {
            await nodeDefinitionRepository.RemoveById(model.Id, nodeId);
        }

        foreach (
            var element1dId in element1dStore.Keys.Where(e => !existingElement1dIds.Contains(e))
        )
        {
            await element1dRepository.RemoveById(model.Id, element1dId);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return response;
    }

    private async Task<NodeDefinition> GetOrAddNodeAtLocation(
        ModelId modelId,
        Octree octree,
        Dictionary<NodeId, NodeDefinition> nodeStore,
        Point location,
        HashSet<NodeId> existingNodeIds
    )
    {
        var nodeIds = octree.FindNodeIdsWithin(
            location,
            toleranceMeters: new Length(1, LengthUnit.Inch).Meters
        );

        if (nodeIds.Count == 0)
        {
            var node = new Node(modelId, location, Restraint.Free);
            nodeDefinitionRepository.Add(node);
            return node;
        }
        else
        {
            var nodeId = nodeIds.First();
            existingNodeIds.Add(nodeId);
            var node = nodeStore[nodeId];
            await nodeDefinitionRepository.Put(node);
            return node;
        }
    }
}
