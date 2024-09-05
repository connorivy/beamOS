using BeamOS.Tests.Common;

namespace BeamOs.Tests.TestRunner;

public record TestResult(TestResultStatus Status, string? ResultMessage);

public record TestResult<T>(
    T ExpectedValue,
    T CalculatedValue,
    string ComparedValueName,
    TestResultStatus ResultStatus,
    string? ResultMessage
) : TestResult(ResultStatus, ResultMessage);

public record TestResult2(
    //BeamOsObjectType BeamOsObjectType,
    string BeamOsObjectId,
    string TestName,
    string ComparedValueName,
    object ExpectedValue,
    object CalculatedValue,
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
