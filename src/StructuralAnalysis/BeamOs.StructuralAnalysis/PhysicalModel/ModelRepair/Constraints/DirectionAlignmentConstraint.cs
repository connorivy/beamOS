using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Constraints;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

internal record DirectionAlignmentConstraint(AxisAlignmentToleranceLevel ToleranceLevel)
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
        var origDirX = originalEndNodeLocation.X - originalStartNodeLocation.X;
        var origDirY = originalEndNodeLocation.Y - originalStartNodeLocation.Y;
        var origDirZ = originalEndNodeLocation.Z - originalStartNodeLocation.Z;
        var newDirX =
            (newEndNodeLocation?.X ?? originalEndNodeLocation.X)
            - (newStartNodeLocation?.X ?? originalStartNodeLocation.X);
        var newDirY =
            (newEndNodeLocation?.Y ?? originalEndNodeLocation.Y)
            - (newStartNodeLocation?.Y ?? originalStartNodeLocation.Y);
        var newDirZ =
            (newEndNodeLocation?.Z ?? originalEndNodeLocation.Z)
            - (newStartNodeLocation?.Z ?? originalStartNodeLocation.Z);
        var deltaX = origDirX - newDirX;
        var deltaY = origDirY - newDirY;
        var deltaZ = origDirZ - newDirZ;
        var diff = Length.FromMeters(
            Math.Sqrt(
                Math.Pow(deltaX.Meters, 2) + Math.Pow(deltaY.Meters, 2) + Math.Pow(deltaZ.Meters, 2)
            )
        );
        var allowed = Length.FromMeters(
            ModelRepairRuleUtils.GetToleranceValue(
                this.ToleranceLevel,
                modelRepairOperationParameters
            )
        );
        return diff <= allowed;
    }
}
