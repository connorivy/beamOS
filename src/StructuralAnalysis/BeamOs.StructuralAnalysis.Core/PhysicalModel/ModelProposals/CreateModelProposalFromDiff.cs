using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "proposals/from-diff")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
[BeamOsTag(BeamOsTags.AI)]
internal class CreateModelProposalFromDiff(
    CreateModelProposalFromDiffCommandHandler createModelProposalFromDiffCommandHandler,
    IStructuralAnalysisUnitOfWork unitOfWork
) : BeamOsModelResourceBaseEndpoint<ModelDiffData, ModelProposalResponse>
{
    public override async Task<Result<ModelProposalResponse>> ExecuteRequestAsync(
        ModelResourceRequest<ModelDiffData> req,
        CancellationToken ct = default
    )
    {
        var result = await createModelProposalFromDiffCommandHandler.ExecuteAsync(req, ct);
        await unitOfWork.SaveChangesAsync(ct);
        if (result.IsError)
        {
            return result.Error;
        }
        return result.Value.ToContract();
    }
}

internal class CreateModelProposalFromDiffCommandHandler(
    CreateModelProposalCommandHandler createModelProposalCommandHandler
) : ICommandHandler<ModelResourceRequest<ModelDiffData>, ModelProposal>
{
    public async Task<Result<ModelProposal>> ExecuteAsync(
        ModelResourceRequest<ModelDiffData> command,
        CancellationToken ct = default
    )
    {
        var diffData = command.Body;

        // Build up a ModelProposalData from the diff
        var modelProposalData = new ModelProposalData
        {
            CreateNodeProposals = [],
            ModifyNodeProposals = [],
            CreateElement1dProposals = [],
            ModifyElement1dProposals = [],
            CreateMaterialProposals = [],
            ModifyMaterialProposals = [],
            CreateSectionProfileProposals = [],
            ModifySectionProfileProposals = [],
            DeleteModelEntityProposals = [],
        };

        // Process nodes
        foreach (var nodeDiff in diffData.Nodes)
        {
            switch (nodeDiff.Status)
            {
                case DiffStatus.Added:
                    modelProposalData.CreateNodeProposals.Add(
                        new CreateNodeRequest(
                            nodeDiff.TargetEntity.LocationPoint,
                            nodeDiff.TargetEntity.Restraint,
                            nodeDiff.TargetEntity.Id,
                            nodeDiff.TargetEntity.Metadata
                        )
                    );
                    break;
                case DiffStatus.Modified:
                    modelProposalData.ModifyNodeProposals.Add(
                        new PutNodeRequest(
                            nodeDiff.TargetEntity.Id,
                            nodeDiff.TargetEntity.LocationPoint,
                            nodeDiff.TargetEntity.Restraint,
                            nodeDiff.TargetEntity.Metadata
                        )
                    );
                    break;
                case DiffStatus.Removed:
                    modelProposalData.DeleteModelEntityProposals.Add(
                        new DeleteModelEntityProposalData
                        {
                            Id = nodeDiff.SourceEntity.Id,
                            ObjectType = BeamOsObjectType.Node,
                        }
                    );
                    break;
                default:
                    return BeamOsError.Validation(
                        description: $"Unknown DiffStatus: {nodeDiff.Status}"
                    );
            }
        }

        // Build lookup sets for newly added entities in this diff
        var addedNodeIds = diffData.Nodes
            .Where(n => n.Status == DiffStatus.Added)
            .Select(n => n.TargetEntity.Id)
            .ToHashSet();
        var addedMaterialIds = diffData.Materials
            .Where(m => m.Status == DiffStatus.Added)
            .Select(m => m.TargetEntity.Id)
            .ToHashSet();
        var addedSectionProfileIds = diffData.SectionProfiles
            .Where(s => s.Status == DiffStatus.Added)
            .Select(s => s.TargetEntity.Id)
            .ToHashSet();

        // Process element1ds
        foreach (var element1dDiff in diffData.Element1ds)
        {
            switch (element1dDiff.Status)
            {
                case DiffStatus.Added:
                    // Check if referenced entities are being added in this diff - if so, use Proposed, otherwise Existing
                    var startNodeId = addedNodeIds.Contains(element1dDiff.TargetEntity.StartNodeId)
                        ? ProposedID.Proposed(element1dDiff.TargetEntity.StartNodeId)
                        : ProposedID.Existing(element1dDiff.TargetEntity.StartNodeId);
                    var endNodeId = addedNodeIds.Contains(element1dDiff.TargetEntity.EndNodeId)
                        ? ProposedID.Proposed(element1dDiff.TargetEntity.EndNodeId)
                        : ProposedID.Existing(element1dDiff.TargetEntity.EndNodeId);
                    var materialId = addedMaterialIds.Contains(element1dDiff.TargetEntity.MaterialId)
                        ? ProposedID.Proposed(element1dDiff.TargetEntity.MaterialId)
                        : ProposedID.Existing(element1dDiff.TargetEntity.MaterialId);
                    var sectionProfileId = addedSectionProfileIds.Contains(element1dDiff.TargetEntity.SectionProfileId)
                        ? ProposedID.Proposed(element1dDiff.TargetEntity.SectionProfileId)
                        : ProposedID.Existing(element1dDiff.TargetEntity.SectionProfileId);
                    
                    modelProposalData.CreateElement1dProposals.Add(
                        Element1dProposalBase.Create(
                            startNodeId,
                            endNodeId,
                            materialId,
                            sectionProfileId,
                            element1dDiff.TargetEntity.SectionProfileRotation,
                            element1dDiff.TargetEntity.Metadata,
                            element1dDiff.TargetEntity.Id
                        )
                    );
                    break;
                case DiffStatus.Modified:
                    // For modified elements, check if referenced entities are being added in this diff
                    var modStartNodeId = addedNodeIds.Contains(element1dDiff.TargetEntity.StartNodeId)
                        ? ProposedID.Proposed(element1dDiff.TargetEntity.StartNodeId)
                        : ProposedID.Existing(element1dDiff.TargetEntity.StartNodeId);
                    var modEndNodeId = addedNodeIds.Contains(element1dDiff.TargetEntity.EndNodeId)
                        ? ProposedID.Proposed(element1dDiff.TargetEntity.EndNodeId)
                        : ProposedID.Existing(element1dDiff.TargetEntity.EndNodeId);
                    var modMaterialId = addedMaterialIds.Contains(element1dDiff.TargetEntity.MaterialId)
                        ? ProposedID.Proposed(element1dDiff.TargetEntity.MaterialId)
                        : ProposedID.Existing(element1dDiff.TargetEntity.MaterialId);
                    var modSectionProfileId = addedSectionProfileIds.Contains(element1dDiff.TargetEntity.SectionProfileId)
                        ? ProposedID.Proposed(element1dDiff.TargetEntity.SectionProfileId)
                        : ProposedID.Existing(element1dDiff.TargetEntity.SectionProfileId);
                    
                    modelProposalData.ModifyElement1dProposals.Add(
                        Element1dProposalBase.Modify(
                            element1dDiff.TargetEntity.Id,
                            modStartNodeId,
                            modEndNodeId,
                            modMaterialId,
                            modSectionProfileId,
                            element1dDiff.TargetEntity.SectionProfileRotation,
                            element1dDiff.TargetEntity.Metadata
                        )
                    );
                    break;
                case DiffStatus.Removed:
                    modelProposalData.DeleteModelEntityProposals.Add(
                        new DeleteModelEntityProposalData
                        {
                            Id = element1dDiff.SourceEntity.Id,
                            ObjectType = BeamOsObjectType.Element1d,
                        }
                    );
                    break;
                default:
                    return BeamOsError.Validation(
                        description: $"Unknown DiffStatus: {element1dDiff.Status}"
                    );
            }
        }

        // Process materials
        foreach (var materialDiff in diffData.Materials)
        {
            switch (materialDiff.Status)
            {
                case DiffStatus.Added:
                    modelProposalData.CreateMaterialProposals.Add(
                        new CreateMaterialRequest
                        {
                            Id = materialDiff.TargetEntity.Id,
                            ModulusOfElasticity = materialDiff.TargetEntity.ModulusOfElasticity,
                            ModulusOfRigidity = materialDiff.TargetEntity.ModulusOfRigidity,
                            PressureUnit = materialDiff.TargetEntity.PressureUnit,
                        }
                    );
                    break;
                case DiffStatus.Modified:
                    modelProposalData.ModifyMaterialProposals.Add(
                        new PutMaterialRequest
                        {
                            Id = materialDiff.TargetEntity.Id,
                            ModulusOfElasticity = materialDiff.TargetEntity.ModulusOfElasticity,
                            ModulusOfRigidity = materialDiff.TargetEntity.ModulusOfRigidity,
                            PressureUnit = materialDiff.TargetEntity.PressureUnit,
                        }
                    );
                    break;
                case DiffStatus.Removed:
                    modelProposalData.DeleteModelEntityProposals.Add(
                        new DeleteModelEntityProposalData
                        {
                            Id = materialDiff.SourceEntity.Id,
                            ObjectType = BeamOsObjectType.Material,
                        }
                    );
                    break;
                default:
                    return BeamOsError.Validation(
                        description: $"Unknown DiffStatus: {materialDiff.Status}"
                    );
            }
        }

        // Process section profiles
        foreach (var sectionProfileDiff in diffData.SectionProfiles)
        {
            switch (sectionProfileDiff.Status)
            {
                case DiffStatus.Added:
                    modelProposalData.CreateSectionProfileProposals.Add(
                        new CreateSectionProfileRequest(
                            sectionProfileDiff.TargetEntity.ToSectionProfileData()
                        )
                        {
                            Id = sectionProfileDiff.TargetEntity.Id,
                        }
                    );
                    break;
                case DiffStatus.Modified:
                    modelProposalData.ModifySectionProfileProposals.Add(
                        new PutSectionProfileRequest(
                            sectionProfileDiff.TargetEntity.Id,
                            sectionProfileDiff.TargetEntity.ToSectionProfileData()
                        )
                    );
                    break;
                case DiffStatus.Removed:
                    modelProposalData.DeleteModelEntityProposals.Add(
                        new DeleteModelEntityProposalData
                        {
                            Id = sectionProfileDiff.SourceEntity.Id,
                            ObjectType = BeamOsObjectType.SectionProfile,
                        }
                    );
                    break;
                default:
                    return BeamOsError.Validation(
                        description: $"Unknown DiffStatus: {sectionProfileDiff.Status}"
                    );
            }
        }

        var proposalRequest = new ModelResourceRequest<ModelProposalData>
        {
            ModelId = command.ModelId,
            Body = modelProposalData,
        };

        return await createModelProposalCommandHandler.ExecuteAsync(proposalRequest, ct);
    }
}
