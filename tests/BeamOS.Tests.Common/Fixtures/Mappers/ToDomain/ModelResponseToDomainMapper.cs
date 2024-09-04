using System.Text.Json;
using BeamOs.ApiClient;
using BeamOs.ApiClient.Builders;
using BeamOs.Application.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.Fixtures.Mappers.ToDomain;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
//[UseStaticMapper(typeof(BeamOsIdMappers))]
public static partial class ModelResponseToDomainMapper
{
    public static partial CreateNodeRequestBuilder ToRequestBuilder(this NodeResponse fixture);

    public static partial CreateElement1dRequestBuilder ToRequestBuilder(
        this Element1DResponse fixture
    );

    public static partial CreateMaterialRequestBuilder ToRequestBuilder(
        this MaterialResponse fixture
    );

    public static partial CreateSectionProfileRequestBuilder ToRequestBuilder(
        this SectionProfileResponse fixture
    );

    public static partial CreatePointLoadRequestBuilder ToRequestBuilder(
        this PointLoadResponse fixture
    );

    public static void PopulateFromJson(this CreateModelRequestBuilder model, string fileLocation)
    {
        var data = File.ReadAllText(fileLocation);
        var options = new JsonSerializerOptions();
        options.IgnoreRequiredKeyword();
        var modelResponse = JsonSerializer.Deserialize<ModelResponse>(data);

        model.InitializeFromModelResponse(modelResponse);
    }

    public static void InitializeFromModelResponse(
        this CreateModelRequestBuilder builder,
        ModelResponse modelResponse
    )
    {
        foreach (var el in modelResponse.Nodes ?? [])
        {
            builder.AddNode(el.ToRequestBuilder());
        }

        foreach (var el in modelResponse.Element1ds ?? [])
        {
            builder.AddElement1d(el.ToRequestBuilder());
        }

        foreach (var el in modelResponse.Materials ?? [])
        {
            builder.AddMaterial(el.ToRequestBuilder());
        }

        foreach (var el in modelResponse.SectionProfiles ?? [])
        {
            builder.AddSectionProfile(el.ToRequestBuilder());
        }

        foreach (var el in modelResponse.PointLoads ?? [])
        {
            builder.AddPointLoad(el.ToRequestBuilder());
        }
    }
}
