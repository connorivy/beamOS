using BeamOs.Tests.TestRunner;
using Fluxor.Blazor.Web.Components;

namespace BeamOS.WebApp.Client.Features.TestExplorer;

public partial class DetailedTestResultsView : FluxorComponent
{
    public string? ComparedValueName { get; set; }

    public double[]? ExpectedDoubleArrayValue { get; set; }

    public double[]? CalculatedDoubleArrayValue { get; set; }

    public AssertionResult<double[,]>? AssertionResultDoubleMatrix { get; set; }

    private bool isLoading;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.SubscribeToAction<TestExecutionBegun>(_ => this.isLoading = true);
        this.SubscribeToAction<TestExecutionCompleted>(arg =>
        {
            switch (arg.Result)
            {
                case TestResult<double[]> testResultDoubleArray:
                    this.ResetValues();
                    this.ExpectedDoubleArrayValue = testResultDoubleArray.ExpectedValue;
                    this.CalculatedDoubleArrayValue = testResultDoubleArray.CalculatedValue;
                    this.ComparedValueName = testResultDoubleArray.ComparedValueName;
                    break;
                case TestResult<double[,]> testResultDoubleMatrix:
                    this.ResetValues();
                    this.AssertionResultDoubleMatrix = new(
                        testResultDoubleMatrix.ExpectedValue,
                        testResultDoubleMatrix.CalculatedValue
                    );
                    this.ComparedValueName = testResultDoubleMatrix.ComparedValueName;
                    break;
                default:
                    this.ResetValues();
                    break;
            }
            this.isLoading = false;
        });
    }

    private void ResetValues()
    {
        this.AssertionResultDoubleMatrix = null;
        this.ExpectedDoubleArrayValue = null;
        this.CalculatedDoubleArrayValue = null;
        this.ComparedValueName = null;
    }

    private static double? GetDifferenceOrNull(double expected, double? calculated)
    {
        if (calculated is not double typedCalculated)
        {
            return null;
        }
        return Math.Round(expected - typedCalculated, 5);
    }

    public readonly struct TestExecutionBegun();

    public readonly record struct TestExecutionCompleted(TestResult Result);
}
