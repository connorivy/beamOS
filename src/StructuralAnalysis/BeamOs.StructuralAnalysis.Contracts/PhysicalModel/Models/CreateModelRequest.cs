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

public record ModelData
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required ModelSettings Settings { get; init; }
    public List<PutNodeRequest>? Nodes { get; init; }
    public List<PutElement1dRequest>? Element1ds { get; init; }
    public List<PutMaterialRequest>? Materials { get; init; }
    public List<PutSectionProfileRequest>? SectionProfiles { get; init; }
    public List<SectionProfileFromLibrary>? SectionProfilesFromLibrary { get; init; }
    public List<PointLoad>? PointLoads { get; init; }
    public List<MomentLoad>? MomentLoads { get; init; }
    public List<ResultSet>? ResultSets { get; init; }
    public List<LoadCase>? LoadCases { get; init; }
    public List<LoadCombination>? LoadCombinations { get; init; }
}

public record ModelInfo : ModelInfoData
{
    public required Guid Id { get; init; }
    public required DateTimeOffset LastModified { get; init; }
}
