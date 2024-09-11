using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOs.Contracts.PhysicalModel.Node;

public record NodeResponse(
    string Id,
    string ModelId,
    Point LocationPoint,
    RestraintContract Restraint,
    Dictionary<string, object>? CustomData = null
) : BeamOsEntityContractBase(Id);

public record RestraintContract(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ
) : BeamOsContractBase
{
    public static RestraintContract Free { get; } = new(true, true, true, true, true, true);
    public static RestraintContract FreeXzPlane { get; } =
        new(true, false, true, false, true, false);
    public static RestraintContract FreeXyPlane { get; } =
        new(true, true, false, false, false, true);
    public static RestraintContract Pinned { get; } = new(false, false, false, true, true, true);
    public static RestraintContract Fixed { get; } = new(false, false, false, false, false, false);
}
