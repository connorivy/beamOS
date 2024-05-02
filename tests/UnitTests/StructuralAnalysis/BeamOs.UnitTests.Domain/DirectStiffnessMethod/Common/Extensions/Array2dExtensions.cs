using System.Globalization;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOs.UnitTests.Domain.DirectStiffnessMethod.Common.Extensions;

internal static class Array2dExtensions
{
    public static void AssertAlmostEqual(
        this double[,] calculated,
        double[,] expected,
        int numDigits = 3
    )
    {
        int numRows = calculated.GetLength(0);
        int numCols = calculated.GetLength(1);
        if (numRows != expected.GetLength(0) || numCols != expected.GetLength(1))
        {
            throw new Exception("Calculated and expected values have different lengths");
        }

        if (numRows == 0 || numCols == 0)
        {
            return;
        }

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                Assert.Equal(expected[row, col], calculated[row, col], numDigits);
            }
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
