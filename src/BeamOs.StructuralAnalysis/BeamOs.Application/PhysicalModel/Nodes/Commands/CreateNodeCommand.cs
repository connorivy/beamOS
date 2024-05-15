using BeamOs.Application.Common.Commands;
using UnitsNet;

namespace BeamOs.Application.PhysicalModel.Nodes.Commands;

public record CreateNodeCommand(
    GuidBasedIdCommand ModelId,
    PointCommand LocationPoint,
    RestraintsCommand? Restraint = null
);

public record PointCommand(Length XCoordinate, Length YCoordinate, Length ZCoordinate);

public record RestraintsCommand(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ
);
