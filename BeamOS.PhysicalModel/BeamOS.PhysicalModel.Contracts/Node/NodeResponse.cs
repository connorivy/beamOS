using BeamOS.PhysicalModel.Contracts.Common;

namespace BeamOS.PhysicalModel.Contracts.Node;
public record NodeResponse(
    string Id,
    string ModelId,
    PointResponse LocationPoint,
    List<string> PointLoadIds,
    RestraintsResponse Restraints);

public record PointResponse(
    UnitValueDTO XCoordinate,
    UnitValueDTO YCoordinate,
    UnitValueDTO ZCoordinate);
public record RestraintsResponse(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ);
