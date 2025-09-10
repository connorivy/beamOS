using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Sdk;

public interface IBeamOsModel
{
    public string Name { get; }
    public string Description { get; }
    public ModelSettings Settings { get; }
    public UnitSettings UnitSettings => this.Settings.UnitSettings;
    public DateTimeOffset LastModified { get; }

    /// <summary>
    /// You can go to this website to generate a random guid string
    /// https://www.uuidgenerator.net/guid
    /// and then use Guid.Parse("your-guid-string") to convert it to a Guid
    /// </summary>
    public Guid Id { get; }

    public IEnumerable<PutNodeRequest> Nodes => this.NodeRequests();
    public IEnumerable<PutNodeRequest> NodeRequests();
    public IEnumerable<InternalNode> InternalNodes => this.InternalNodeRequests();

    public IEnumerable<InternalNode> InternalNodeRequests() => [];

    public IEnumerable<PutMaterialRequest> Materials => this.MaterialRequests();
    public IEnumerable<PutMaterialRequest> MaterialRequests();
    public IEnumerable<PutSectionProfileRequest> SectionProfiles => this.SectionProfileRequests();

    public IEnumerable<PutSectionProfileRequest> SectionProfileRequests() => [];

    public IEnumerable<SectionProfileFromLibrary> SectionProfilesFromLibrary =>
        this.SectionProfilesFromLibraryRequests();

    public IEnumerable<SectionProfileFromLibrary> SectionProfilesFromLibraryRequests() => [];

    public IEnumerable<PutElement1dRequest> Element1ds => this.Element1dRequests();
    public IEnumerable<PutElement1dRequest> Element1dRequests();
    public IEnumerable<PutPointLoadRequest> PointLoads => this.PointLoadRequests();

    public IEnumerable<PutPointLoadRequest> PointLoadRequests() => [];

    public IEnumerable<PutMomentLoadRequest> MomentLoads => this.MomentLoadRequests();

    public IEnumerable<PutMomentLoadRequest> MomentLoadRequests() => [];

    public IEnumerable<LoadCaseContract> LoadCases => this.LoadCaseRequests();

    public IEnumerable<LoadCaseContract> LoadCaseRequests();

    public IEnumerable<LoadCombinationContract> LoadCombinations => this.LoadCombinationRequests();

    public IEnumerable<LoadCombinationContract> LoadCombinationRequests();
}
