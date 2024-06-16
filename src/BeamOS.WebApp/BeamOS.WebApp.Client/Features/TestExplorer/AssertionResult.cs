namespace BeamOS.WebApp.Client.Features.TestExplorer;

public record AssertionResult<T>(T ExpectedValue, T CalculatedValue);
