using BeamOs.Application.PhysicalModel.Element1dAggregate.Queries;
using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Element1ds.Mappers;

[Mapper]
public static partial class GetElement1dRequestMapper
{
    public static partial GetElement1dByIdQuery ToCommand(this IdRequest request);
}
