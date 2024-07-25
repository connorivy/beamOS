using Riok.Mapperly.Abstractions;

namespace BeamOs.ApiClient.Builders;

[Mapper]
//[UseStaticMapper(typeof(FixtureIdToStringMapper))]
public static partial class CreateModelRequestBuilderMapper
{
    [MapProperty(
        nameof(CreateModelRequestBuilder.ModelSettings),
        nameof(CreateModelRequest.Settings)
    )]
    public static partial ModelResponse ToResponseWithLocalIds(
        this CreateModelRequestBuilder fixture
    );

    public static string MapToId(FixtureId fixtureId) => fixtureId.Id;
}
