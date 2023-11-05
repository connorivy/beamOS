using BeamOS.Common.Domain.Models;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.Common.Domain.ValueObjects;
public class Point : BeamOSValueObject
{
    public Point(double xCoordinate, double yCoordinate, double zCoordinate, LengthUnit lengthUnit)
    {
        this.XCoordinate = new Length(xCoordinate, lengthUnit);
        this.YCoordinate = new Length(yCoordinate, lengthUnit);
        this.ZCoordinate = new Length(zCoordinate, lengthUnit);
    }
    public Point(Length xCoordinate, Length yCoordinate, Length zCoordinate)
    {
        this.XCoordinate = xCoordinate;
        this.YCoordinate = yCoordinate;
        this.ZCoordinate = zCoordinate;
    }
    public Length XCoordinate { get; private set; }
    public Length YCoordinate { get; private set; }
    public Length ZCoordinate { get; private set; }

    public static Point operator -(Point lhs, Point rhs) => new(
        lhs.XCoordinate - rhs.XCoordinate,
        lhs.YCoordinate - rhs.YCoordinate,
        rhs.ZCoordinate - rhs.ZCoordinate
      );
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.XCoordinate;
        yield return this.YCoordinate;
        yield return this.ZCoordinate;
    }
}
