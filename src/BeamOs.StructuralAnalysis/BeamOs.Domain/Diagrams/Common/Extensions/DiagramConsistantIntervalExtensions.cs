using BeamOs.Domain.Diagrams.Common.ValueObjects;
using UnitsNet;

namespace BeamOs.Domain.Diagrams.Common.Extensions;

public static class DiagramConsistantIntervalExtensions
{
    internal static (double leftValue, double rightValue) GetValueAtLocation(
        this IList<DiagramConsistantInterval> intervals,
        Length location,
        Length equalityTolerance,
        out bool isBetweenConsistantIntervals
    )
    {
        for (int i = 0; i < intervals.Count; i++)
        {
            DiagramConsistantInterval interval = intervals[i];
            if (location < interval.StartLocation)
            {
                continue;
            }

            // not needed for ordered intervals
            if (location > interval.EndLocation)
            {
                continue;
            }

            double left = interval.EvalutateAtLocation(location);
            double? right = null;
            if (i < intervals.Count - 1 && location.Equals(interval.EndLocation, equalityTolerance))
            {
                DiagramConsistantInterval rightInterval = intervals[i + 1];
                if (!rightInterval.Length.Equals(new Length(), equalityTolerance))
                {
                    right = intervals[i + 1].EvalutateAtLocation(location);
                }
            }
            isBetweenConsistantIntervals = right is not null;
            return (left, right ?? left);
        }

        throw new Exception("Out of bounds, I guess??");
    }
}
