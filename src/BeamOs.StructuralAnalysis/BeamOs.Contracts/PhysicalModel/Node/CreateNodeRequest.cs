using System.Text.Json.Serialization;
using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.Node;

// todo : unit test fails when this is a record due to not have 'json deserializer' attr
// but client generator fails to create this method when it's a class for some other reason
//public record CreateNodeRequest(
//    string ModelId,
//    PointRequest LocationPoint,
//    RestraintRequest? Restraint = null
//)
//{
//    public CreateNodeRequest(
//        string modelId,
//        double xCoordinate,
//        double yCoordinate,
//        double zCoordinate,
//        string lengthUnit,
//        RestraintRequest? restraint = null
//    )
//        : this(
//            modelId,
//            new(
//                new(xCoordinate, lengthUnit),
//                new(yCoordinate, lengthUnit),
//                new(zCoordinate, lengthUnit)
//            ),
//            restraint
//        ) { }
//}

public class CreateNodeRequest
{
    public string ModelId { get; init; }
    public PointRequest LocationPoint { get; init; }
    public RestraintRequest? Restraint { get; init; }

    public CreateNodeRequest(
        string modelId,
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

    [JsonConstructor]
    public CreateNodeRequest(
        string modelId,
        PointRequest locationPoint,
        RestraintRequest? restraint
    )
    {
        this.ModelId = modelId;
        this.LocationPoint = locationPoint;
        this.Restraint = restraint;
    }
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
