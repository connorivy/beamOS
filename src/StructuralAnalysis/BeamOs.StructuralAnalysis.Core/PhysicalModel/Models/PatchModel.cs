using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using StructuralShapes.Contracts;

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
    IElement1dRepository element1dRepository,
    IMaterialRepository materialRepository,
    ISectionProfileRepository sectionProfileRepository,
    ISectionProfileFromLibraryRepository sectionProfileFromLibraryRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<PatchModelRequest>, PatchModelResponse>
{
    public async Task<Result<PatchModelResponse>> ExecuteAsync(
        ModelResourceRequest<PatchModelRequest> req,
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

        var octree = new Octree(model.Id);
        var element1dStore = model.Element1ds?.ToDictionary(e => e.Id) ?? [];
        var nodeStore = (model.Nodes ?? [])
            .Concat<NodeDefinition>(model.InternalNodes ?? [])
            .ToDictionary(n => n.Id);
        var materialStore = (model.Materials ?? []).ToDictionary(m => m.Id);
        var sectionProfileStore = (model.SectionProfiles ?? [])
            .Concat<SectionProfileInfoBase>(model.SectionProfilesFromLibrary ?? [])
            .ToDictionary(sp => sp.Id);
        
        // Track next available IDs for new entities created in this request
        int nextNodeId = model.MaxNodeId;
        int nextElement1dId = model.MaxElement1dId;

        foreach (Node node in model.Nodes ?? [])
        {
            octree.Add(node);
        }
        foreach (var internalNode in model.InternalNodes ?? [])
        {
            octree.Add(internalNode, element1dStore, nodeStore);
        }

        foreach (var materialRequest in req.Body.MaterialRequests ?? [])
        {
            Material material;
            if (materialRequest.Id.HasValue && materialStore.ContainsKey(materialRequest.Id.Value))
            {
                material = materialStore[materialRequest.Id.Value];
            }
            else
            {
                material = materialRequest.ToDomainObject(model.Id);
            }
            materialRepository.Add(material);
        }

        foreach (var sectionProfileRequest in req.Body.SectionProfileRequests ?? [])
        {
            SectionProfileInfoBase sectionProfile;
            if (
                sectionProfileRequest.Id.HasValue
                && sectionProfileStore.ContainsKey(sectionProfileRequest.Id.Value)
            )
            {
                sectionProfile = sectionProfileStore[sectionProfileRequest.Id.Value];
            }
            else
            {
                sectionProfile = sectionProfileRequest.ToDomainObject(model.Id);
            }
            if (sectionProfile is SectionProfile sp)
            {
                sectionProfileRepository.Add(sp);
            }
            else if (sectionProfile is SectionProfileFromLibrary spl)
            {
                sectionProfileFromLibraryRepository.Add(spl);
            }
        }

        foreach (var sectionProfileRequest in req.Body.SectionProfileFromLibraryRequests ?? [])
        {
            SectionProfileInfoBase sectionProfile;
            // if (
            //     sectionProfileRequest.Id.HasValue
            //     && sectionProfileStore.ContainsKey(sectionProfileRequest.Id.Value)
            // )
            // {
            //     sectionProfile = sectionProfileStore[sectionProfileRequest.Id.Value];
            // }
            if (
                sectionProfileStore.TryGetValue(
                    sectionProfileRequest.Id,
                    out var existingSectionProfile
                )
            )
            {
                sectionProfile = existingSectionProfile;
            }
            else
            {
                var sectionProfileNullable = SectionProfile.FromLibraryValue(
                    req.ModelId,
                    sectionProfileRequest.Library,
                    sectionProfileRequest.Name
                );
                if (sectionProfileNullable is null)
                {
                    return BeamOsError.NotFound(
                        description: $"Section profile named {sectionProfileRequest.Name} with code {sectionProfileRequest.Library} not found."
                    );
                }
                sectionProfile = sectionProfileNullable;
            }
            if (sectionProfile is SectionProfile sp)
            {
                sectionProfileRepository.Add(sp);
            }
            else if (sectionProfile is SectionProfileFromLibrary spl)
            {
                sectionProfileFromLibraryRepository.Add(spl);
            }
        }

        var response = new PatchModelResponse() { Element1dsToAddOrUpdateByExternalIdResults = [] };
        var nodeResponses = new List<NodeResponse>();
        foreach (var elementByLoc in req.Body.Element1dsToAddOrUpdateByExternalId ?? [])
        {
            var (startNode, isStartNodeNew) = this.GetOrAddNodeAtLocation(
                model,
                octree,
                nodeStore,
                elementByLoc.StartNodeLocation.ToDomain(),
                ref nextNodeId
            );
            if (isStartNodeNew)
            {
                nodeDefinitionRepository.Add(startNode);
            }
            var (endNode, isEndNodeNew) = this.GetOrAddNodeAtLocation(
                model,
                octree,
                nodeStore,
                elementByLoc.EndNodeLocation.ToDomain(),
                ref nextNodeId
            );
            if (isEndNodeNew)
            {
                nodeDefinitionRepository.Add(endNode);
            }
            var existingElement1d = element1dStore.Values.FirstOrDefault(e =>
                e.ExternalId == elementByLoc.ExternalId
            );
            Element1d element1d;
            bool isElementNew;
            if (existingElement1d is not null)
            {
                element1d = element1dStore[existingElement1d.Id];
                element1d.StartNodeId = startNode.Id;
                element1d.EndNodeId = endNode.Id;
                isElementNew = false;
            }
            else
            {
                nextElement1dId++;
                element1d = new Element1d(
                    model.Id,
                    startNode.Id,
                    endNode.Id,
                    1,
                    1,
                    new Element1dId(nextElement1dId),
                    elementByLoc.ExternalId
                );
                isElementNew = true;
            }
            if (isElementNew)
            {
                element1dRepository.Add(element1d);
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

    private (NodeDefinition node, bool isNew) GetOrAddNodeAtLocation(
        Model model,
        Octree octree,
        Dictionary<NodeId, NodeDefinition> nodeStore,
        Point location,
        ref int nextNodeId
    )
    {
        var nodeIds = octree.FindNodeIdsWithin(
            location,
            toleranceMeters: new Length(1, LengthUnit.Inch).Meters
        );

        if (nodeIds.Count == 0)
        {
            nextNodeId++;
            var newNode = new Node(model.Id, location, Restraint.Free, new NodeId(nextNodeId));
            octree.Add(newNode);
            return (newNode, true);
        }
        else
        {
            return (nodeStore[nodeIds[0]], false);
        }
    }
}
