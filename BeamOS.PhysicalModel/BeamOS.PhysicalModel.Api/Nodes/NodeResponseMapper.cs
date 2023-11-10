using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.Mappers;
[Mapper]
public static partial class NodeResponseMapper
{
    public static partial NodeResponse ToResponse(this Node model);
}
