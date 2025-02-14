namespace BeamOs.WebApp.Components.Features.TestExplorer;

public record AssertionResult<T>(T ExpectedValue, T CalculatedValue);
