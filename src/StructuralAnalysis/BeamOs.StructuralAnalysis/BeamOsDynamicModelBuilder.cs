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

public sealed class BeamOsDynamicModel(
    Guid id,
    ModelSettings physicalModelSettings,
    string name,
    string description
) : IBeamOsModel
{
    public string Name => name;
    public string Description => description;
    public ModelSettings Settings => physicalModelSettings;
    public Guid Id => id;
    public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;

    private readonly List<PutNodeRequest> nodes = [];

    public UnitSettingsContract UnitSettings => this.Settings.UnitSettings;

    public BeamOsDynamicModel(
        Guid modelId,
        ModelSettings physicalModelSettings,
        string name,
        string description,
        BeamOsModelBuilderDto beamOsModelBuilderDto
    )
        : this(modelId, physicalModelSettings, name, description)
    {
        this.nodes = beamOsModelBuilderDto.Nodes.ToList();
        this.materials = beamOsModelBuilderDto.Materials.ToList();
        this.element1ds = beamOsModelBuilderDto.Element1ds.ToList();
        this.pointLoads = beamOsModelBuilderDto.PointLoads.ToList();
        this.momentLoads = beamOsModelBuilderDto.MomentLoads.ToList();
        this.sectionProfiles = beamOsModelBuilderDto.SectionProfiles.ToList();
    }

    public void AddNode(
        int id,
        double x,
        double y,
        double z,
        RestraintContract? restraint = null
    ) =>
        this.AddNodes(
            new PutNodeRequest()
            {
                Id = id,
                LocationPoint = new(x, y, z, this.UnitSettings.LengthUnit),
                Restraint = restraint ?? RestraintContract.Free,
            }
        );

    public void AddNodes(params Span<PutNodeRequest> nodes) => this.nodes.AddRange(nodes);

    public IEnumerable<PutNodeRequest> NodeRequests() => this.nodes.AsReadOnly();

    private readonly List<PutElement1dRequest> element1ds = [];

    public void AddElement1d(
        int id,
        int startNodeId,
        int endNodeId,
        int materialId,
        int sectionProfileId,
        AngleContract? sectionProfileRotation = null
    ) =>
        this.AddElement1ds(
            new PutElement1dRequest(
                id,
                startNodeId,
                endNodeId,
                materialId,
                sectionProfileId,
                sectionProfileRotation ?? new(0, AngleUnitContract.Radian)
            )
        );

    public void AddElement1ds(params Span<PutElement1dRequest> els) =>
        this.element1ds.AddRange(els);

    public IEnumerable<PutElement1dRequest> Element1dRequests() => this.element1ds.AsReadOnly();

    private readonly List<PutMaterialRequest> materials = [];

    public void AddMaterial(int id, double modulusOfElasticity, double modulusOfRigidity) =>
        this.AddMaterials(
            new PutMaterialRequest()
            {
                Id = id,
                ModulusOfElasticity = modulusOfElasticity,
                ModulusOfRigidity = modulusOfRigidity,
                PressureUnit = this.UnitSettings.PressureUnit,
            }
        );

    public void AddMaterials(params Span<PutMaterialRequest> materials) =>
        this.materials.AddRange(materials);

    public IEnumerable<PutMaterialRequest> MaterialRequests() => this.materials.AsReadOnly();

    private readonly List<LoadCaseContract> loadCases = [];

    public void AddLoadCase(int id, string caseName) =>
        this.AddLoadCases(new LoadCaseContract() { Id = id, Name = caseName });

    public void AddLoadCases(params Span<LoadCaseContract> loadCases) =>
        this.loadCases.AddRange(loadCases);

    public IEnumerable<LoadCaseContract> LoadCaseRequests() => this.loadCases.AsReadOnly();

    private readonly List<LoadCombinationContract> loadCombinations = [];

    public void AddLoadCombination(int id, params Span<(int, double)> loadCaseFactor) =>
        this.AddLoadCombinations(new LoadCombinationContract(id, loadCaseFactor));

    public void AddLoadCombinations(params Span<LoadCombinationContract> loadCombinations) =>
        this.loadCombinations.AddRange(loadCombinations);

    public IEnumerable<LoadCombinationContract> LoadCombinationRequests() =>
        this.loadCombinations.AsReadOnly();

    private readonly List<PutPointLoadRequest> pointLoads = [];

    public void AddPointLoad(int id, int nodeId, int loadCaseId, double force, Vector3 direction) =>
        this.AddPointLoads(
            new PutPointLoadRequest()
            {
                Id = id,
                NodeId = nodeId,
                LoadCaseId = loadCaseId,
                Force = new(force, this.UnitSettings.ForceUnit),
                Direction = direction,
            }
        );

    public void AddPointLoads(params Span<PutPointLoadRequest> pointLoads) =>
        this.pointLoads.AddRange(pointLoads);

    public IEnumerable<PutPointLoadRequest> PointLoadRequests() => this.pointLoads.AsReadOnly();

    private readonly List<PutMomentLoadRequest> momentLoads = [];

    public void AddMomentLoad(
        int id,
        int nodeId,
        int loadCaseId,
        double moment,
        Vector3 axisDirection
    ) =>
        this.AddMomentLoads(
            new PutMomentLoadRequest()
            {
                Id = id,
                NodeId = nodeId,
                LoadCaseId = loadCaseId,
                Torque = new(moment, this.UnitSettings.TorqueUnit),
                AxisDirection = axisDirection,
            }
        );

    public void AddMomentLoads(params Span<PutMomentLoadRequest> momentLoads) =>
        this.momentLoads.AddRange(momentLoads);

    public IEnumerable<PutMomentLoadRequest> MomentLoadRequests() => this.momentLoads.AsReadOnly();

    private readonly List<PutSectionProfileRequest> sectionProfiles = [];

    public void AddSectionProfile(
        int id,
        string name,
        double area,
        double strongAxisMomentOfInertia,
        double weakAxisMomentOfInertia,
        double polarMomentOfInertia,
        double strongAxisPlasticSectionModulus,
        double weakAxisPlasticSectionModulus,
        double strongAxisShearArea,
        double weakAxisShearArea
    ) =>
        this.AddSectionProfiles(
            new PutSectionProfileRequest()
            {
                Id = id,
                Name = name,
                Area = area,
                StrongAxisMomentOfInertia = strongAxisMomentOfInertia,
                WeakAxisMomentOfInertia = weakAxisMomentOfInertia,
                PolarMomentOfInertia = polarMomentOfInertia,
                StrongAxisPlasticSectionModulus = strongAxisPlasticSectionModulus,
                WeakAxisPlasticSectionModulus = weakAxisPlasticSectionModulus,
                StrongAxisShearArea = strongAxisShearArea,
                WeakAxisShearArea = weakAxisShearArea,
                LengthUnit = this.UnitSettings.LengthUnit,
            }
        );

    public void AddSectionProfiles(params Span<PutSectionProfileRequest> sectionProfiles) =>
        this.sectionProfiles.AddRange(sectionProfiles);

    public IEnumerable<PutSectionProfileRequest> SectionProfileRequests() =>
        this.sectionProfiles.AsReadOnly();

    private readonly List<SectionProfileFromLibraryContract> sectionProfilesFromLibrary = [];

    public void AddSectionProfileFromLibrary(int id, string name, StructuralCode library) =>
        this.AddSectionProfilesFromLibrary(
            new SectionProfileFromLibraryContract()
            {
                Id = id,
                Name = name,
                Library = library,
            }
        );

    public void AddSectionProfilesFromLibrary(
        params Span<SectionProfileFromLibraryContract> sectionProfilesFromLibrary
    ) => this.sectionProfilesFromLibrary.AddRange(sectionProfilesFromLibrary);

    public IEnumerable<SectionProfileFromLibraryContract> SectionProfilesFromLibraryRequests() =>
        this.sectionProfilesFromLibrary.AsReadOnly();
}
