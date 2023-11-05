using BeamOS.Common.Domain.Enums;
using BeamOS.Common.Domain.Models;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;
public class MomentLoad : BeamOSValueObject
{
    public MomentLoad(CoordinateSystemDirection3D direction, Torque magnitude)
    {
        this.Direction = direction;
        this.Magnitude = magnitude;
    }

    public CoordinateSystemDirection3D Direction { get; set; }
    public Torque Magnitude { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Direction;
        yield return this.Magnitude;
    }
}
