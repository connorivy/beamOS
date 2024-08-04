using BeamOs.ApiClient;
using BeamOs.ApiClient.Builders;
using BeamOs.Application.Common.Mappers;
using BeamOS.Tests.Common.Fixtures;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

[Mapper]
public partial class ModelFixtureInDb(ModelFixture2 modelFixture) : IModelFixtureInDb
{
    private ModelFixture2 ModelFixture { get; } = modelFixture;

    [UseMapper]
    private UnitMapperWithOptionalUnits UnitMapperWithOptionalUnits { get; } =
        new(modelFixture.Settings.UnitSettings.ToDomain());

    private Dictionary<FixtureId, string> RuntimeIdToDbIdDict { get; } = [];

    public async Task Create(ApiAlphaClient client)
    {
        this.RuntimeIdToDbIdDict[this.ModelFixture.Id] = this.ModelFixture.Id.ToString();

        // todo : update, don't just delete and re-create
        try
        {
            this.RuntimeIdToDbIdDict[this.ModelFixture.Id] = (
                await client.CreateModelAsync(this.ToRequest())
            ).Id;
        }
        catch (ApiException)
        {
            await client.DeleteModelAsync(this.ModelFixture.Id.ToString());
            this.RuntimeIdToDbIdDict[this.ModelFixture.Id] = (
                await client.CreateModelAsync(this.ToRequest())
            ).Id;
        }

        foreach (var nodeFixture in this.ModelFixture.Nodes)
        {
            this.RuntimeIdToDbIdDict[nodeFixture.Id] = (
                await client.CreateNodeAsync(this.ToRequest(nodeFixture))
            ).Id;
        }

        foreach (var materialFixture in this.ModelFixture.Materials)
        {
            this.RuntimeIdToDbIdDict[materialFixture.Id] = (
                await client.CreateMaterialAsync(this.ToRequest(materialFixture))
            ).Id;
        }

        foreach (var sectionProfileFixture in this.ModelFixture.SectionProfiles)
        {
            this.RuntimeIdToDbIdDict[sectionProfileFixture.Id] = (
                await client.CreateSectionProfileAsync(this.ToRequest(sectionProfileFixture))
            ).Id;
        }

        foreach (var element1dFixture in this.ModelFixture.Element1ds)
        {
            this.RuntimeIdToDbIdDict[element1dFixture.Id] = (
                await client.CreateElement1dAsync(this.ToRequest(element1dFixture))
            ).Id;
        }

        foreach (var pointLoadFixture in this.ModelFixture.PointLoads)
        {
            this.RuntimeIdToDbIdDict[pointLoadFixture.Id] = (
                await client.CreatePointLoadAsync(this.ToRequest(pointLoadFixture))
            ).Id;
        }

        foreach (var momentLoadFixture in this.ModelFixture.MomentLoads)
        {
            this.RuntimeIdToDbIdDict[momentLoadFixture.Id] = (
                await client.CreateMomentLoadAsync(this.ToRequest(momentLoadFixture))
            ).Id;
        }
    }

    public string RuntimeIdToDbId(FixtureId id) => this.RuntimeIdToDbIdDict[id];
}
