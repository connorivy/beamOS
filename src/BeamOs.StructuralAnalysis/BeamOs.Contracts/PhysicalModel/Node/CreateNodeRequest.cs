using System.Diagnostics.CodeAnalysis;
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

public record CreateNodeRequest
{
    public string ModelId { get; init; }
    public Point LocationPoint { get; init; }
    public RestraintRequest? Restraint { get; init; }

    public CreateNodeRequest(
        string modelId,
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        string lengthUnit,
        RestraintRequest? restraint = null
    )
        : this(modelId, new(xCoordinate, yCoordinate, zCoordinate, lengthUnit), restraint) { }

    [JsonConstructor]
    public CreateNodeRequest(string modelId, Point locationPoint, RestraintRequest? restraint)
    {
        this.ModelId = modelId;
        this.LocationPoint = locationPoint;
        this.Restraint = restraint;
    }
}

public record Point
{
    public required double XCoordinate { get; init; }
    public required double YCoordinate { get; init; }
    public required double ZCoordinate { get; init; }
    public required string LengthUnit { get; init; }

    public Point() { }

    [SetsRequiredMembers]
    public Point(double xCoordinate, double yCoordinate, double zCoordinate, string lengthUnit)
    {
        this.XCoordinate = xCoordinate;
        this.YCoordinate = yCoordinate;
        this.ZCoordinate = zCoordinate;
        this.LengthUnit = lengthUnit;
    }

    [SetsRequiredMembers]
    public Point(UnitValueDto xCoordinate, UnitValueDto yCoordinate, UnitValueDto zCoordinate)
        : this(xCoordinate.Value, yCoordinate.Value, zCoordinate.Value, xCoordinate.Unit)
    {
        if (xCoordinate.Unit != yCoordinate.Unit || xCoordinate.Unit != zCoordinate.Unit)
        {
            throw new InvalidOperationException("Cannot mix units");
        }
    }
}

//public record PatchPointRequest(
//    UnitValueDto? XCoordinate = null,
//    UnitValueDto? YCoordinate = null,
//    UnitValueDto? ZCoordinate = null
//)
//{
//    [JsonConstructor]
//    public PatchPointRequest(
//        string lengthUnit,
//        double? xCoordinate = null,
//        double? yCoordinate = null,
//        double? zCoordinate = null
//    )
//        : this(
//            xCoordinate.HasValue ? new(xCoordinate.Value, lengthUnit) : null,
//            yCoordinate.HasValue ? new(yCoordinate.Value, lengthUnit) : null,
//            zCoordinate.HasValue ? new(zCoordinate.Value, lengthUnit) : null
//        ) { }
//}

public record PatchPointRequest
{
    public required string LengthUnit { get; init; }
    public double? XCoordinate { get; init; }
    public double? YCoordinate { get; init; }
    public double? ZCoordinate { get; init; }
}

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
    public static RestraintRequest FreeXzPlane { get; } =
        new(true, false, true, false, true, false);
    public static RestraintRequest FreeXyPlane { get; } =
        new(true, true, false, false, false, true);
    public static RestraintRequest Pinned { get; } = new(false, false, false, true, true, true);
    public static RestraintRequest Fixed { get; } = new(false, false, false, false, false, false);
}

public record PatchRestraintRequest(
    bool? CanTranslateAlongX = null,
    bool? CanTranslateAlongY = null,
    bool? CanTranslateAlongZ = null,
    bool? CanRotateAboutX = null,
    bool? CanRotateAboutY = null,
    bool? CanRotateAboutZ = null
) { }

public record PatchNodeRequest
{
    public required string NodeId { get; init; }
    public PatchPointRequest? LocationPoint { get; init; }
    public PatchRestraintRequest? Restraint { get; init; }

    //public PatchNodeRequest(
    //    string modelId,
    //    double xCoordinate,
    //    double yCoordinate,
    //    double zCoordinate,
    //    string lengthUnit,
    //    PatchRestraintRequest? restraint = null
    //)
    //    : this(modelId, new(lengthUnit, xCoordinate, yCoordinate, zCoordinate), restraint) { }

    //[JsonConstructor]
    //public PatchNodeRequest(
    //    string nodeId,
    //    PatchPointRequest? locationPoint,
    //    PatchRestraintRequest? restraint
    //)
    //{
    //    this.NodeId = nodeId;
    //    this.LocationPoint = locationPoint;
    //    this.Restraint = restraint;
    //}
}
