using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Application.PhysicalModel.Common;

public record PatchPointCommand(PatchPointRequest PatchRequest, Point Point);
