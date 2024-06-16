namespace BeamOs.Tests.TestRunner;

public record TestResult(TestResultStatus Status, string? ResultMessage);

public record TestResult<T>(
    T ExpectedValue,
    T CalculatedValue,
    string ComparedValueName,
    TestResultStatus ResultStatus,
    string? ResultMessage
) : TestResult(ResultStatus, ResultMessage);

public enum TestResultStatus
{
    Undefined = 0,
    Failure,
    Skipped,
    Success,
}

public enum TestProgressStatus
{
    Undefined = 0,
    Finished,
    InProgress,
    NotStarted
}
