namespace BeamOs.ApiClient.Builders;

public abstract partial class CreateModelRequestBuilder : IModelFixtureInDb, IModelFixture
{
    public abstract Guid ModelGuid { get; }
    public FixtureId Id => this.ModelGuid;
    public string Name { get; init; } = "Test Model";
    public string Description { get; init; } = "Created from CustomModelRequestBuilder";
    public abstract PhysicalModelSettings ModelSettings { get; }

    private List<CreateElement1dRequestBuilder> element1ds = [];
    public IEnumerable<CreateElement1dRequestBuilder> Element1ds => this.element1ds;
    private Dictionary<FixtureId, CreateNodeRequestBuilder> nodes = [];
    public IEnumerable<CreateNodeRequestBuilder> Nodes => this.nodes.Values;
    private List<CreatePointLoadRequestBuilder> pointLoads = [];
    public IEnumerable<CreatePointLoadRequestBuilder> PointLoads => this.pointLoads;
    private List<CreateMaterialRequestBuilder> materials = [];
    public IEnumerable<CreateMaterialRequestBuilder> Materials => this.materials;
    private List<CreateSectionProfileRequestBuilder> sectionProfiles = [];
    public IEnumerable<CreateSectionProfileRequestBuilder> SectionProfiles => this.sectionProfiles;

    private readonly Dictionary<FixtureId, string> runtimeIdToDbIdDict = [];
    public FixtureId DefaultMaterialId { get; set; }
    public FixtureId DefaultSectionProfileId { get; set; }

    public string RuntimeIdToDbId(FixtureId fixtureId) => this.runtimeIdToDbIdDict[fixtureId];

    public void AddNode(CreateNodeRequestBuilder node)
    {
        if (!this.nodes.ContainsKey(node.Id))
        {
            this.nodes.Add(node.Id, node with { ModelId = this.Id });
        }
    }

    public void AddElement1d(CreateElement1dRequestBuilder element1d)
    {
        this.element1ds.Add(element1d with { ModelId = this.Id });
    }

    public void AddPointLoad(CreatePointLoadRequestBuilder pointLoad)
    {
        this.pointLoads.Add(pointLoad with { ModelId = this.Id });
    }

    public void AddMaterial(CreateMaterialRequestBuilder material)
    {
        this.materials.Add(material with { ModelId = this.Id });
    }

    public void AddSectionProfile(CreateSectionProfileRequestBuilder sectionProfile)
    {
        this.sectionProfiles.Add(sectionProfile with { ModelId = this.Id });
    }

    public void AddElement(CreateModelEntityRequestBuilderBase builder)
    {
        switch (builder)
        {
            case CreateNodeRequestBuilder node:
                this.AddNode(node);
                break;
            case CreateElement1dRequestBuilder element1d:
                this.AddElement1d(element1d);
                break;
            case CreatePointLoadRequestBuilder pointLoad:
                this.AddPointLoad(pointLoad);
                break;
            case CreateMaterialRequestBuilder material:
                this.AddMaterial(material);
                break;
            case CreateSectionProfileRequestBuilder sectionProfile:
                this.AddSectionProfile(sectionProfile);
                break;
            default:
                throw new Exception("Unsupported entity type");
        }
    }

    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task Create(ApiAlphaClient client)
    {
        this.runtimeIdToDbIdDict[this.Id] = this.Id.Id;

        // todo : update, don't just delete and re-create
        try
        {
            this.runtimeIdToDbIdDict[this.Id] = (
                await client.CreateModelAsync(this.ToRequest(this))
            ).Id;
        }
        catch (ApiException)
        {
            await client.DeleteModelAsync(this.Id.ToString());
            this.runtimeIdToDbIdDict[this.Id] = (
                await client.CreateModelAsync(this.ToRequest(this))
            ).Id;
        }

        foreach (var nodeFixture in this.Nodes)
        {
            this.runtimeIdToDbIdDict[nodeFixture.Id] = (
                await client.CreateNodeAsync(this.ToRequest(nodeFixture))
            ).Id;
        }

        if (this.Materials.FirstOrDefault() is not CreateMaterialRequestBuilder defaultMaterial)
        {
            defaultMaterial = new();
            this.AddMaterial(defaultMaterial);
        }
        this.DefaultMaterialId = defaultMaterial.Id;

        foreach (var materialFixture in this.Materials)
        {
            this.runtimeIdToDbIdDict[materialFixture.Id] = (
                await client.CreateMaterialAsync(this.ToRequest(materialFixture))
            ).Id;
        }

        if (
            this.SectionProfiles.FirstOrDefault()
            is not CreateSectionProfileRequestBuilder defaultSectionProfile
        )
        {
            defaultSectionProfile = new();
            this.AddSectionProfile(defaultSectionProfile);
        }
        this.DefaultSectionProfileId = defaultSectionProfile.Id;

        foreach (var sectionProfileFixture in this.SectionProfiles)
        {
            this.runtimeIdToDbIdDict[sectionProfileFixture.Id] = (
                await client.CreateSectionProfileAsync(this.ToRequest(sectionProfileFixture))
            ).Id;
        }

        foreach (var element1dFixture in this.Element1ds)
        {
            if (element1dFixture.MaterialId == default)
            {
                element1dFixture.MaterialId = this.DefaultMaterialId;
            }

            if (element1dFixture.SectionProfileId == default)
            {
                element1dFixture.SectionProfileId = this.DefaultSectionProfileId;
            }

            this.runtimeIdToDbIdDict[element1dFixture.Id] = (
                await client.CreateElement1dAsync(this.ToRequest(element1dFixture))
            ).Id;
        }

        foreach (var pointLoadFixture in this.PointLoads)
        {
            this.runtimeIdToDbIdDict[pointLoadFixture.Id] = (
                await client.CreatePointLoadAsync(this.ToRequest(pointLoadFixture))
            ).Id;
        }

        //foreach (var momentLoadFixture in this.MomentLoads)
        //{
        //    this.RuntimeIdToDbIdDict[momentLoadFixture.Id] = (
        //        await client.CreateMomentLoadAsync(this.ToRequest(momentLoadFixture))
        //    ).Id;
        //}
    }
}
