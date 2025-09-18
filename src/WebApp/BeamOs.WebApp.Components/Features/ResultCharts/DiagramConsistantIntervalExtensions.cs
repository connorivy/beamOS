using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using UnitsNet;

namespace BeamOs.WebApp.Components.Features.ResultCharts;

internal static class DiagramConsistantIntervalExtensions
{
    internal static (double leftValue, double rightValue) GetValueAtLocation(
        this IList<IDiagramConsistentIntervalResponse> intervals,
        Length location,
        Length equalityTolerance,
        out bool isBetweenConsistantIntervals
    )
    {
        for (int i = 0; i < intervals.Count; i++)
        {
            var interval = intervals[i];
            if (location < interval.StartLocation.ToUnitsNet() - equalityTolerance)
            {
                continue;
            }

            // not needed for ordered intervals
            if (location > interval.EndLocation.ToUnitsNet() + equalityTolerance)
            {
                continue;
            }

            double left = interval.EvaluateAtLocation(location, equalityTolerance);
            double? right = null;
            if (
                i < intervals.Count - 1
                && location.Equals(interval.EndLocation.ToUnitsNet(), equalityTolerance)
            )
            {
                //var rightInterval = intervals[i + 1];
                right = intervals[i + 1].EvaluateAtLocation(location, equalityTolerance);
                //if (
                //    !rightInterval
                //        .StartLocation
                //        .MapToLength()
                //        .Equals(new Length(), equalityTolerance)
                //)
                //{
                //    right = intervals[i + 1].EvaluateAtLocation(location);
                //}
            }
            isBetweenConsistantIntervals = right is not null;
            return (left, right ?? left);
        }

        throw new Exception("Out of bounds, I guess??");
    }

    public static double EvaluateAtLocation(
        this IDiagramConsistentIntervalResponse interval,
        Length location,
        Length equalityTolerance
    )
    {
        if (
            location < interval.StartLocation.ToUnitsNet() - equalityTolerance
            || location > interval.EndLocation.ToUnitsNet() + equalityTolerance
        )
        {
            throw new Exception("Out of bounds my guy");
        }

        return interval.PolynomialCoefficients.Evaluate(
            location.As(interval.StartLocation.Unit.ToUnitsNet())
        );
    }

    public static double Evaluate(this IList<double> coefficients, double loc)
    {
        double result = 0;
        for (var i = 0; i < coefficients.Count; i++)
        {
            result += coefficients[i] * Math.Pow(loc, i);
        }

        return result;
    }
}
