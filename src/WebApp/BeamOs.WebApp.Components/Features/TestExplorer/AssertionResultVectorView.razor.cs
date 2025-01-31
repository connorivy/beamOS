using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.TestExplorer;

public partial class AssertionResultVectorView : ComponentBase
{
    [Parameter]
    public required string ComparedValueName { get; init; }

    [Parameter]
    public required AssertionResult<double?[]> AssertionResultArray { get; init; }

    [Parameter]
    public ICollection<string>? ComparedValueNameCollection { get; init; }

    private static double? GetDifferenceOrNull(double? expected, double? calculated)
    {
        if (expected is not double typedExpected || calculated is not double typedCalculated)
        {
            return null;
        }
        return Math.Round(typedExpected - typedCalculated, 5);
    }
}
