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
    public ModelFixture2 ModelFixture { get; }

    [UseMapper]
    public UnitMapperWithOptionalUnits UnitMapperWithOptionalUnits { get; }
    public Dictionary<Guid, string> RuntimeIdToDbIdDict { get; } = [];

    public ModelFixtureInDb(ModelFixture2 modelFixture)
    {
        this.ModelFixture = modelFixture;
        this.UnitMapperWithOptionalUnits = new(modelFixture.Settings.UnitSettings);
    }

    public async Task Create(ApiAlphaClient client)
    {
        // todo : update, don't just delete and re-create
        try
        {
            this.RuntimeIdToDbIdDict[this.ModelFixture.Id] = (
                await client.CreateModelAsync(this.ToRequest(this))
            ).Id;
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

    protected string RuntimeIdToDbId(Guid id) => this.RuntimeIdToDbIdDict[id];

    protected string RuntimeModelIdToDbId(GuidWrapperForModelId modelId) =>
        modelId.ModelId?.ToString() ?? this.ModelFixture.Id.ToString();

    public ModelResponse GetExpectedResponse()
    {
        var nodeResponses = this.ModelFixture.Nodes.Select(this.ToResponse).ToList();
        var element1dResponse = this.ModelFixture.Element1ds.Select(this.ToResponse).ToList();
        var materialResponse = this.ModelFixture.Materials.Select(this.ToResponse).ToList();
        var sectionProfileResponses = this.ModelFixture
            .SectionProfiles
            .Select(this.ToResponse)
            .ToList();
        var pointLoadResponses = this.ModelFixture.PointLoads.Select(this.ToResponse).ToList();
        var momentLoadResponses = this.ModelFixture.MomentLoads.Select(this.ToResponse).ToList();

        return new ModelResponse(
            this.ModelFixture.Id.ToString(),
            this.ModelFixture.Name,
            this.ModelFixture.Description,
            new ModelSettingsResponse(this.ModelFixture.Settings.UnitSettings.ToResponse()),
            nodeResponses,
            element1dResponse,
            materialResponse,
            sectionProfileResponses,
            pointLoadResponses,
            momentLoadResponses
        );
    }
}
