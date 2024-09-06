using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Common;

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
    public RestraintContract? Restraint { get; init; }
    public Dictionary<string, object>? CustomData { get; init; }

    public CreateNodeRequest(
        string modelId,
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        string lengthUnit,
        RestraintContract? restraint = null
    )
        : this(modelId, new(xCoordinate, yCoordinate, zCoordinate, lengthUnit), restraint) { }

    [JsonConstructor]
    public CreateNodeRequest(string modelId, Point locationPoint, RestraintContract? restraint)
    {
        this.ModelId = modelId;
        this.LocationPoint = locationPoint;
        this.Restraint = restraint;
    }
}

public record Point : BeamOsContractBase
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

public record PatchPointRequest
{
    public required string LengthUnit { get; init; }
    public double? XCoordinate { get; init; }
    public double? YCoordinate { get; init; }
    public double? ZCoordinate { get; init; }
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
}
