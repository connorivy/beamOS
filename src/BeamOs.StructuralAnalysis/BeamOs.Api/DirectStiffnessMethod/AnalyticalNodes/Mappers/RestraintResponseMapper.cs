using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalNodes.Commands;
using BeamOs.Contracts.PhysicalModel.Node;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalNodes.Mappers;

[Mapper]
public partial class RestraintResponseMapper : IMapper<RestraintResponse, RestraintCommand>
{
    public RestraintCommand Map(RestraintResponse from) => this.ToCommand(from);

    public partial RestraintCommand ToCommand(RestraintResponse response);
}
