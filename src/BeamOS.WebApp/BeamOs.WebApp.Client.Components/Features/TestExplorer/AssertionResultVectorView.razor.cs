using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Client.Components.Features.TestExplorer;

public partial class AssertionResultVectorView : ComponentBase
{
    [Parameter]
    public required string ComparedValueName { get; init; }

    [Parameter]
    public required AssertionResult<double[]> AssertionResultArray { get; init; }

    private static double? GetDifferenceOrNull(double expected, double? calculated)
    {
        if (calculated is not double typedCalculated)
        {
            return null;
        }
        return Math.Round(expected - typedCalculated, 5);
    }
}
