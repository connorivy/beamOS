using BeamOS.Common.Application.Commands;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Application.Nodes.Commands;

public record CreateNodeCommand(
    GuidBasedIdCommand ModelId,
    double XCoordinate,
    double YCoordinate,
    double ZCoordinate,
    LengthUnit LengthUnit,
    RestraintsCommand? Restraint = null
);

public record RestraintsCommand(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ
);
