using Microsoft.AspNetCore.Components;
using Xunit;

namespace BeamOs.WebApp.Client.Components.Features.TestExplorer;

public partial class AssertionResultMatrixView
{
    [Parameter]
    public required string ComparedValueName { get; init; }

    [Parameter]
    public required AssertionResult<double?[,]> AssertionResultMatrix { get; init; }

    [Parameter]
    public ICollection<string>? ComparedValueNameCollection { get; init; }

    private static double? GetDifferenceOrNull(double expected, double? calculated)
    {
        if (calculated is not double typedCalculated)
        {
            return null;
        }
        return Math.Round(expected - typedCalculated, 5);
    }

    private static double?[,] GetDifferenceMatrix(double?[,] expected, double?[,] calculated)
    {
        double?[,] result = new double?[expected.GetLength(0), expected.GetLength(1)];

        for (int row = 0; row < expected.GetLength(1); row++)
        {
            for (int col = 0; col < expected.GetLength(0); col++)
            {
                if (
                    expected[row, col] is double expectedVal
                    && calculated[row, col] is double calculatedVal
                )
                {
                    result[row, col] = Math.Round(expectedVal - calculatedVal, 4);
                }
            }
        }

        return result;
    }

    public IEnumerable<(string, double?[,])> ExpectedCalculatedDifferenceEnumerable =>

        [
            ($"Expected {this.ComparedValueName}", this.AssertionResultMatrix.ExpectedValue),
            ($"Calculated {this.ComparedValueName}", this.AssertionResultMatrix.CalculatedValue),
            ($"Difference", GetDifferenceMatrix(this.AssertionResultMatrix.ExpectedValue, this.AssertionResultMatrix.CalculatedValue))
        ];

    public static double?[,] ToNullable(double[,] original)
    {
        int numRows = original.GetLength(0);
        int numCols = original.GetLength(1);
        double?[,] copy = new double?[numRows, numRows];

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                copy[row, col] = original[row, col];
            }
        }

        return copy;
    }
}
