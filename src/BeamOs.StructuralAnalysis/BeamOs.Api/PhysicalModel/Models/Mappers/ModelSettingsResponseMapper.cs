using BeamOs.Application.Common.Interfaces;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Models.Mappers;

[Mapper]
public partial class ModelSettingsResponseMapper : IMapper<ModelSettings, ModelSettingsResponse>
{
    public ModelSettingsResponse Map(ModelSettings from) => this.ToResponse(from);

    public partial ModelSettingsResponse ToResponse(ModelSettings model);
}
