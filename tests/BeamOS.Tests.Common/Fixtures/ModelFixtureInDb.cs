using BeamOs.Api.Common.Mappers;
using BeamOs.ApiClient;
using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOS.Tests.Common.Fixtures;
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
            await client.DeleteModelAsync(this.ModelFixture.Id.ToString());
            this.RuntimeIdToDbIdDict[this.ModelFixture.Id] = (
                await client.CreateModelAsync(this.ToRequest(this))
            ).Id;
        }

        foreach (var nodeFixture in this.ModelFixture.Nodes)
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

        foreach (var element1dFixture in this.ModelFixture.Element1dFixtures.Value)
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

    protected string RuntimeModelIdToDbId(GuidWrapperForModelId modelId) =>
        modelId.ModelId?.ToString() ?? this.ModelFixture.Id.ToString();

    public ModelResponse GetExpectedResponse()
    {
        var nodeResponses = this.ModelFixture.Nodes.Select(this.ToResponse).ToList();
        var element1dResponse = this.ModelFixture
            .Element1dFixtures
            .Value
            .Select(this.ToResponse)
            .ToList();
        var materialResponse = this.ModelFixture.MaterialFixtures.Select(this.ToResponse).ToList();
        var sectionProfileResponses = this.ModelFixture
            .SectionProfileFixtures
            .Select(this.ToResponse)
            .ToList();
        var pointLoadResponses = this.ModelFixture
            .PointLoadFixtures
            .Select(this.ToResponse)
            .ToList();
        var momentLoadResponses = this.ModelFixture
            .MomentLoadFixtures
            .Select(this.ToResponse)
            .ToList();

        return new ModelResponse(
            this.ModelFixture.Id.ToString(),
            this.ModelFixture.Name,
            this.ModelFixture.Description,
            new ModelSettingsResponse(this.ModelFixture.UnitSettings.ToResponse()),
            nodeResponses,
            element1dResponse,
            materialResponse,
            sectionProfileResponses,
            pointLoadResponses,
            momentLoadResponses
        );
    }
}
