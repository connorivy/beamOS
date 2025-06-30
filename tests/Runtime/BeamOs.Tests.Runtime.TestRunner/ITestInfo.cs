using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using BeamOs.CodeGen.EditorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Common;
using TestResult = BeamOs.Tests.Common.TestResult;

namespace BeamOs.Tests.Runtime.TestRunner;

public enum TestType
{
    Undefined = 0,
    StructuralAnalysis,
    ModelRepair,
}

public abstract class TestInfoBase
{
    public object[]? TestData { get; init; }
    public required Func<Task> ExecuteTest { get; init; }
    public required TestType TestType { get; init; }
    public required string TestName { get; init; }

    public async Task<string> DisplayAndReturnId(
        IEditorApiAlpha editorApi,
        string? currentlyDisplayedTestId
    )
    {
        var thisTestId = this.GetTestId();
        if (currentlyDisplayedTestId != thisTestId)
        {
            await editorApi.ClearAsync();
            await this.DisplayNewModel(editorApi);
        }

        return thisTestId;
    }

    public abstract IEnumerable<IBeamOsEntityResponse> GetDisplayableEntities();

    protected abstract Task DisplayNewModel(IEditorApiAlpha editorApi);

    protected virtual ValueTask BeforeTest() => ValueTask.CompletedTask;

    protected virtual ValueTask AfterTest() => ValueTask.CompletedTask;

    public virtual ValueTask<IList<TestResult>> GetTestResultsAsync()
    {
        return ValueTask.FromResult<IList<TestResult>>([]);
    }

    public abstract string GetTestId();

    public event EventHandler<TestResult>? OnTestResult;

    public async Task RunTest()
    {
        void OnAssertedEqual(object? _, ComparedObjectEventArgs args) =>
            this.OnTestResult?.Invoke(
                this,
                new TestResult(
                    args.BeamOsObjectType,
                    args.BeamOsObjectId,
                    this.TestName,
                    args.ComparedObjectPropertyName,
                    args.ExpectedValue,
                    args.CalculatedValue,
                    TestResultStatus.Success,
                    null,
                    args.ComparedValueNameCollection
                )
            );

        Asserter.AssertedEqual2 += OnAssertedEqual;

        try
        {
            await this.BeforeTest();
            await this.ExecuteTest();
        }
        catch (Exception ex)
        {
            ;
        }
        finally
        {
            Asserter.AssertedEqual2 -= OnAssertedEqual;
            await this.AfterTest();
        }
    }
}

public class ModelRepairTestInfo<TTestClass> : TestInfoBase
{
    [SetsRequiredMembers]
    public ModelRepairTestInfo(
        Func<TTestClass, Task> executeTest,
        string testName,
        TTestClass testClass,
        IStructuralAnalysisApiClientV1 apiClient
    )
    {
        this.TestName = testName;
        this.TestType = TestType.ModelRepair;
        this.TestClass = testClass;
        this.ExecuteTest = async () => await executeTest(testClass);
        this.ApiClient = apiClient;
    }

    public Guid? ModelId { get; set; }
    public ModelResponse? ModelResponse { get; set; }
    public ModelProposalResponse? ModelProposalResponse { get; set; }
    public TTestClass TestClass { get; set; }
    public IStructuralAnalysisApiClientV1 ApiClient { get; }

    protected override ValueTask BeforeTest()
    {
        BeamOsModelBuilder.ModelCreated += this.OnModelCreated;
        TestUtils.Asserter.ModelProposalVerified += this.OnProposalVerified;
        return ValueTask.CompletedTask;
    }

    private void OnModelCreated(object? sender, Guid modelId)
    {
        this.ModelId = modelId;
    }

    private void OnProposalVerified(object? sender, ModelProposalResponse modelProposalResponse)
    {
        this.ModelProposalResponse = modelProposalResponse;
    }

    protected override async ValueTask AfterTest()
    {
        BeamOsModelBuilder.ModelCreated -= this.OnModelCreated;
        TestUtils.Asserter.ModelProposalVerified -= this.OnProposalVerified;

        if (this.ModelId is null)
        {
            throw new Exception("ModelId is null");
        }

        var response = await this.ApiClient.GetModelAsync(this.ModelId.Value);
        response.ThrowIfError();
        this.ModelResponse = response.Value;
    }

    protected override async Task DisplayNewModel(IEditorApiAlpha editorApi)
    {
        if (this.ModelResponse is null || this.ModelProposalResponse is null)
        {
            throw new InvalidOperationException("ModelResponse or ModelProposal is null");
        }

        await editorApi.CreateModelAsync(this.ModelResponse);
        await editorApi.DisplayModelProposalAsync(this.ModelProposalResponse);
    }

    public override IEnumerable<IBeamOsEntityResponse> GetDisplayableEntities()
    {
        yield return this.ModelResponse
            ?? throw new InvalidOperationException("ModelResponse is null");
        yield return this.ModelProposalResponse
            ?? throw new InvalidOperationException("ModelProposalResponse is null");
    }

    public override string GetTestId()
    {
        return $"{this.TestType}_{this.TestName}";
    }

    public override ValueTask<IList<TestResult>> GetTestResultsAsync()
    {
        TestResult testResult = new(
            BeamOsObjectType.Model,
            this.ModelId?.ToString() ?? string.Empty,
            this.TestName,
            "Model Repair",
            JsonSerializer.Serialize(this.ModelProposalResponse, BeamOsSerializerOptions.Pretty),
            null,
            TestResultStatus.Success,
            null,
            []
        );
        IList<TestResult> result = [testResult];
        return ValueTask.FromResult(result);
    }
}

public class StructuralAnalysisTestInfo<TTestClass> : TestInfoBase
{
    [SetsRequiredMembers]
    public StructuralAnalysisTestInfo(
        Func<TTestClass, Task> executeTest,
        string testName,
        TTestClass testClass,
        ModelResponse modelResponse
    )
    {
        this.TestName = testName;
        this.TestType = TestType.StructuralAnalysis;
        this.TestClass = testClass;
        this.ExecuteTest = async () => await executeTest(testClass);
        this.ModelResponse = modelResponse;
    }

    public ModelResponse ModelResponse { get; set; }
    public TTestClass TestClass { get; set; }
    private List<TestResult> TestResults { get; } = [];

    private void OnTestResult2(object? sender, TestResult testResult)
    {
        this.TestResults.Add(testResult);
    }

    protected override ValueTask BeforeTest()
    {
        this.OnTestResult += this.OnTestResult2;
        return ValueTask.CompletedTask;
    }

    protected override ValueTask AfterTest()
    {
        this.OnTestResult -= this.OnTestResult2;
        return ValueTask.CompletedTask;
    }

    public virtual ITestFixture? GetTestFixture() =>
        this.TestData?.FirstOrDefault() as ITestFixture;

    protected override async Task DisplayNewModel(IEditorApiAlpha editorApi)
    {
        await editorApi.CreateModelAsync(this.ModelResponse);
    }

    public override IEnumerable<IBeamOsEntityResponse> GetDisplayableEntities()
    {
        yield return this.ModelResponse
            ?? throw new InvalidOperationException("ModelResponse is null");
    }

    public override string GetTestId() => this.ModelResponse.Id.ToString();

    public SourceInfo? SourceInfo => this.GetTestFixture()?.SourceInfo;

    public override ValueTask<IList<TestResult>> GetTestResultsAsync()
    {
        return ValueTask.FromResult<IList<TestResult>>(this.TestResults);
    }
}
