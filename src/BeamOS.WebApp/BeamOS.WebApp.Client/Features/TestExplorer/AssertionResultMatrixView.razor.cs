using Microsoft.AspNetCore.Components;

namespace BeamOS.WebApp.Client.Features.TestExplorer;

public partial class AssertionResultMatrixView
{
    [Parameter]
    public required string ComparedValueName { get; init; }

    [Parameter]
    public required AssertionResult<double[,]> AssertionResultMatrix { get; init; }

    private static double? GetDifferenceOrNull(double expected, double? calculated)
    {
        if (calculated is not double typedCalculated)
        {
            return null;
        }
        return Math.Round(expected - typedCalculated, 5);
    }
}
