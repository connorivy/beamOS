using System.Globalization;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOs.Domain.UnitTests.DirectStiffnessMethod.Common.Extensions;

internal static class ArrayExtensions
{
    public static void AssertAlmostEqual(
        this double[] calculated,
        double[] expected,
        int numDigits = 3
    )
    {
        if (calculated.Length != expected.Length)
        {
            throw new Exception("Calculated and expected values have different lengths");
        }

        if (calculated.Length == 0)
        {
            return;
        }

        for (int col = 0; col < calculated.Length; col++)
        {
            Assert.Equal(expected[col], calculated[col], numDigits);
        }
    }

    public static string MatrixUnequalExceptionMessage(
        Matrix<double> calculatedMatrix,
        Matrix<double> expectedMatrix
    )
    {
        var diff = calculatedMatrix - expectedMatrix;
        return string.Format(
            CultureInfo.CurrentCulture,
            "{1}{0}Expected: {2}{0}Actual:   {3}{0}Diff:    {4}{0}",
            Environment.NewLine,
            "Assert.Equal() Failure",
            expectedMatrix.ToString(12, 12) ?? "(null)",
            calculatedMatrix.ToString(12, 12) ?? "(null)",
            diff.ToString(12, 12) ?? "(null)"
        );
    }
}
