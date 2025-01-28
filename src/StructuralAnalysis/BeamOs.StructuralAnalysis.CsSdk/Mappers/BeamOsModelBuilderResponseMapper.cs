using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.CsSdk.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public partial class BeamOsModelBuilderResponseMapper(Guid modelId)
{
    private Guid GetModelId() => modelId;

    public partial ModelResponse ToReponse(BeamOsModelBuilder builder);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial NodeResponse ToResponse(CreateNodeRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial Element1dResponse ToResponse(CreateElement1dRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial PointLoadResponse ToResponse(CreatePointLoadRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial MomentLoadResponse ToResponse(CreateMomentLoadRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial MaterialResponse ToResponse(CreateMaterialRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial SectionProfileResponse ToResponse(CreateSectionProfileRequest request);
}
