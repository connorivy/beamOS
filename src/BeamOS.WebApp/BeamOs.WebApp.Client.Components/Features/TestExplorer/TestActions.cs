using BeamOs.Tests.TestRunner;

namespace BeamOs.WebApp.Client.Components.Features.TestExplorer;

public readonly struct TestExecutionBegun();

public readonly record struct ExecutionTestActionById(string TestId);

public readonly record struct ExecutionTestAction(TestInfo TestInfo);

public readonly record struct ExecutionTestActionResult(string TestId, TestResult Result);
