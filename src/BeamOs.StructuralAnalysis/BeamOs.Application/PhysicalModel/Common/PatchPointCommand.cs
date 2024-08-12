using BeamOs.Contracts.PhysicalModel.Node;

namespace BeamOs.Application.PhysicalModel.Common;

public record PatchPointCommand(
    PatchPointRequest PatchRequest,
    Domain.Common.ValueObjects.Point Point
);
