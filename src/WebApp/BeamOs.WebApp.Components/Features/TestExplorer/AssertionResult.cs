namespace BeamOs.WebApp.Client.Components.Features.TestExplorer;

public record AssertionResult<T>(T ExpectedValue, T CalculatedValue);
