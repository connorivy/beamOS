using BeamOs.ApiClient;
using BeamOs.Application.Common.Mappers;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

[Mapper]
public partial class ModelFixtureInDb
{
    [UseMapper]
    public ModelFixture ModelFixture { get; }

    [UseMapper]
    public UnitMapperWithOptionalUnits UnitMapperWithOptionalUnits { get; }
    public Dictionary<Guid, string> RuntimeIdToDbIdDict { get; } = [];

    public ModelFixtureInDb(ModelFixture modelFixture)
    {
        this.ModelFixture = modelFixture;
        this.UnitMapperWithOptionalUnits = modelFixture.UnitMapperWithOptionalUnits;
    }

    public async Task Create(ApiAlphaClient client)
    {
        // todo : update, don't just delete and re-create
        try
        {
            await client.CreateModelAsync(this.ToRequest(this));
        }
        catch (ApiException)
        {
            await client.DeleteModelAsync(this.ModelFixture.ModelId);
            await client.CreateModelAsync(this.ToRequest(this));
        }

        foreach (var nodeFixture in this.ModelFixture.NodeFixtures)
        {
            this.RuntimeIdToDbIdDict[nodeFixture.Id] = (
                await client.CreateNodeAsync(this.ToRequest(nodeFixture))
            ).Id;
        }

        foreach (var materialFixture in this.ModelFixture.MaterialFixtures)
        {
            this.RuntimeIdToDbIdDict[materialFixture.Id] = (
                await client.CreateMaterialAsync(this.ToRequest(materialFixture))
            ).Id;
        }

        foreach (var sectionProfileFixture in this.ModelFixture.SectionProfileFixtures)
        {
            this.RuntimeIdToDbIdDict[sectionProfileFixture.Id] = (
                await client.CreateSectionProfileAsync(this.ToRequest(sectionProfileFixture))
            ).Id;
        }

        foreach (var element1dFixture in this.ModelFixture.Element1dFixtures)
        {
            this.RuntimeIdToDbIdDict[element1dFixture.Id] = (
                await client.CreateElement1dAsync(this.ToRequest(element1dFixture))
            ).Id;
        }

        foreach (var pointLoadFixture in this.ModelFixture.PointLoadFixtures)
        {
            this.RuntimeIdToDbIdDict[pointLoadFixture.Id] = (
                await client.CreatePointLoadAsync(this.ToRequest(pointLoadFixture))
            ).Id;
        }

        foreach (var momentLoadFixture in this.ModelFixture.MomentLoadFixtures)
        {
            this.RuntimeIdToDbIdDict[momentLoadFixture.Id] = (
                await client.CreateMomentLoadAsync(this.ToRequest(momentLoadFixture))
            ).Id;
        }
    }

    protected string RuntimeIdToDbId(Guid id) => this.RuntimeIdToDbIdDict[id];

    protected string RuntimeModelIdToDbModelId(DummyModelId id) => this.ModelFixture.ModelId;
}
