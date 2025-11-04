using MathNet.Numerics;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.Extensions;

internal static class PolynomialExtensions
{
    public static Func<double, double> ToFunc(this Polynomial poly)
    {
        return poly.Degree switch
        {
            0 => (x) => poly.Coefficients[0],
            1 => (x) => poly.Coefficients[1] * x + poly.Coefficients[0],
            2 => (x) =>
                poly.Coefficients[2] * Math.Pow(x, 2)
                + poly.Coefficients[1] * x
                + poly.Coefficients[0],
            3 => (x) =>
                poly.Coefficients[3] * Math.Pow(x, 3)
                + poly.Coefficients[2] * Math.Pow(x, 2)
                + poly.Coefficients[1] * x
                + poly.Coefficients[0],
            4 => (x) =>
                poly.Coefficients[4] * Math.Pow(x, 4)
                + poly.Coefficients[3] * Math.Pow(x, 3)
                + poly.Coefficients[2] * Math.Pow(x, 2)
                + poly.Coefficients[1] * x
                + poly.Coefficients[0],
            _ => throw new ArgumentNullException($"Unsupported polynomial degree, {poly.Degree}"),
        };
    }

    public static double SafeEvaluate(this Polynomial poly, double x)
    {
        if (poly.Degree == -1)
        {
            return 0;
        }
        return poly.Evaluate(x);
    }
}
