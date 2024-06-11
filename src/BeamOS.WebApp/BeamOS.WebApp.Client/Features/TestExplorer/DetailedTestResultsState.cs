using Fluxor;

namespace BeamOS.WebApp.Client.Features.TestExplorer;

[FeatureState]
public record DetailedTestResultsState(
    string? NameOfAssertionResult,
    AssertionResult<double>? DoubleAssertionResult,
    AssertionResult<double[]>? DoubleVectorAssertionResult,
    AssertionResult<double[,]>? DoubleMatrixAssertionResult
)
{
    private DetailedTestResultsState()
        : this(null, null, null, null) { }
}

public record AssertionResult<T>(T ExpectedValue, T CalculatedValue);
