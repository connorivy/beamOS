namespace BeamOs.ApiClient.Builders;

//[UseStaticMapper(typeof(UnitsNetMappers))]
public partial class CreateModelRequestBuilder
{
    public partial NodeResponse ToResponse(CreateNodeRequestBuilder fixture);

    public partial Element1DResponse ToResponse(CreateElement1dRequestBuilder fixture);

    public partial MaterialResponse ToResponse(CreateMaterialRequestBuilder fixture);

    public partial SectionProfileResponse ToResponse(CreateSectionProfileRequestBuilder fixture);

    public partial PointLoadResponse ToResponse(CreatePointLoadRequestBuilder fixture);

    //public partial MomentLoadResponse ToResponse(CreateMomentLoadRequestBuilder fixture);

    //public partial PointResponse ToResponse(BeamOs.Domain.Common.ValueObjects.Point source);

    //public partial RestraintResponse ToResponse(CreateRestraint source);

    //public partial ModelResponse ToResponse(CreateModelRequestBuilder fixture);
}
