using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithoutTrailingSlash)]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
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
    ILoadCaseRepository loadCaseRepository,
    IPointLoadRepository pointLoadRepository,
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
            nameof(Model.SectionProfilesFromLibrary),
            nameof(Model.LoadCases),
            nameof(Model.PointLoads)
        );

        if (model is null)
        {
            return BeamOsError.NotFound(
                description: $"Model with id '{req.ModelId}' was not found."
            );
        }
        if (model.Materials is null || model.Materials.Count == 0)
        {
            return BeamOsError.InvalidOperation(
                description: "Model must have at least one material defined."
            );
        }
        var defaultMaterial = model.Materials[0];
        if (
            model.SectionProfiles is null
            || model.SectionProfiles.Count == 0
            || model.SectionProfilesFromLibrary is null
            || model.SectionProfilesFromLibrary.Count == 0
        )
        {
            return BeamOsError.InvalidOperation(
                description: "Model must have at least one section profile defined."
            );
        }
        var defaultSectionProfile =
            model.SectionProfiles.FirstOrDefault<SectionProfileInfoBase>()
            ?? model.SectionProfilesFromLibrary.First();

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
        var loadCaseStore = (model.LoadCases ?? []).ToDictionary(lc => lc.Id);
        var pointLoadStore = (model.PointLoads ?? []).ToDictionary(pl => pl.Id);

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
                await materialRepository.Put(material);
            }
            else
            {
                material = materialRequest.ToDomainObject(model.Id);
                materialRepository.Add(material);
            }
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
            await this.AddToRepo(sectionProfile);
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
                sectionProfile.SetIntId(sectionProfileRequest.Id);
            }
            await this.AddToRepo(sectionProfile);
        }

        // Process LoadCases
        foreach (var loadCaseContract in req.Body.LoadCases ?? [])
        {
            LoadCase loadCase;
            // Check if we should use an existing LoadCase or create a new one
            // If Id is provided and > 0 and exists in store, we skip it (assume it's already there)
            // Otherwise, create a new LoadCase
            if (loadCaseContract.Id > 0 && loadCaseStore.ContainsKey(new LoadCaseId(loadCaseContract.Id)))
            {
                // LoadCase already exists, skip
                continue;
            }
            else
            {
                // Create new LoadCase
                // If Id is > 0, use it; otherwise let EF Core assign one
                var loadCaseId = loadCaseContract.Id > 0 ? new LoadCaseId(loadCaseContract.Id) : (LoadCaseId?)null;
                loadCase = new LoadCase(model.Id, loadCaseContract.Name, loadCaseId);
                loadCaseRepository.Add(loadCase);
                loadCaseStore[loadCase.Id] = loadCase;
            }
        }

        // Process PointLoads
        foreach (var pointLoadRequest in req.Body.PointLoads ?? [])
        {
            PointLoad pointLoad;
            // Check if we should use an existing PointLoad or create a new one
            var hasId = pointLoadRequest.Id.HasValue && pointLoadRequest.Id.Value > 0;
            if (hasId && pointLoadStore.ContainsKey(new PointLoadId(pointLoadRequest.Id!.Value)))
            {
                // PointLoad already exists, skip or update
                continue;
            }
            else
            {
                // Create new PointLoad
                // If Id is > 0, use it; otherwise let EF Core assign one
                PointLoadId? pointLoadId = hasId ? new PointLoadId(pointLoadRequest.Id!.Value) : null;
                
                var nodeId = new NodeId(pointLoadRequest.NodeId);
                var loadCaseId = new LoadCaseId(pointLoadRequest.LoadCaseId);
                
                pointLoad = new PointLoad(
                    model.Id,
                    nodeId,
                    loadCaseId,
                    pointLoadRequest.Force.MapToForce(),
                    pointLoadRequest.Direction.ToDomain(),
                    pointLoadId
                );
                pointLoadRepository.Add(pointLoad);
                pointLoadStore[pointLoad.Id] = pointLoad;
            }
        }

        var response = new PatchModelResponse() { Element1dsToAddOrUpdateByExternalIdResults = [] };
        var nodeResponses = new List<NodeResponse>();
        foreach (var elementByLoc in req.Body.Element1dsToAddOrUpdateByExternalId ?? [])
        {
            var startNode = await this.GetOrAddNodeAtLocation(
                model.Id,
                octree,
                elementByLoc.StartNodeLocation.ToDomain()
            );
            var endNode = await this.GetOrAddNodeAtLocation(
                model.Id,
                octree,
                elementByLoc.EndNodeLocation.ToDomain()
            );

            Element1d? existingElement1d;
            if (startNode.Id != 0 && endNode.Id != 0)
            {
                existingElement1d = element1dStore.Values.FirstOrDefault(e =>
                    e.StartNodeId == startNode.Id && e.EndNodeId == endNode.Id
                );
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
                element1d = new Element1d(
                    model.Id,
                    startNode.Id,
                    endNode.Id,
                    defaultMaterial.Id,
                    defaultSectionProfile.Id
                )
                {
                    StartNode = startNode,
                    EndNode = endNode,
                };
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

    private async Task AddToRepo(SectionProfileInfoBase sectionProfile, bool isNew = false)
    {
        if (sectionProfile is SectionProfile sp)
        {
            if (isNew)
            {
                sectionProfileRepository.Add(sp);
            }
            else
            {
                await sectionProfileRepository.Put(sp);
            }
        }
        else if (sectionProfile is SectionProfileFromLibrary spl)
        {
            if (isNew)
            {
                sectionProfileFromLibraryRepository.Add(spl);
            }
            else
            {
                await sectionProfileFromLibraryRepository.Put(spl);
            }
        }
    }

    private async Task<NodeDefinition> GetOrAddNodeAtLocation(
        ModelId modelId,
        Octree octree,
        Point location
    )
    {
        var nodes = octree.FindNodesWithin(
            location,
            toleranceMeters: new Length(1, LengthUnit.Inch).Meters
        );

        if (nodes.Count > 0)
        {
            return nodes[0];
        }

        var node = new Node(modelId, location, Restraint.Free);
        nodeDefinitionRepository.Add(node);
        octree.Add(node);
        return node;
    }
}
