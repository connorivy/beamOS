using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.Models.Mappers;

[Mapper]
public partial class ModelSettingsResponseMapper : IMapper<ModelSettings, ModelSettingsResponse>
{
    public ModelSettingsResponse Map(ModelSettings from) => this.ToResponse(from);

    public partial ModelSettingsResponse ToResponse(ModelSettings model);
}
