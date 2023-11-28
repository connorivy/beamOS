using BeamOS.Common.Contracts;

namespace BeamOS.PhysicalModel.Contracts.Node;
public record NodeResponse(
    string Id,
    string ModelId,
    PointResponse LocationPoint,
    //List<string> PointLoadIds,
    RestraintResponse Restraint);

public record PointResponse(
    UnitValueDTO XCoordinate,
    UnitValueDTO YCoordinate,
    UnitValueDTO ZCoordinate);
public record RestraintResponse(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ);
