using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Common.Mappers.UnitValueDtoMappers;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.CsSdk.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
internal partial class BeamOsModelBuilderResponseMapper(Guid modelId)
{
    private Guid GetModelId() => modelId;

    [MapValue(nameof(ModelResponse.ResultSets), Use = nameof(EmptyResultSets))]
    public partial ModelResponse ToReponse(BeamOsStaticModelBase builder);

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

    private static List<ResultSetResponse> EmptyResultSets() => [];
}

// [Mapper]
// [UseStaticMapper(typeof(UnitsNetMappers))]
// internal static partial class BeamOsModelBuilderDtoMapper
// {
//     public static partial BeamOsModelBuilderDto ToDto(this BeamOsStaticModelBase builder);
// }
