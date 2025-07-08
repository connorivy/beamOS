using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Constraints;

public record EqualCoordinateConstraint(CoordinateSystemDirection3D CoordinateSystemDirection3d)
    : IElementConstraint
{
    public bool IsSatisfied(
        Element1d element1d,
        ModelRepairOperationParameters modelRepairOperationParameters,
        Point originalStartNodeLocation,
        Point originalEndNodeLocation,
        Point? newStartNodeLocation,
        Point? newEndNodeLocation
    )
    {
        Length tolerance = modelRepairOperationParameters.GetLengthTolerance(
            AxisAlignmentToleranceLevel.Strict
        );

        if (newStartNodeLocation is not null && newEndNodeLocation is not null)
        {
            return this.CoordinateSystemDirection3d switch
            {
                CoordinateSystemDirection3D.AlongX => newStartNodeLocation.X.Equals(
                    newEndNodeLocation.X,
                    tolerance
                ),
                CoordinateSystemDirection3D.AlongY => newStartNodeLocation.Y.Equals(
                    newEndNodeLocation.Y,
                    tolerance
                ),
                CoordinateSystemDirection3D.AlongZ => newStartNodeLocation.Z.Equals(
                    newEndNodeLocation.Z,
                    tolerance
                ),
                _ => throw new ArgumentOutOfRangeException("Invalid coordinate system direction."),
            };
        }
        else if (newStartNodeLocation is not null)
        {
            return this.CoordinateSystemDirection3d switch
            {
                CoordinateSystemDirection3D.AlongX => newStartNodeLocation.X.Equals(
                    originalStartNodeLocation.X,
                    tolerance
                ),
                CoordinateSystemDirection3D.AlongY => newStartNodeLocation.Y.Equals(
                    originalStartNodeLocation.Y,
                    tolerance
                ),
                CoordinateSystemDirection3D.AlongZ => newStartNodeLocation.Z.Equals(
                    originalStartNodeLocation.Z,
                    tolerance
                ),
                _ => throw new ArgumentOutOfRangeException("Invalid coordinate system direction."),
            };
        }
        else if (newEndNodeLocation is not null)
        {
            return this.CoordinateSystemDirection3d switch
            {
                CoordinateSystemDirection3D.AlongX => newEndNodeLocation.X.Equals(
                    originalEndNodeLocation.X,
                    tolerance
                ),
                CoordinateSystemDirection3D.AlongY => newEndNodeLocation.Y.Equals(
                    originalEndNodeLocation.Y,
                    tolerance
                ),
                CoordinateSystemDirection3D.AlongZ => newEndNodeLocation.Z.Equals(
                    originalEndNodeLocation.Z,
                    tolerance
                ),
                _ => throw new ArgumentOutOfRangeException("Invalid coordinate system direction."),
            };
        }

        throw new InvalidOperationException(
            "At least one of the new node locations must be provided."
        );
    }
}
