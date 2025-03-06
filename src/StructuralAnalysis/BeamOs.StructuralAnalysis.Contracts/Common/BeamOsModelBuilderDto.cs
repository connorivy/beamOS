using System.Text.Json.Serialization;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public record BeamOsModelBuilderDto
{
    public string Name { get; init; }
    public string Description { get; init; }
    public ModelSettings Settings { get; init; }

    /// <summary>
    /// You can go to this website to generate a random guid string
    /// https://www.uuidgenerator.net/guid
    /// </summary>
    public string GuidString { get; init; }

    public IEnumerable<PutNodeRequest> Nodes { get; init; }
    public IEnumerable<PutMaterialRequest> Materials { get; init; }
    public IEnumerable<PutSectionProfileRequest> SectionProfiles { get; init; }
    public IEnumerable<PutElement1dRequest> Element1ds { get; init; }
    public IEnumerable<PutPointLoadRequest> PointLoads { get; init; }
    public IEnumerable<PutMomentLoadRequest> MomentLoads { get; init; }
}

public record SpeckleReceiveParameters(
    string ApiToken,
    string ProjectId,
    string ObjectId,
    string ServerUrl
);

[JsonPolymorphic]
[JsonDerivedType(typeof(PutNodeRequest), 1)]
[JsonDerivedType(typeof(PutElement1dRequest), 2)]
public interface IBeamOsEntityRequest { }
