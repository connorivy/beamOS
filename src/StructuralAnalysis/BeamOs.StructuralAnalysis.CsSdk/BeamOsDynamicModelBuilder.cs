using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.StructuralAnalysis.CsSdk;

public sealed class BeamOsDynamicModelBuilder(
    string guidString,
    PhysicalModelSettings physicalModelSettings,
    string name,
    string description
) : BeamOsModelBuilder
{
    public override string Name => name;
    public override string Description => description;
    public override PhysicalModelSettings Settings => physicalModelSettings;
    public override string GuidString => guidString;

    private readonly List<PutNodeRequest> nodes = [];

    public BeamOsDynamicModelBuilder(
        string guidString,
        PhysicalModelSettings physicalModelSettings,
        string name,
        string description,
        BeamOsModelBuilderDto beamOsModelBuilderDto
    )
        : this(guidString, physicalModelSettings, name, description)
    {
        this.nodes = beamOsModelBuilderDto.Nodes.ToList();
        this.materials = beamOsModelBuilderDto.Materials.ToList();
        this.element1ds = beamOsModelBuilderDto.Element1ds.ToList();
        this.pointLoads = beamOsModelBuilderDto.PointLoads.ToList();
        this.momentLoads = beamOsModelBuilderDto.MomentLoads.ToList();
        this.sectionProfiles = beamOsModelBuilderDto.SectionProfiles.ToList();
    }

    public void AddNodes(params Span<PutNodeRequest> nodes) => this.nodes.AddRange(nodes);

    public override IEnumerable<PutNodeRequest> NodeRequests() => nodes.AsReadOnly();

    private readonly List<PutElement1dRequest> element1ds = [];

    public void AddElement1ds(params Span<PutElement1dRequest> els) =>
        this.element1ds.AddRange(els);

    public override IEnumerable<PutElement1dRequest> Element1dRequests() => element1ds.AsReadOnly();

    private readonly List<PutMaterialRequest> materials = [];

    public void AddMaterials(params Span<PutMaterialRequest> materials) =>
        this.materials.AddRange(materials);

    public override IEnumerable<PutMaterialRequest> MaterialRequests() => materials.AsReadOnly();

    private readonly List<PutPointLoadRequest> pointLoads = [];

    public void AddPointLoads(params Span<PutPointLoadRequest> pointLoads) =>
        this.pointLoads.AddRange(pointLoads);

    public override IEnumerable<PutPointLoadRequest> PointLoadRequests() => pointLoads.AsReadOnly();

    private readonly List<PutMomentLoadRequest> momentLoads = [];

    public void AddMomentLoads(params Span<PutMomentLoadRequest> momentLoads) =>
        this.momentLoads.AddRange(momentLoads);

    public override IEnumerable<PutMomentLoadRequest> MomentLoadRequests() =>
        momentLoads.AsReadOnly();

    private readonly List<PutSectionProfileRequest> sectionProfiles = [];

    public void AddSectionProfiles(params Span<PutSectionProfileRequest> sectionProfiles) =>
        this.sectionProfiles.AddRange(sectionProfiles);

    public override IEnumerable<PutSectionProfileRequest> SectionProfileRequests() =>
        sectionProfiles.AsReadOnly();
}
