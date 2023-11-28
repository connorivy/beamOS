using BeamOS.Common.Api.Interfaces;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;
using BeamOS.PhysicalModel.Contracts.Node;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalNodes.Mappers;

[Mapper]
public partial class RestraintResponseMapper : IMapper<RestraintResponse, RestraintCommand>
{
    public RestraintCommand Map(RestraintResponse from) => this.ToCommand(from);
    public partial RestraintCommand ToCommand(RestraintResponse response);
}
