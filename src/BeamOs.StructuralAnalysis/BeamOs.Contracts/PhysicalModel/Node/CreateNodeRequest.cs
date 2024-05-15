using System.ComponentModel.DataAnnotations;
using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.Node;

//public record CreateNodeRequest(
//    [Required] string ModelId,
//    double XCoordinate,
//    double YCoordinate,
//    double ZCoordinate,
//    string? LengthUnit = null,
//    RestraintRequest? Restraint = null
//)
//{
//    //public CreateNodeRequest(
//    //    string ModelId,
//    //    PointRequest LocationPoint,
//    //    RestraintRequest? Restraint = null) : this(ModelId, LocationPoint.)
//}

public record CreateNodeRequest(
    string ModelId,
    PointRequest LocationPoint,
    RestraintRequest? Restraint = null
)
{
    public CreateNodeRequest(
        [Required] string modelId,
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        string lengthUnit,
        RestraintRequest? restraint = null
    )
        : this(
            modelId,
            new(
                new(xCoordinate, lengthUnit),
                new(yCoordinate, lengthUnit),
                new(zCoordinate, lengthUnit)
            ),
            restraint
        ) { }
}

public record PointRequest(
    UnitValueDto XCoordinate,
    UnitValueDto YCoordinate,
    UnitValueDto ZCoordinate
);

public record RestraintRequest(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ
)
{
    public static RestraintRequest Free { get; } = new(true, true, true, true, true, true);
    public static RestraintRequest Fixed { get; } = new(false, false, false, false, false, false);
}
