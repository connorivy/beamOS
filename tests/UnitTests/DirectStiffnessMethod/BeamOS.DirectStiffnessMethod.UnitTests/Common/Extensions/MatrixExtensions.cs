using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using Xunit.Sdk;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Extensions;

public static class MatrixExtensions
{
    private static Matrix<double> GetRounded(this Matrix<double> matrix, int numDigits = 3) =>
        matrix.Map(m =>
        {
            m = Math.Round(m, numDigits);
            if (m == double.NegativeZero)
            {
                m = default;
            }

            return m;
        });

    public static void AssertAlmostEqual(
        this Matrix<double> calculatedMatrix,
        Matrix<double> expectedMatrix,
        int numDigits = 3
    )
    {
        var calculated = calculatedMatrix.GetRounded(numDigits);
        var expected = expectedMatrix.GetRounded(numDigits);

        var exception = Record.Exception(() => Assert.Equal(expected, calculated));

        Assert.False(
            exception is EqualException,
            MatrixUnequalExceptionMessage(calculated, expected)
        );
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

    public static void AssertSymmetric(this Matrix<double> matrix, int numDigits = 3)
    {
        var matrixRounded = matrix.GetRounded(numDigits);
        var exception = Record.Exception(() => Assert.True(matrixRounded.IsSymmetric()));

        Assert.False(
            exception is TrueException,
            MatrixUnsymmetricalExceptionMessage(matrixRounded)
        );
    }

    public static string MatrixUnsymmetricalExceptionMessage(Matrix<double> matrix)
    {
        Assert.Equal(matrix.ColumnCount, matrix.RowCount);
        var reflectedDiff = Matrix<double>.Build.Dense(matrix.ColumnCount, matrix.RowCount);
        for (var col = 0; col < matrix.ColumnCount; col++)
        {
            for (var row = col; row < matrix.RowCount; row++)
            {
                var diff = matrix[row, col] - matrix[col, row];
                reflectedDiff[col, row] = diff;
                reflectedDiff[row, col] = diff;
            }
        }
        return string.Format(
            CultureInfo.CurrentCulture,
            "{1}{0}Matrix:   {2}{0}Diff:    {3}{0}",
            Environment.NewLine,
            "Assert.Symmetric() Failure",
            matrix.ToString(12, 12) ?? "(null)",
            reflectedDiff.ToString(12, 12) ?? "(null)"
        );
    }
}
