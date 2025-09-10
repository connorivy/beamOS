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

public abstract class BeamOsStaticModelBase : IBeamOsModel
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract ModelSettings Settings { get; }
    public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// You can go to this website to generate a random guid string
    /// https://www.uuidgenerator.net/guid
    /// </summary>
    public abstract string GuidString { get; }
    public Guid Id => Guid.Parse(this.GuidString);

    public abstract IEnumerable<PutNodeRequest> NodeRequests();

    public virtual IEnumerable<InternalNodeContract> InternalNodeRequests() => [];

    public abstract IEnumerable<PutMaterialRequest> MaterialRequests();

    public virtual IEnumerable<PutSectionProfileRequest> SectionProfileRequests() => [];

    public virtual IEnumerable<SectionProfileFromLibraryContract> SectionProfilesFromLibraryRequests() =>
        [];

    public abstract IEnumerable<PutElement1dRequest> Element1dRequests();

    public virtual IEnumerable<PutPointLoadRequest> PointLoadRequests() => [];

    public virtual IEnumerable<PutMomentLoadRequest> MomentLoadRequests() => [];

    public abstract IEnumerable<LoadCaseContract> LoadCaseRequests();
    public abstract IEnumerable<LoadCombinationContract> LoadCombinationRequests();
}
