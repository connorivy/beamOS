using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
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
    CreateModelProposalFromDiffCommandHandler createModelProposalFromDiffCommandHandler
) : BeamOsModelResourceBaseEndpoint<ModelDiffData, ModelProposalResponse>
{
    public override async Task<Result<ModelProposalResponse>> ExecuteRequestAsync(
        ModelResourceRequest<ModelDiffData> req,
        CancellationToken ct = default
    ) => await createModelProposalFromDiffCommandHandler.ExecuteAsync(req, ct);
}

internal class CreateModelProposalFromDiffCommandHandler(
    CreateModelProposalCommandHandler createModelProposalCommandHandler
) : ICommandHandler<ModelResourceRequest<ModelDiffData>, ModelProposalResponse>
{
    public async Task<Result<ModelProposalResponse>> ExecuteAsync(
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
                            nodeDiff.Entity.LocationPoint,
                            nodeDiff.Entity.Restraint,
                            nodeDiff.Entity.Id,
                            nodeDiff.Entity.Metadata
                        )
                    );
                    break;
                case DiffStatus.Modified:
                    modelProposalData.ModifyNodeProposals.Add(
                        new PutNodeRequest(
                            nodeDiff.Entity.Id,
                            nodeDiff.Entity.LocationPoint,
                            nodeDiff.Entity.Restraint,
                            nodeDiff.Entity.Metadata
                        )
                    );
                    break;
                case DiffStatus.Removed:
                    modelProposalData.DeleteModelEntityProposals.Add(
                        new DeleteModelEntityProposalData
                        {
                            Id = nodeDiff.Entity.Id,
                            ObjectType = BeamOsObjectType.Node,
                        }
                    );
                    break;
            }
        }

        // Process element1ds
        foreach (var element1dDiff in diffData.Element1ds)
        {
            switch (element1dDiff.Status)
            {
                case DiffStatus.Added:
                    modelProposalData.CreateElement1dProposals.Add(
                        Element1dProposalBase.Create(
                            ProposedID.Existing(element1dDiff.Entity.StartNodeId),
                            ProposedID.Existing(element1dDiff.Entity.EndNodeId),
                            ProposedID.Existing(element1dDiff.Entity.MaterialId),
                            ProposedID.Existing(element1dDiff.Entity.SectionProfileId),
                            element1dDiff.Entity.SectionProfileRotation,
                            element1dDiff.Entity.Metadata,
                            element1dDiff.Entity.Id
                        )
                    );
                    break;
                case DiffStatus.Modified:
                    modelProposalData.ModifyElement1dProposals.Add(
                        Element1dProposalBase.Modify(
                            element1dDiff.Entity.Id,
                            ProposedID.Existing(element1dDiff.Entity.StartNodeId),
                            ProposedID.Existing(element1dDiff.Entity.EndNodeId),
                            ProposedID.Existing(element1dDiff.Entity.MaterialId),
                            ProposedID.Existing(element1dDiff.Entity.SectionProfileId),
                            element1dDiff.Entity.SectionProfileRotation,
                            element1dDiff.Entity.Metadata
                        )
                    );
                    break;
                case DiffStatus.Removed:
                    modelProposalData.DeleteModelEntityProposals.Add(
                        new DeleteModelEntityProposalData
                        {
                            Id = element1dDiff.Entity.Id,
                            ObjectType = BeamOsObjectType.Element1d,
                        }
                    );
                    break;
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
                            Id = materialDiff.Entity.Id,
                            ModulusOfElasticity = materialDiff.Entity.ModulusOfElasticity,
                            ModulusOfRigidity = materialDiff.Entity.ModulusOfRigidity,
                            PressureUnit = materialDiff.Entity.PressureUnit,
                        }
                    );
                    break;
                case DiffStatus.Modified:
                    modelProposalData.ModifyMaterialProposals.Add(
                        new PutMaterialRequest
                        {
                            Id = materialDiff.Entity.Id,
                            ModulusOfElasticity = materialDiff.Entity.ModulusOfElasticity,
                            ModulusOfRigidity = materialDiff.Entity.ModulusOfRigidity,
                            PressureUnit = materialDiff.Entity.PressureUnit,
                        }
                    );
                    break;
                case DiffStatus.Removed:
                    modelProposalData.DeleteModelEntityProposals.Add(
                        new DeleteModelEntityProposalData
                        {
                            Id = materialDiff.Entity.Id,
                            ObjectType = BeamOsObjectType.Material,
                        }
                    );
                    break;
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
                            sectionProfileDiff.Entity.ToSectionProfileData()
                        )
                        {
                            Id = sectionProfileDiff.Entity.Id,
                        }
                    );
                    break;
                case DiffStatus.Modified:
                    modelProposalData.ModifySectionProfileProposals.Add(
                        new PutSectionProfileRequest(
                            sectionProfileDiff.Entity.Id,
                            sectionProfileDiff.Entity.ToSectionProfileData()
                        )
                    );
                    break;
                case DiffStatus.Removed:
                    modelProposalData.DeleteModelEntityProposals.Add(
                        new DeleteModelEntityProposalData
                        {
                            Id = sectionProfileDiff.Entity.Id,
                            ObjectType = BeamOsObjectType.SectionProfile,
                        }
                    );
                    break;
            }
        }

        // Call the existing CreateModelProposalCommandHandler with the built data
        var proposalRequest = new ModelResourceRequest<ModelProposalData>
        {
            ModelId = command.ModelId,
            Body = modelProposalData,
        };

        return await createModelProposalCommandHandler.ExecuteAsync(proposalRequest, ct);
    }
}
