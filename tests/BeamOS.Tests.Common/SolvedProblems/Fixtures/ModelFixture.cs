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
using BeamOS.Tests.Common.Interfaces;
using BeamOS.WebApp.EditorApi;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

[Mapper]
public abstract partial class ModelFixture : FixtureBase, ITestFixtureDisplayable
{
    protected ModelFixture()
    {
        this.UnitMapperWithOptionalUnits = new(this.UnitSettings);
        this.StrongModelId = new(this.Id);
    }

    public Dictionary<Guid, string> FixtureGuidToIdDict { get; } = [];
    public abstract override Guid Id { get; }
    public override GuidWrapperForModelId ModelId => new(this.Id);
    public abstract UnitSettings UnitSettings { get; protected set; }
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

    public ModelId StrongModelId { get; }

    public partial PointLoad ToDomainObjectWithLocalIds(PointLoadFixture fixture);

    public partial MomentLoad ToDomainObjectWithLocalIds(MomentLoadFixture fixture);

    public static Vector<double> ToVector(Vector3D vector3D) => vector3D.ToVector();

    public partial Node ToDomainObjectWithLocalIds(NodeFixture fixture);

    public partial Material ToDomainObjectWithLocalIds(MaterialFixture fixture);

    public partial SectionProfile ToDomainObjectWithLocalIds(SectionProfileFixture fixture);

    [UseMapper]
    public UnitMapperWithOptionalUnits UnitMapperWithOptionalUnits { get; protected set; }

    public ModelResponse GetExpectedResponse(UnitSettings? unitSettingsOverride = null)
    {
        // todo : this is not thread safe and is just generally not good.
        // we probably need to bit the bullet and write mappers that accept units
        UnitMapperWithOptionalUnits? existingUnitMapper = null;
        if (unitSettingsOverride is not null)
        {
            existingUnitMapper = this.UnitMapperWithOptionalUnits.ShallowCopy();
            this.UnitMapperWithOptionalUnits = new(unitSettingsOverride);
        }
        try
        {
            var nodeResponses = this.NodeFixtures.Select(this.ToResponse).ToList();
            var element1dResponse = this.Element1dFixtures.Select(this.ToResponse).ToList();
            var materialResponse = this.MaterialFixtures.Select(this.ToResponse).ToList();
            var sectionProfileResponses = this.SectionProfileFixtures
                .Select(this.ToResponse)
                .ToList();
            var pointLoadResponses = this.PointLoadFixtures.Select(this.ToResponse).ToList();
            var momentLoadResponses = this.MomentLoadFixtures.Select(this.ToResponse).ToList();

            return new ModelResponse(
                this.Id.ToString(),
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
        finally
        {
            if (existingUnitMapper is not null)
            {
                this.UnitMapperWithOptionalUnits = existingUnitMapper;
            }
        }
    }

    public async Task Display(IEditorApiAlpha editorApiAlpha)
    {
        await editorApiAlpha.CreateModelAsync(this.GetExpectedResponse(UnitSettings.SI));
    }
}

public interface IHasExpectedNodeResults
{
    public NodeResultFixture[] ExpectedNodeResults { get; }
    public NodeResultResponse ToResponse(NodeResultFixture source);
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
