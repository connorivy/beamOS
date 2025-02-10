using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.StructuralAnalysis.CsSdk;

public abstract class BeamOsModelBuilder
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract PhysicalModelSettings Settings { get; }

    /// <summary>
    /// You can go to this website to generate a random guid string
    /// https://www.uuidgenerator.net/guid
    /// </summary>
    public abstract string GuidString { get; }
    public Guid Id => Guid.Parse(this.GuidString);

    public IEnumerable<CreateNodeRequest> Nodes => this.NodeRequests();
    public abstract IEnumerable<CreateNodeRequest> NodeRequests();
    public IEnumerable<CreateMaterialRequest> Materials => this.MaterialRequests();
    public abstract IEnumerable<CreateMaterialRequest> MaterialRequests();
    public IEnumerable<CreateSectionProfileRequest> SectionProfiles =>
        this.SectionProfileRequests();
    public abstract IEnumerable<CreateSectionProfileRequest> SectionProfileRequests();
    public IEnumerable<CreateElement1dRequest> Element1ds => this.Element1dRequests();
    public abstract IEnumerable<CreateElement1dRequest> Element1dRequests();
    public IEnumerable<CreatePointLoadRequest> PointLoads => this.PointLoadRequests();
    public abstract IEnumerable<CreatePointLoadRequest> PointLoadRequests();
    public IEnumerable<CreateMomentLoadRequest> MomentLoads => this.MomentLoadRequests();

    public virtual IEnumerable<CreateMomentLoadRequest> MomentLoadRequests() => [];

    public Task Build() => this.Build(CreateDefaultApiClient());

    public async Task<bool> Build(
        IStructuralAnalysisApiClientV1 apiClient,
        bool createIfDoesntExist = false
    )
    {
        if (!Guid.TryParse(this.GuidString, out var modelId))
        {
            throw new Exception("Guid string is not formatted correctly");
        }

        try
        {
            await apiClient.CreateModelAsync(
                new()
                {
                    Name = this.Name,
                    Description = this.Description,
                    Settings = this.Settings,
                    Id = modelId
                }
            );
        }
        catch
        {
            if (createIfDoesntExist)
            {
                return false;
            }
            throw;
        }

        // todo : batching
        foreach (var el in this.NodeRequests())
        {
            (await apiClient.CreateNodeAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.PointLoadRequests())
        {
            (await apiClient.CreatePointLoadAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.MomentLoadRequests())
        {
            (await apiClient.CreateMomentLoadAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.MaterialRequests())
        {
            (await apiClient.CreateMaterialAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.SectionProfileRequests())
        {
            (await apiClient.CreateSectionProfileAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.Element1dRequests())
        {
            (await apiClient.CreateElement1dAsync(modelId, el)).ThrowIfError();
        }

        return true;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="structuralAnalysisApiClient"></param>
    /// <returns>a bool that is true if the model was created or false if it already existed</returns>
    public async Task<bool> CreateIfDoesntExist(
        IStructuralAnalysisApiClientV1 structuralAnalysisApiClient
    ) => await this.Build(structuralAnalysisApiClient, true);

    private static StructuralAnalysisApiClientV1 CreateDefaultApiClient()
    {
        HttpClient httpClient = new();
        return new StructuralAnalysisApiClientV1(httpClient);
    }
}

public sealed class BeamOsDynamicModelBuilder(
    string guidString,
    PhysicalModelSettings physicalModelSettings
) : BeamOsModelBuilder
{
    public override string Name => "Runtime Model";
    public override string Description => "A model that is created at runtime";
    public override PhysicalModelSettings Settings => physicalModelSettings;
    public override string GuidString => guidString;

    private List<CreateNodeRequest> nodes = new();

    public void AddNodes(params Span<CreateNodeRequest> nodes) => this.nodes.AddRange(nodes);

    public override IEnumerable<CreateNodeRequest> NodeRequests() => nodes.AsReadOnly();

    private List<CreateElement1dRequest> element1ds = new();

    public void AddElement1ds(params Span<CreateElement1dRequest> els) =>
        this.element1ds.AddRange(els);

    public override IEnumerable<CreateElement1dRequest> Element1dRequests() =>
        element1ds.AsReadOnly();

    private List<CreateMaterialRequest> materials = new();

    public void AddMaterials(params Span<CreateMaterialRequest> materials) =>
        this.materials.AddRange(materials);

    public override IEnumerable<CreateMaterialRequest> MaterialRequests() => materials.AsReadOnly();

    private List<CreatePointLoadRequest> pointLoads = new();

    public void AddPointLoads(params Span<CreatePointLoadRequest> pointLoads) =>
        this.pointLoads.AddRange(pointLoads);

    public override IEnumerable<CreatePointLoadRequest> PointLoadRequests() =>
        pointLoads.AsReadOnly();

    private List<CreateMomentLoadRequest> momentLoads = new();

    public void AddMomentLoads(params Span<CreateMomentLoadRequest> momentLoads) =>
        this.momentLoads.AddRange(momentLoads);

    public override IEnumerable<CreateMomentLoadRequest> MomentLoadRequests() =>
        momentLoads.AsReadOnly();

    private List<CreateSectionProfileRequest> sectionProfiles = new();

    public void AddSectionProfiles(params Span<CreateSectionProfileRequest> sectionProfiles) =>
        this.sectionProfiles.AddRange(sectionProfiles);

    public override IEnumerable<CreateSectionProfileRequest> SectionProfileRequests() =>
        sectionProfiles.AsReadOnly();
}
