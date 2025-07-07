using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Constraints;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

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
        if (newStartNodeLocation == null || newEndNodeLocation == null)
        {
            return false;
        }
        Length tolerance = modelRepairOperationParameters.GetLengthTolerance(
            AxisAlignmentToleranceLevel.Strict
        );

        return this.CoordinateSystemDirection3d switch
        {
            CoordinateSystemDirection3D.AlongX => Math.Abs(
                (newStartNodeLocation.X - newEndNodeLocation.X).Meters
            ) < tolerance.Meters,
            CoordinateSystemDirection3D.AlongY => Math.Abs(
                (newStartNodeLocation.Y - newEndNodeLocation.Y).Meters
            ) < tolerance.Meters,
            CoordinateSystemDirection3D.AlongZ => Math.Abs(
                (newStartNodeLocation.Z - newEndNodeLocation.Z).Meters
            ) < tolerance.Meters,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
