using BeamOs.Api.Common.Mappers;
using BeamOs.ApiClient;
using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

[Mapper]
public abstract partial class ModelFixture : FixtureBase
{
    protected ModelFixture()
    {
        this.UnitMapperWithOptionalUnits = new(this.UnitSettings);
        this.ModelId = this.Id.ToString();
        this.StrongModelId = new(Guid.Parse(this.ModelId));
    }

    public Dictionary<Guid, string> FixtureGuidToIdDict { get; } = [];
    public abstract override Guid Id { get; }
    public abstract UnitSettings UnitSettings { get; }
    public virtual string Name { get; } = "Test Model";
    public virtual string Description { get; } = "Test Model Description";
    public virtual NodeFixture[] NodeFixtures { get; } = [];
    public virtual MaterialFixture[] MaterialFixtures { get; } = [];
    public virtual SectionProfileFixture[] SectionProfileFixtures { get; } = [];
    public virtual Element1dFixture[] Element1dFixtures { get; } = [];
    public virtual PointLoadFixture[] PointLoadFixtures { get; } = [];
    public virtual MomentLoadFixture[] MomentLoadFixtures { get; } = [];

    private async Task PopulateFixtureGuidToIdDictFromExistingModel(ApiAlphaClient client)
    {
        ModelResponse response = await client.GetModelAsync(this.Id.ToString(), null);
    }

    public string ModelId { get; private set; }
    public ModelId StrongModelId { get; }

    //public partial NodeResponse ToResponse(NodeFixture fixture);

    protected string GetModelId(DummyModelId modelId)
    {
        return this.Id.ToString();
    }

    //protected string GuidToString(Guid id) => this.FixtureGuidToIdDict[id];

    public ModelId ToModelId(DummyModelId id) => this.StrongModelId;

    public partial PointLoad ToDomainObjectWithLocalIds(PointLoadFixture fixture);

    public partial MomentLoad ToDomainObjectWithLocalIds(MomentLoadFixture fixture);

    public static Vector<double> ToVector(Vector3D vector3D) => vector3D.ToVector();

    public partial Node ToDomainObjectWithLocalIds(NodeFixture fixture);

    public partial Material ToDomainObjectWithLocalIds(MaterialFixture fixture);

    public partial SectionProfile ToDomainObjectWithLocalIds(SectionProfileFixture fixture);

    [UseMapper]
    public UnitMapperWithOptionalUnits UnitMapperWithOptionalUnits { get; }

    public ModelResponse GetExpectedResponse()
    {
        var nodeResponses = this.NodeFixtures.Select(this.ToResponse).ToList();
        var element1dResponse = this.Element1dFixtures.Select(this.ToResponse).ToList();
        var materialResponse = this.MaterialFixtures.Select(this.ToResponse).ToList();
        var sectionProfileResponses = this.SectionProfileFixtures.Select(this.ToResponse).ToList();
        var pointLoadResponses = this.PointLoadFixtures.Select(this.ToResponse).ToList();
        var momentLoadResponses = this.MomentLoadFixtures.Select(this.ToResponse).ToList();

        return new ModelResponse(
            this.ModelId,
            this.Name,
            this.Description,
            new ModelSettingsResponse(this.UnitSettings.ToResponse()),
            nodeResponses,
            element1dResponse,
            materialResponse,
            sectionProfileResponses,
            pointLoadResponses,
            momentLoadResponses
        );
    }
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
        where T : ModelFixtureInDb
    {
        var nodeResponses = modelFixture
            .ModelFixture
            .NodeFixtures
            .Select(modelFixture.ToResponse)
            .ToList();
        var element1dResponse = modelFixture
            .ModelFixture
            .Element1dFixtures
            .Select(modelFixture.ToResponse)
            .ToList();
        var materialResponse = modelFixture
            .ModelFixture
            .MaterialFixtures
            .Select(modelFixture.ToResponse)
            .ToList();
        var sectionProfileResponses = modelFixture
            .ModelFixture
            .SectionProfileFixtures
            .Select(modelFixture.ToResponse)
            .ToList();
        var pointLoadResponses = modelFixture
            .ModelFixture
            .PointLoadFixtures
            .Select(modelFixture.ToResponse)
            .ToList();
        var momentLoadResponses = modelFixture
            .ModelFixture
            .MomentLoadFixtures
            .Select(modelFixture.ToResponse)
            .ToList();

        return new ModelResponse(
            modelFixture.ModelFixture.ModelId,
            modelFixture.ModelFixture.Name,
            modelFixture.ModelFixture.Description,
            new ModelSettingsResponse(modelFixture.ModelFixture.UnitSettings.ToResponse()),
            nodeResponses,
            element1dResponse,
            materialResponse,
            sectionProfileResponses,
            pointLoadResponses,
            momentLoadResponses
        );
    }
}

public struct DummyModelId
{
    public Guid ToGuid() => Guid.NewGuid();
}

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
