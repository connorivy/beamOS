using Riok.Mapperly.Abstractions;

namespace BeamOs.ApiClient.Builders;

[Mapper]
public partial class CreateModelRequestBuilder
{
    //[MapperIgnoreTarget(nameof(CreateModelRequest.Id))]
    //[MapProperty(nameof(CreateModelRequestBuilder.Settings), nameof(CreateModelRequest.Settings))]
    public partial CreateModelRequest ToRequest(CreateModelRequestBuilder fixture);

    public partial CreateElement1dRequest ToRequest(CreateElement1dRequestBuilder fixture);

    public partial CreatePointLoadRequest ToRequest(CreatePointLoadRequestBuilder fixture);

    //public partial CreateMomentLoadRequest ToRequest(MomentLoadFixture2 fixture);

    public partial CreateMaterialRequest ToRequest(CreateMaterialRequestBuilder fixture);

    public partial CreateSectionProfileRequest ToRequest(
        CreateSectionProfileRequestBuilder fixture
    );

    public partial CreateNodeRequest ToRequest(CreateNodeRequestBuilder fixture);
}
