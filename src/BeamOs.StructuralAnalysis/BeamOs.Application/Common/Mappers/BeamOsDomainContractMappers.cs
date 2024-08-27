using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;

namespace BeamOs.Application.Common.Mappers;

public static class BeamOsDomainContractMappers
{
    public static ModelId ToModelId(string modelId) => new(Guid.Parse(modelId));

    public static NodeId ToNodeId(string id) => new(Guid.Parse(id));

    public static Element1DId ToElement1DId(string id) => new(Guid.Parse(id));

    public static MaterialId ToMaterialId(string id) => new(Guid.Parse(id));

    public static SectionProfileId ToSectionProfileId(string id) => new(Guid.Parse(id));

    public static PointLoadId ToPointLoadId(string id) => new(Guid.Parse(id));

    public static MomentLoadId ToMomentLoadId(string id) => new(Guid.Parse(id));

    public static BeamOs.Contracts.PhysicalModel.Node.Point ToContract(
        BeamOs.Domain.Common.ValueObjects.Point source
    )
    {
        return new(
            UnitsNetMappers.MapToContract(source.XCoordinate),
            UnitsNetMappers.MapToContract(source.YCoordinate),
            UnitsNetMappers.MapToContract(source.ZCoordinate)
        );
    }

    //public static BeamOs.Domain.Common.ValueObjects.Point ToDomain(
    //    BeamOs.Domain.Common.ValueObjects.Point source
    //)
    //{
    //    return new(
    //        UnitsNetMappers.MapToContract(source.XCoordinate),
    //        UnitsNetMappers.MapToContract(source.YCoordinate),
    //        UnitsNetMappers.MapToContract(source.ZCoordinate)
    //    );
    //}
}
