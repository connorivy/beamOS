namespace BeamOS.PhysicalModel.Contracts.Nodes;
public record NodeResponse(
    string Id,
    string ModelId,
    PointResponse LocationPoint,
    List<string> PointLoadIds,
    RestraintsResponse Restraints);

public record PointResponse(LengthResponse XCoordinate, LengthResponse YCoordinate, LengthResponse ZCoordinate);
public record LengthResponse(double Value, string Unit);
public record RestraintsResponse(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ);
