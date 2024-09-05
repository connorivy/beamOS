namespace BeamOs.ApiClient.Builders;

public abstract partial class CreateModelRequestBuilder
    : IModelFixtureInDb,
        IModelFixture,
        IHasFixtureId
{
    public abstract Guid ModelGuid { get; }
    public FixtureId Id => this.ModelGuid;
    public virtual string Name { get; init; } = "Test Model";
    public virtual string Description { get; init; } = "Created from CustomModelRequestBuilder";
    public abstract PhysicalModelSettings Settings { get; }

    private Dictionary<FixtureId, CreateElement1dRequestBuilder> element1ds = [];
    public IEnumerable<CreateElement1dRequestBuilder> Element1ds
    {
        get => this.element1ds.Values;
        init => this.element1ds = value.ToDictionary(el => el.Id, el => el);
    }
    private Dictionary<FixtureId, CreateNodeRequestBuilder> nodes = [];
    public IEnumerable<CreateNodeRequestBuilder> Nodes
    {
        get => this.nodes.Values;
        init => this.nodes = value.ToDictionary(n => n.Id, n => n);
    }
    private List<CreatePointLoadRequestBuilder> pointLoads = [];
    public IEnumerable<CreatePointLoadRequestBuilder> PointLoads
    {
        get => this.pointLoads;
        init => this.pointLoads = value.ToList();
    }
    private List<CreateMaterialRequestBuilder> materials = [];
    public IEnumerable<CreateMaterialRequestBuilder> Materials
    {
        get => this.materials;
        init => this.materials = value.ToList();
    }
    private List<CreateSectionProfileRequestBuilder> sectionProfiles = [];
    public IEnumerable<CreateSectionProfileRequestBuilder> SectionProfiles
    {
        get => this.sectionProfiles;
        init => this.sectionProfiles = value.ToList();
    }

    private readonly Dictionary<FixtureId, string> runtimeIdToDbIdDict = [];
    private FixtureId DefaultMaterialId { get; set; }
    private FixtureId DefaultSectionProfileId { get; set; }

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
        if (!this.element1ds.ContainsKey(element1d.Id))
        {
            this.element1ds.Add(element1d.Id, element1d with { ModelId = this.Id });
        }
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

    public async Task Create(IApiAlphaClient client)
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

//public class ModelRequestBuilderDeserializationModel
//{
//    public IEnumerable<CreateElement1dRequestBuilder> Element1ds { get; init; }
//    public IEnumerable<CreateNodeRequestBuilder> Nodes { get; init; }
//    public IEnumerable<CreatePointLoadRequestBuilder> PointLoads { get; init; }
//    public IEnumerable<CreateMaterialRequestBuilder> Materials { get; init; }
//    public IEnumerable<CreateSectionProfileRequestBuilder> SectionProfiles { get; init; }
//}
