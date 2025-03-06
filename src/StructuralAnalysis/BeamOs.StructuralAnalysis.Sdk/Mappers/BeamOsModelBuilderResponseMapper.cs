using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.Sdk;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.CsSdk.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public partial class BeamOsModelBuilderResponseMapper(Guid modelId)
{
    private Guid GetModelId() => modelId;

    [MapValue(nameof(ModelResponse.ResultSets), Use = nameof(EmptyResultSets))]
    public partial ModelResponse ToReponse(BeamOsModelBuilder builder);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial NodeResponse ToResponse(PutNodeRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial Element1dResponse ToResponse(PutElement1dRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial PointLoadResponse ToResponse(PutPointLoadRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial MomentLoadResponse ToResponse(PutMomentLoadRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial MaterialResponse ToResponse(PutMaterialRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial SectionProfileResponse ToResponse(PutSectionProfileRequest request);

    private List<ResultSetResponse> EmptyResultSets() => [];
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public static partial class BeamOsModelBuilderDtoMapper
{
    public static partial BeamOsModelBuilderDto ToDto(this BeamOsModelBuilder builder);
}
