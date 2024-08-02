using BeamOs.Application.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Application.PhysicalModel.Models.Interfaces;
using BeamOs.Contracts.PhysicalModel.Model;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Models.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public partial class IModelDataToResponseMapper : AbstractMapper<IModelData, ModelResponse>
{
    public override ModelResponse Map(IModelData source) => this.ToResponse(source);

    public partial ModelResponse ToResponse(IModelData source);
}
