using System.Text.Json;
using BeamOs.ApiClient;
using BeamOs.ApiClient.Builders;
using BeamOs.Application.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.Fixtures.Mappers.ToDomain;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
[UseStaticMapper(typeof(BeamOsIdMappers))]
public static partial class ModelResponseToDomainMapper
{
    public static partial Model ToDomain(ModelResponse fixture);

    public static partial ModelRequestBuilderDeserializationModel ToModelRequestBuilder(
        this ModelResponse fixture
    );

    public static void PopulateFromJson(this CreateModelRequestBuilder model, string fileLocation)
    {
        var data = File.ReadAllText(fileLocation);
        var options = new JsonSerializerOptions();
        options.IgnoreRequiredKeyword();
        var modelResponse = JsonSerializer.Deserialize<ModelResponse>(data);

        var deserializationModel = modelResponse.ToModelRequestBuilder();

        foreach (var el in deserializationModel.Nodes)
        {
            model.AddNode(el);
        }

        foreach (var el in deserializationModel.Element1ds)
        {
            model.AddElement1d(el);
        }

        foreach (var el in deserializationModel.Materials)
        {
            model.AddMaterial(el);
        }

        foreach (var el in deserializationModel.SectionProfiles)
        {
            model.AddSectionProfile(el);
        }

        foreach (var el in deserializationModel.PointLoads)
        {
            model.AddPointLoad(el);
        }
    }
}
