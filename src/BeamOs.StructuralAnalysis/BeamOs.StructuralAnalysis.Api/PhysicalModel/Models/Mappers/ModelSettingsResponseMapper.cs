using BeamOs.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Models.Mappers;

[Mapper]
public partial class ModelSettingsResponseMapper : IMapper<ModelSettings, ModelSettingsResponse>
{
    public ModelSettingsResponse Map(ModelSettings from) => this.ToResponse(from);

    public partial ModelSettingsResponse ToResponse(ModelSettings model);
}
