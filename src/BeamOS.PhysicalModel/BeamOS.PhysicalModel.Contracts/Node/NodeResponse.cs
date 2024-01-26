using BeamOS.Common.Contracts;
using BeamOS.PhysicalModel.Contracts.Common;

namespace BeamOS.PhysicalModel.Contracts.Node;

public record NodeResponse(
    string Id,
    string ModelId,
    PointResponse LocationPoint,
    //List<string> PointLoadIds,
    RestraintResponse Restraint
) : BeamOsContractBase;

public record PointResponse(
    UnitValueDTO XCoordinate,
    UnitValueDTO YCoordinate,
    UnitValueDTO ZCoordinate
) : BeamOsContractBase;

public record RestraintResponse(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ
) : BeamOsContractBase
{
    public static RestraintResponse Fixed { get; } = new(false, false, false, false, false, false);
    public static RestraintResponse Free { get; } = new(true, true, true, true, true, true);
}
