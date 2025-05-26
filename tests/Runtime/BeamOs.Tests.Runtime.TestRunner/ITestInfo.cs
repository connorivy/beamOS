using System.Diagnostics.CodeAnalysis;
using BeamOs.CodeGen.EditorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Runtime.TestRunner;
using BeamOs.Tests.StructuralAnalysis.Integration;

namespace BeamOs.Tests.Common;

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

    public Task Display(
        IEditorApiAlpha editorApi,
        string? currentlyDisplayedTestId,
        out string newTestId
    )
    {
        var thisTestId = this.GetTestId();
        newTestId = thisTestId;
        if (currentlyDisplayedTestId == thisTestId)
        {
            return Task.CompletedTask;
        }

        return this.DisplayNewModel(editorApi);
    }

    protected abstract Task DisplayNewModel(IEditorApiAlpha editorApi);

    protected virtual ValueTask BeforeTest() => ValueTask.CompletedTask;

    protected virtual ValueTask AfterTest() => ValueTask.CompletedTask;

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
    where TTestClass : IModelResponseEmitter, new()
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
        Asserter.ModelProposalVerified += this.OnProposalVerified;
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
        Asserter.ModelProposalVerified -= this.OnProposalVerified;

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

    public override string GetTestId()
    {
        return $"{this.TestType}_{this.TestName}";
    }
}

public static class TestInfoRetriever
{
    public static IEnumerable<TestInfoBase> GetTestInfos()
    {
        var tests = new ModelRepairerTests();
        yield return new ModelRepairTestInfo<ModelRepairerTests>(
            static async (testClass) =>
                await testClass.ProposeRepairs_MergesCloseNodes_AddsNodeProposal(),
            nameof(ModelRepairerTests.ProposeRepairs_MergesCloseNodes_AddsNodeProposal),
            tests,
            AssemblySetup.StructuralAnalysisApiClient
        );
        yield return new ModelRepairTestInfo<ModelRepairerTests>(
            static async (testClass) =>
                await testClass.ProposeRepairs_NoCloseNodes_NoNodeProposals(),
            nameof(ModelRepairerTests.ProposeRepairs_NoCloseNodes_NoNodeProposals),
            tests,
            AssemblySetup.StructuralAnalysisApiClient
        );
    }
}
