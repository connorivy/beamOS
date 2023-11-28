using BeamOS.Common.Api.Interfaces;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;
using BeamOS.PhysicalModel.Contracts.Model;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalModels.Mappers;

[Mapper]
public partial class ModelSettingsResponseMapper : IMapper<ModelSettingsResponse, ModelSettingsCommand>
{
    public ModelSettingsCommand Map(ModelSettingsResponse source) => this.ToCommand(source);
    public partial ModelSettingsCommand ToCommand(ModelSettingsResponse source);
}
