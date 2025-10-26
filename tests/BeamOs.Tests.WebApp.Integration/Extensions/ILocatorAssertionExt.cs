using Microsoft.Playwright;
using TUnit.Assertions.Exceptions;

namespace BeamOs.Tests.WebApp.Integration.Extensions;

public static class ILocatorExtensions
{
    public static async Task ExpectToHaveApproximateValueAsync(
        this ILocator locator,
        double expected,
        double epsilon = 1e-6,
        string? message = null
    )
    {
        var valueStr = await locator.InputValueAsync();
        if (!double.TryParse(valueStr, out var actual))
            throw new AssertionException($"Could not parse '{valueStr}' as a number.");

        if (Math.Abs(actual - expected) >= epsilon)
            throw new AssertionException(
                message ?? $"Expected value ≈ {expected}, but got {actual} (tolerance {epsilon})"
            );
    }
}
