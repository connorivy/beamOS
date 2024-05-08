using BeamOs.Api.Common.Mappers;
using BeamOs.ApiClient;
using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

[Mapper(PreferParameterlessConstructors = false)]
public abstract partial class ModelFixture
{
    protected ModelFixture()
    {
        this.UnitMapperWithOptionalUnits = new(this.UnitSettings);
        this.ModelId = this.Id.ToString();
    }

    public abstract Guid Id { get; }
    public virtual string Name { get; } = "Test Model";
    public virtual string Description { get; } = "Test Model Description";
    public abstract UnitSettings UnitSettings { get; }
    public virtual NodeFixture[] NodeFixtures { get; } = [];
    public virtual MaterialFixture[] MaterialFixtures { get; } = [];
    public virtual SectionProfileFixture[] SectionProfileFixtures { get; } = [];
    public virtual Element1dFixture[] Element1dFixtures { get; } = [];
    public virtual PointLoadFixture[] PointLoadFixtures { get; } = [];
    public virtual MomentLoadFixture[] MomentLoadFixtures { get; } = [];

    public async Task Create(ApiAlphaClient client)
    {
        // todo : update, don't just delete and re-create
        try
        {
            await client.CreateModelAsync(this.ToRequest());
        }
        catch (ApiException)
        {
            await client.DeleteModelAsync(this.ModelId);
            await client.CreateModelAsync(this.ToRequest());
        }

        foreach (var nodeFixture in this.NodeFixtures)
        {
            this.FixtureGuidToIdDict[nodeFixture.Id] = (
                await client.CreateNodeAsync(nodeFixture.ToRequest(this.ModelId))
            ).Id;
        }

        foreach (var materialFixture in this.MaterialFixtures)
        {
            this.FixtureGuidToIdDict[materialFixture.Id] = (
                await client.CreateMaterialAsync(materialFixture.ToRequest(this.ModelId))
            ).Id;
        }

        foreach (var sectionProfileFixture in this.SectionProfileFixtures)
        {
            this.FixtureGuidToIdDict[sectionProfileFixture.Id] = (
                await client.CreateSectionProfileAsync(
                    sectionProfileFixture.ToRequest(this.ModelId)
                )
            ).Id;
        }

        foreach (var element1dFixture in this.Element1dFixtures)
        {
            this.FixtureGuidToIdDict[element1dFixture.Id] = (
                await client.CreateElement1dAsync(this.ToRequest(element1dFixture))
            ).Id;
        }

        foreach (var pointLoadFixture in this.PointLoadFixtures)
        {
            this.FixtureGuidToIdDict[pointLoadFixture.Id] = (
                await client.CreatePointLoadAsync(this.ToRequest(pointLoadFixture))
            ).Id;
        }

        foreach (var momentLoadFixture in this.MomentLoadFixtures)
        {
            this.FixtureGuidToIdDict[momentLoadFixture.Id] = (
                await client.CreateMomentLoadAsync(this.ToRequest(momentLoadFixture))
            ).Id;
        }
    }

    private async Task PopulateFixtureGuidToIdDictFromExistingModel(ApiAlphaClient client)
    {
        ModelResponse response = await client.GetModelAsync(this.Id.ToString(), null);
    }

    public string ModelId { get; private set; }

    public Dictionary<Guid, string> FixtureGuidToIdDict { get; } = [];

    public partial NodeResponse ToResponse(NodeFixture fixture);

    protected string GetModelId(DummyModelId modelId)
    {
        return this.Id.ToString();
    }

    protected string GuidToString(Guid id) => this.FixtureGuidToIdDict[id];

    public partial Element1DResponse ToResponse(Element1dFixture fixture);

    public partial MaterialResponse ToResponse(MaterialFixture fixture);

    public partial SectionProfileResponse ToResponse(SectionProfileFixture fixture);

    public partial PointLoadResponse ToResponse(PointLoadFixture fixture);

    public partial MomentLoadResponse ToResponse(MomentLoadFixture fixture);

    public partial PointResponse ToResponse(Point source);

    public partial RestraintResponse ToResponse(Restraint source);

    public partial CreateElement1dRequest ToRequest(Element1dFixture fixture);

    [MapProperty(
        nameof(PointLoadFixture.Direction),
        nameof(CreatePointLoadRequest.Direction)
    )]
    public partial CreatePointLoadRequest ToRequest(PointLoadFixture fixture);

    [MapProperty(
        nameof(MomentLoadFixture.AxisDirection),
        nameof(CreateMomentLoadRequest.AxisDirection)
    )]
    public partial CreateMomentLoadRequest ToRequest(MomentLoadFixture fixture);

    //[MapDerivedType<Guid, PointLoadId>]
    //[MapDerivedType<Guid, NodeId>]
    //public partial GuidBasedId ToId(Guid id);

    public TId ToId<TId>(Guid id)
        where TId : IConstructable<TId, Guid>
    {
        return TId.Construct(Guid.Parse(this.FixtureGuidToIdDict[id]));
    }

    public NodeId ToNodeId(Guid id) => this.ToId<NodeId>(id);

    public PointLoadId ToPointLoadId(Guid id) => this.ToId<PointLoadId>(id);

    //[ObjectFactory]
    //public TId ToId<TId>(Guid id)
    //    where TId : NodeBaseId, IConstructable<TId, Guid>
    //{
    //    return TId.Construct(id);
    //}

    //public partial NodeId ToId(Guid id);

    public partial PointLoad ToDomainObject2(PointLoadFixture fixture);

    // public partial PointLoadFixture ToDomainObject2(PointLoad fixture);

    [UseMapper]
    protected UnitMapperWithOptionalUnits UnitMapperWithOptionalUnits { get; }
}

public interface IHasExpectedModelResponse { }

public interface IHasExpectedNodeResults
{
    public NodeResultFixture[] ExpectedNodeResults { get; }
    public NodeResultResponse ToResponse(NodeResultFixture source);
}

public static class IHasExpectedModelResponseExtensions
{
    public static ModelResponse GetExpectedResponse<T>(this T modelFixture)
        where T : ModelFixture, IHasExpectedModelResponse
    {
        var nodeResponses = modelFixture.NodeFixtures.Select(modelFixture.ToResponse).ToList();
        var element1dResponse = modelFixture
            .Element1dFixtures
            .Select(modelFixture.ToResponse)
            .ToList();
        var materialResponse = modelFixture
            .MaterialFixtures
            .Select(modelFixture.ToResponse)
            .ToList();
        var sectionProfileResponses = modelFixture
            .SectionProfileFixtures
            .Select(modelFixture.ToResponse)
            .ToList();
        var pointLoadResponses = modelFixture
            .PointLoadFixtures
            .Select(modelFixture.ToResponse)
            .ToList();
        var momentLoadResponses = modelFixture
            .MomentLoadFixtures
            .Select(modelFixture.ToResponse)
            .ToList();

        return new ModelResponse(
            modelFixture.ModelId,
            modelFixture.Name,
            modelFixture.Description,
            new ModelSettingsResponse(modelFixture.UnitSettings.ToContract()),
            nodeResponses,
            element1dResponse,
            materialResponse,
            sectionProfileResponses,
            pointLoadResponses,
            momentLoadResponses
        );
    }
}

public struct DummyModelId { }

[Mapper]
public static partial class IHasExpectedNodeResultsExtensions
{
    public static NodeResultResponse[] GetExpectedNodeResultFixtures(
        this IHasExpectedNodeResults modelFixture
    )
    {
        return modelFixture.ExpectedNodeResults.Select(modelFixture.ToResponse).ToArray();
    }
}
