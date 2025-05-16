using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

public record CreateModelRequest : ModelInfoData
{
    public Guid? Id { get; init; }
}

public record ModelInfoData
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required ModelSettings Settings { get; init; }
}

public record ModelInfo : ModelInfoData
{
    public required Guid Id { get; init; }
    public required DateTimeOffset LastModified { get; init; }
}

public record ModelProposalData
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public ModelSettings? Settings { get; init; }
    public List<PutNodeRequest>? NodeProposals { get; init; }
    public List<Element1dProposal>? Element1dProposals { get; init; }
    public List<PutMaterialRequest>? MaterialProposals { get; init; }
    public List<PutSectionProfileRequest>? SectionProfileProposals { get; init; }
    public List<SectionProfileFromLibrary>? SectionProfileFromLibraryProposals { get; init; }
    public List<PointLoad>? PointLoadProposals { get; init; }
    public List<MomentLoad>? MomentLoadProposals { get; init; }
    public List<ResultSet>? ResultSetProposals { get; init; }
    public List<LoadCase>? LoadCaseProposals { get; init; }
    public List<LoadCombination>? LoadCombinationProposals { get; init; }
}

public record ModelProposal : ModelProposalData, IHasIntId
{
    public required int Id { get; init; }
}
