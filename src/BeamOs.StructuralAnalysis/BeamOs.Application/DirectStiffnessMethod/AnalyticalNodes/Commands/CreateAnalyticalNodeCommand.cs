using BeamOs.Application.DirectStiffnessMethod.MomentLoads;
using BeamOs.Application.DirectStiffnessMethod.PointLoads;
using UnitsNet;

namespace BeamOs.Application.DirectStiffnessMethod.AnalyticalNodes.Commands;

public record CreateAnalyticalNodeCommand(
    string Id,
    string ModelId,
    PointCommand LocationPoint,
    RestraintCommand Restraint,
    List<CreatePointLoadCommand> PointLoads,
    List<CreateMomentLoadCommand> MomentLoads
);

public record PointCommand(Length XCoordinate, Length YCoordinate, Length ZCoordinate);

public record RestraintCommand(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ
);
