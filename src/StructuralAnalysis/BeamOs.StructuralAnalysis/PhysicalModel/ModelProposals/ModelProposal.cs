using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

public record ModelProposal : ModelProposalData, IHasIntId
{
    public required int Id { get; init; }
    public required DateTimeOffset LastModified { get; init; }
}

public record ModelProposalInfoData
{
    public string? Description { get; init; }
}

public record ModelProposalInfo : ModelProposalInfoData, IHasIntId
{
    public required int Id { get; init; }
    public required DateTimeOffset LastModified { get; init; }
}

public record ModelProposalData : ModelProposalInfoData
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public ModelSettings? Settings { get; init; }
    public List<CreateNodeRequest>? CreateNodeProposals { get; init; }
    public List<PutNodeRequest>? ModifyNodeProposals { get; init; }
    public List<CreateElement1dProposal>? CreateElement1dProposals { get; init; }
    public List<ModifyElement1dProposal>? ModifyElement1dProposals { get; init; }
    public List<CreateMaterialRequest>? CreateMaterialProposals { get; init; }
    public List<PutMaterialRequest>? ModifyMaterialProposals { get; init; }
    public List<CreateSectionProfileRequest>? CreateSectionProfileProposals { get; init; }
    public List<PutSectionProfileRequest>? ModifySectionProfileProposals { get; init; }
    public List<CreateSectionProfileFromLibraryRequest>? CreateSectionProfileFromLibraryProposals { get; init; }
    public List<PointLoadContract>? PointLoadProposals { get; init; }
    public List<MomentLoadContract>? MomentLoadProposals { get; init; }
    public List<ResultSet>? ResultSetProposals { get; init; }
    public List<LoadCaseContract>? LoadCaseProposals { get; init; }
    public List<LoadCombinationContract>? LoadCombinationProposals { get; init; }
    public List<ProposalIssueData>? ProposalIssues { get; init; }
    public List<DeleteModelEntityProposalData>? DeleteModelEntityProposals { get; init; }
}

public record ModelProposalResponse : IHasIntId, IBeamOsEntityResponse
{
    public required int Id { get; init; }
    public required DateTimeOffset LastModified { get; init; }
    public ModelProposalInfo? ModelProposal { get; init; }
    public List<CreateNodeProposalResponse>? CreateNodeProposals { get; init; }
    public List<ModifyNodeProposalResponse>? ModifyNodeProposals { get; init; }
    public List<CreateInternalNodeProposalResponse>? CreateInternalNodeProposals { get; init; }
    public List<ModifyInternalNodeProposalResponse>? ModifyInternalNodeProposals { get; init; }
    public List<CreateElement1dProposalResponse>? CreateElement1dProposals { get; init; }
    public List<ModifyElement1dProposalResponse>? ModifyElement1dProposals { get; init; }
    public List<int>? Element1dsModifiedBecauseOfNodeChange { get; init; }
    public List<PutMaterialRequest>? MaterialProposals { get; init; }
    public List<PutSectionProfileRequest>? SectionProfileProposals { get; init; }
    public List<SectionProfileFromLibrary>? SectionProfileFromLibraryProposals { get; init; }
    public List<PointLoadContract>? PointLoadProposals { get; init; }
    public List<MomentLoadContract>? MomentLoadProposals { get; init; }
    public List<ResultSet>? ResultSetProposals { get; init; }
    public List<LoadCaseContract>? LoadCaseProposals { get; init; }
    public List<LoadCombinationContract>? LoadCombinationProposals { get; init; }
    public List<ProposalIssue>? ProposalIssues { get; init; }
    public List<DeleteModelEntityProposal>? DeleteModelEntityProposals { get; init; }
}

public enum ProposalIssueSeverity
{
    Undefined = 0,

    /// <summary>
    /// The proposed object will be created, but there is some information that the user should be aware of
    /// </summary>
    Information = 10,

    /// <summary>
    /// The proposed object will be created, but some data may have been inferred due to missing information
    /// </summary>
    Warning = 20,

    /// <summary>
    /// The proposed object will fail to be created
    /// </summary>
    Error = 30,

    /// <summary>
    /// The entire proposal will fail to be applied
    /// </summary>
    Critical = 40,
}

public record ProposalIssueData
{
    public required ProposedID ProposedId { get; init; }
    public required BeamOsObjectType ObjectType { get; init; }
    public required string Message { get; init; }
    public required ProposalIssueSeverity Severity { get; init; }
    public required ProposalIssueCode Code { get; init; }
}

public record ProposalIssue : ProposalIssueData, IHasIntId
{
    public required int Id { get; init; }
}

public enum ProposalIssueCode
{
    Undefined = 0,
    Other,
    CouldNotCreateProposedObject,
    SomeInfoInferred,
    SwappedInPlaceholder,
}

public record DeleteModelEntityProposalData
{
    public required int ModelEntityId { get; init; }
    public required BeamOsObjectType ObjectType { get; init; }
}

public record DeleteModelEntityProposal : DeleteModelEntityProposalData, IHasIntId, IEntityProposal
{
    public required int Id { get; init; }
    public ProposalType ProposalType => ProposalType.Delete;
}

public record ModelRepairOperationParameters
{
    public required LengthContract FavorableOperationTolerance { get; init; }
    public required LengthContract StandardOperationTolerance { get; init; }
    public required LengthContract UnfavorableOperationTolerance { get; init; }
}

public record AcceptModelProposalRequest : IModelEntity, IBeamOsEntityRequest
{
    public Guid ModelId { get; }

    public int Id { get; }
    public List<ModelEntityId>? ModelEntityIdsToIgnore { get; init; }
}
