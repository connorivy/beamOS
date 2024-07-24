using BeamOs.ApiClient;
using BeamOs.ApiClient.Builders;

namespace BeamOs.ApiClient.Builders;

public partial class CreateModelRequestBuilder
{
    public FixtureId Id { get; } = Guid.NewGuid().ToString();
    public string Name { get; init; }
    public string Description { get; init; }
    public required PhysicalModelSettingsDto Settings { get; init; }
    public List<CreateElement1dRequestBuilder> Element1ds { get; } = [];
    public List<CreateNodeRequestBuilder> Nodes { get; } = [];
    public List<CreatePointLoadRequestBuilder> PointLoads { get; } = [];
    public List<CreateMaterialRequestBuilder> Materials { get; } = [];
    public List<CreateSectionProfileRequestBuilder> SectionProfiles { get; } = [];

    private readonly Dictionary<FixtureId, string> runtimeIdToDbIdDict = [];
    public FixtureId DefaultMaterialId { get; set; }
    public FixtureId DefaultSectionProfileId { get; set; }

    private string GetDbId(FixtureId fixtureId) => this.runtimeIdToDbIdDict[fixtureId];

    public void AddNode(CreateNodeRequestBuilder node)
    {
        this.Nodes.Add(node with { ModelId = this.Id });
    }

    public void AddElement1d(CreateElement1dRequestBuilder element1d)
    {
        this.Element1ds.Add(
            element1d with
            {
                ModelId = this.Id,
                MaterialId = element1d.MaterialId ?? this.DefaultMaterialId,
                SectionProfileId = element1d.SectionProfileId ?? this.DefaultSectionProfileId,
            }
        );
    }

    public void AddPointLoad(CreatePointLoadRequestBuilder pointLoad)
    {
        this.PointLoads.Add(pointLoad with { ModelId = this.Id });
    }

    public void AddMaterial(CreateMaterialRequestBuilder material)
    {
        this.Materials.Add(material with { ModelId = this.Id });
    }

    public void AddSectionProfile(CreateSectionProfileRequestBuilder sectionProfile)
    {
        this.SectionProfiles.Add(sectionProfile with { ModelId = this.Id });
    }

    public async Task AddToDatabase(ApiAlphaClient client)
    {
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

        var defaultMaterial = this.Materials.FirstOrDefault() ?? new();
        this.DefaultMaterialId = defaultMaterial.Id;
        this.runtimeIdToDbIdDict[defaultMaterial.Id] = (
            await client.CreateMaterialAsync(this.ToRequest(defaultMaterial))
        ).Id;

        foreach (var materialFixture in this.Materials.Skip(1))
        {
            this.runtimeIdToDbIdDict[materialFixture.Id] = (
                await client.CreateMaterialAsync(this.ToRequest(materialFixture))
            ).Id;
        }

        var defaultSectionProfile = this.SectionProfiles.FirstOrDefault() ?? new();
        this.DefaultSectionProfileId = defaultSectionProfile.Id;
        this.runtimeIdToDbIdDict[defaultSectionProfile.Id] = (
            await client.CreateSectionProfileAsync(this.ToRequest(defaultSectionProfile))
        ).Id;
        foreach (var sectionProfileFixture in this.SectionProfiles.Skip(1))
        {
            this.runtimeIdToDbIdDict[sectionProfileFixture.Id] = (
                await client.CreateSectionProfileAsync(this.ToRequest(sectionProfileFixture))
            ).Id;
        }

        foreach (var element1dFixture in this.Element1ds)
        {
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
