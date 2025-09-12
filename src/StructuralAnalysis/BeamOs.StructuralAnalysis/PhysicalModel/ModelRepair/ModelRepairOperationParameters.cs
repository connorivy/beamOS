using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

internal readonly record struct ModelRepairOperationParameters
{
    public required Length VeryRelaxedTolerance { get; init; }
    public required Length RelaxedTolerance { get; init; }
    public required Length StandardTolerance { get; init; }
    public required Length StrictTolerance { get; init; }
    public required Length VeryStrictTolerance { get; init; }

    // public Angle VeryRelaxedAngleTolerance { get; init; } = new(10, AngleUnit.Degree);
    // public Angle RelaxedAngleTolerance { get; init; } = new(10, AngleUnit.Degree);
    // public Angle StandardAngleTolerance { get; init; } = new(5, AngleUnit.Degree);
    // public Angle StrictAngleTolerance { get; init; } = new(2, AngleUnit.Degree);
    // public Angle VeryStrictAngleTolerance { get; init; } = new(2, AngleUnit.Degree);

    public AxisAlignmentToleranceLevel GetAxisAlignmentToleranceLevel(Length length)
    {
        length = length.Abs();
        if (length < this.VeryStrictTolerance)
        {
            return AxisAlignmentToleranceLevel.VeryStrict;
        }
        if (length < this.StrictTolerance)
        {
            return AxisAlignmentToleranceLevel.Strict;
        }
        if (length < this.StandardTolerance)
        {
            return AxisAlignmentToleranceLevel.Standard;
        }
        if (length < this.RelaxedTolerance)
        {
            return AxisAlignmentToleranceLevel.Relaxed;
        }
        return AxisAlignmentToleranceLevel.VeryRelaxed;
    }

    // public AxisAlignmentToleranceLevel GetAxisAlignmentToleranceLevel(Angle angle)
    // {
    //     angle = angle.Abs();
    //     if (angle < this.VeryStrictAngleTolerance)
    //     {
    //         return AxisAlignmentToleranceLevel.VeryStrict;
    //     }
    //     if (angle < this.StrictAngleTolerance)
    //     {
    //         return AxisAlignmentToleranceLevel.Strict;
    //     }
    //     if (angle < this.StandardAngleTolerance)
    //     {
    //         return AxisAlignmentToleranceLevel.Standard;
    //     }
    //     if (angle < this.RelaxedAngleTolerance)
    //     {
    //         return AxisAlignmentToleranceLevel.Relaxed;
    //     }
    //     return AxisAlignmentToleranceLevel.VeryRelaxed;
    // }

    public Length GetLengthTolerance(AxisAlignmentToleranceLevel level)
    {
        return level switch
        {
            AxisAlignmentToleranceLevel.VeryRelaxed => this.VeryRelaxedTolerance,
            AxisAlignmentToleranceLevel.Relaxed => this.RelaxedTolerance,
            AxisAlignmentToleranceLevel.Standard => this.StandardTolerance,
            AxisAlignmentToleranceLevel.Strict => this.StrictTolerance,
            AxisAlignmentToleranceLevel.VeryStrict => this.VeryStrictTolerance,
            AxisAlignmentToleranceLevel.Undefined or _ => throw new ArgumentOutOfRangeException(
                nameof(level),
                level,
                null
            ),
        };
    }

    // public Angle GetAngleTolerance(AxisAlignmentToleranceLevel level)
    // {
    //     return level switch
    //     {
    //         AxisAlignmentToleranceLevel.VeryRelaxed => this.VeryRelaxedAngleTolerance,
    //         AxisAlignmentToleranceLevel.Relaxed => this.RelaxedAngleTolerance,
    //         AxisAlignmentToleranceLevel.Standard => this.StandardAngleTolerance,
    //         AxisAlignmentToleranceLevel.Strict => this.StrictAngleTolerance,
    //         AxisAlignmentToleranceLevel.VeryStrict => this.VeryStrictAngleTolerance,
    //         AxisAlignmentToleranceLevel.Undefined or _ => throw new ArgumentOutOfRangeException(
    //             nameof(level),
    //             level,
    //             null
    //         ),
    //     };
    // }

    public AxisAlignmentTolerance GetAxisAlignmentTolerance(Node startNode, Node endNode)
    {
        return new(
            this.GetAxisAlignmentToleranceLevel(
                startNode.LocationPoint.X - endNode.LocationPoint.X
            ),
            this.GetAxisAlignmentToleranceLevel(
                startNode.LocationPoint.Y - endNode.LocationPoint.Y
            ),
            this.GetAxisAlignmentToleranceLevel(startNode.LocationPoint.Z - endNode.LocationPoint.Z)
        );
    }

    public AxisAlignmentTolerance GetAxisAlignmentTolerance(
        Common.Point startNodeLocation,
        Common.Point endNodeLocation
    )
    {
        return new(
            this.GetAxisAlignmentToleranceLevel(startNodeLocation.X - endNodeLocation.X),
            this.GetAxisAlignmentToleranceLevel(startNodeLocation.Y - endNodeLocation.Y),
            this.GetAxisAlignmentToleranceLevel(startNodeLocation.Z - endNodeLocation.Z)
        );
    }

    internal Length GetToleranceForRule(IModelRepairRule rule) => this.GetTolerance(rule.RuleType);

    internal Length GetTolerance(ModelRepairRuleType ruleType) =>
        ruleType switch
        {
            ModelRepairRuleType.Favorable => this.RelaxedTolerance,
            ModelRepairRuleType.Standard => this.StandardTolerance,
            ModelRepairRuleType.Unfavorable => this.StrictTolerance,
            ModelRepairRuleType.Undefined or _ => throw new ArgumentOutOfRangeException(
                nameof(ruleType),
                ruleType,
                null
            ),
        };
}
