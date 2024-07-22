using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOs.Contracts.PhysicalModel.Node;

public record NodeResponse(
    string Id,
    string ModelId,
    PointResponse LocationPoint,
    //List<string> PointLoadIds,
    RestraintResponse Restraint
) : BeamOsEntityContractBase(Id);

public record PointResponse(
    UnitValueDto XCoordinate,
    UnitValueDto YCoordinate,
    UnitValueDto ZCoordinate
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
