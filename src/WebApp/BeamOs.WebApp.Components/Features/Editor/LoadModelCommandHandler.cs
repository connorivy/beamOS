using BeamOs.CodeGen.EditorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.Tests.Common;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.TestExplorer;
using Fluxor;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.Editor;

public class LoadModelCommandHandler(
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClient,
    //IState<AllEditorComponentState> allEditorComponentState,
    IState<EditorComponentState> editorComponentState,
    IDispatcher dispatcher,
    HybridCache cache,
    LoadBeamOsEntityCommandHandler loadBeamOsEntityCommandHandler,
    ILogger<LoadModelCommandHandler> logger
) : CommandHandlerBase<LoadModelCommand, CachedModelResponse>(snackbar, logger)
{
    protected override async Task<Result<CachedModelResponse>> ExecuteCommandAsync(
        LoadModelCommand command,
        CancellationToken ct = default
    )
    {
        //if (
        //    !allEditorComponentState
        //        .Value
        //        .EditorState
        //        .TryGetValue(command.CanvasId, out var editorComponentState)
        //)
        //{
        //    return BeamOsError.NotFound(
        //        description: $"could not find canvas with Id = {command.CanvasId}"
        //    );
        //}

        if (editorComponentState.Value.EditorApi is null)
        {
            return BeamOsError.Failure(
                description: "Failed to load model because canvas editor api is null"
            );
        }

        var modelResponse = await structuralAnalysisApiClient.GetModelAsync(command.ModelId, ct);

        //var modelCacheResponse = await cache.GetOrCreateAsync<Result<CachedModelResponse>>(
        //    command.ModelId.ToString(),
        //    async ct =>
        //    {
        //        var modelResponse = await structuralAnalysisApiClient.GetModelAsync(
        //            command.ModelId,
        //            ct
        //        );
        //        if (modelResponse.IsError)
        //        {
        //            return modelResponse.Error;
        //        }
        //        else
        //        {
        //            return new CachedModelResponse(modelResponse.Value);
        //        }
        //    },
        //    cancellationToken: ct
        //);

        if (modelResponse.IsError)
        {
            return modelResponse.Error;
        }

        await loadBeamOsEntityCommandHandler.ExecuteAsync(
            new LoadModelResponseHydratedCommand(
                modelResponse.Value,
                editorComponentState.Value.EditorApi
            ),
            ct
        );

        var modelCacheResponse = new CachedModelResponse(modelResponse.Value);
        dispatcher.Dispatch(new ModelLoaded(modelCacheResponse));

        if (modelResponse.Value.ResultSets is not null && modelResponse.Value.ResultSets.Count > 0)
        {
            dispatcher.Dispatch(new EditorLoadingBegin(command.CanvasId, "Fetching Results"));
            var resultId = modelResponse.Value.ResultSets[0].Id;
            var diagrams = await structuralAnalysisApiClient.GetDiagramsAsync(
                command.ModelId,
                resultId,
                "kn-m",
                ct
            );
            if (diagrams.IsError)
            {
                this.Snackbar.Add(diagrams.Error.Description, Severity.Error);
                return diagrams.Error;
            }
            else
            {
                dispatcher.Dispatch(
                    new AnalyticalResultsCreated() { AnalyticalResults = diagrams.Value }
                );
            }
        }

        return modelCacheResponse;
    }

    protected override void PostProcess(
        LoadModelCommand command,
        Result<CachedModelResponse> response
    )
    {
        dispatcher.Dispatch(new EditorLoadingEnd(command.CanvasId));
    }
}

public record struct LoadModelCommand(string CanvasId, Guid ModelId);

public record struct LoadEntityCommand(
    IBeamOsEntityResponse EntityResponse,
    IEditorApiAlpha EditorApi
) : ILoadEntityResponseCommand;

public record struct LoadModelResponseCommand(
    ModelResponse EntityResponse,
    IEditorApiAlpha EditorApi
) : ILoadEntityResponseCommand
{
    IBeamOsEntityResponse ILoadEntityResponseCommand.EntityResponse => this.EntityResponse;
}

public record struct LoadModelResponseHydratedCommand(
    ModelResponse EntityResponse,
    IEditorApiAlpha EditorApi
) : ILoadEntityResponseCommand
{
    IBeamOsEntityResponse ILoadEntityResponseCommand.EntityResponse => this.EntityResponse;
}

public interface ILoadEntityResponseCommand
{
    public IBeamOsEntityResponse EntityResponse { get; }
    public IEditorApiAlpha EditorApi { get; }
}

public class LoadBeamOsEntityCommandHandler(
    ISnackbar snackbar,
    ILogger<LoadBeamOsEntityCommandHandler> logger
) : CommandHandlerBase<ILoadEntityResponseCommand, CachedModelResponse>(snackbar, logger)
{
    protected override async Task<Result<CachedModelResponse>> ExecuteCommandAsync(
        ILoadEntityResponseCommand command,
        CancellationToken ct = default
    )
    {
        if (command.EntityResponse is ModelResponse modelResponse)
        {
            await command.EditorApi.ClearAsync(ct);

            await command.EditorApi.SetSettingsAsync(modelResponse.Settings, ct);

            await command.EditorApi.CreateNodesAsync(
                modelResponse.Nodes.Select(e => e.ToEditorUnits()),
                ct
            );

            await command.EditorApi.CreatePointLoadsAsync(
                modelResponse.PointLoads.Select(e => e.ToEditorUnits()),
                ct
            );

            await command.EditorApi.CreateElement1dsAsync(modelResponse.Element1ds, ct);

            return new CachedModelResponse(modelResponse);
        }
        // else if (command.EntityResponse is ModelResponseHydrated modelResponseH)
        // {
        //     await command.EditorApi.ClearAsync(ct);

        //     await command.EditorApi.SetSettingsAsync(modelResponseH.Settings, ct);

        //     await command.EditorApi.CreateNodesAsync(
        //         modelResponseH.Nodes.Select(e => e.ToEditorUnits()),
        //         ct
        //     );

        //     await command.EditorApi.CreatePointLoadsAsync(
        //         modelResponseH.PointLoads.Select(e => e.ToEditorUnits()),
        //         ct
        //     );

        //     await command.EditorApi.CreateElement1dsAsync(modelResponseH.Element1ds, ct);

        //     return new CachedModelResponse(modelResponseH);
        // }

        throw new NotImplementedException(
            $"Type {command.EntityResponse.GetType()} is not supported in '{nameof(LoadBeamOsEntityCommandHandler)}'"
        );
    }
}

public record struct RunTestCommand(IEnumerable<TestInfo> TestInfos);

public class RunTestCommandHandler(
    ISnackbar snackbar,
    IDispatcher dispatcher,
    IServiceProvider serviceProvider,
    ILogger<RunTestCommandHandler> logger
) : CommandHandlerBase<RunTestCommand, bool>(snackbar, logger)
{
    protected override async Task<Result<bool>> ExecuteCommandAsync(
        RunTestCommand command,
        CancellationToken ct = default
    )
    {
        void OnAssertedEqual2(object? _, TestResult args) =>
            dispatcher.Dispatch(new TestResultComputed(args));

        foreach (var test in command.TestInfos ?? [])
        {
            test.OnTestResult += OnAssertedEqual2;
            try
            {
                await test.RunTest(serviceProvider);
            }
            finally
            {
                test.OnTestResult -= OnAssertedEqual2;
            }
        }

        return true;
    }
}

public record struct RunTestsInFrontEndCommand(params IEnumerable<string> testResultIds);

public class RunTestsInFrontEndCommandHandler(
    ISnackbar snackbar,
    IDispatcher dispatcher,
    IState<TestInfoState> testInfoState,
    ILogger<RunTestsInFrontEndCommandHandler> logger
) : CommandHandlerBase<RunTestsInFrontEndCommand, bool>(snackbar, logger)
{
    protected override async Task<Result<bool>> ExecuteCommandAsync(
        RunTestsInFrontEndCommand command,
        CancellationToken ct = default
    )
    {
        List<string> notStartedTestResultIds = [];
        foreach (var resultId in command.testResultIds)
        {
            SingleTestState testResult = testInfoState.Value.TestInfoIdToTestResultDict[resultId];
            if (testResult.FrontEndProgressStatus == TestProgressStatus.NotStarted)
            {
                notStartedTestResultIds.Add(resultId);
                dispatcher.Dispatch(
                    new TestResultProgressChanged(resultId, TestProgressStatus.InProgress)
                );
            }
        }

        await foreach (
            var resultId in Task.WhenEach(notStartedTestResultIds.Select(GiveTestsTimeToRun))
        )
        {
            dispatcher.Dispatch(
                new TestResultProgressChanged(await resultId, TestProgressStatus.Finished)
            );
        }

        return true;
    }

    public static async Task<string> GiveTestsTimeToRun(string resultId)
    {
        var x = Convert.ToInt32(1000 * Random.Shared.NextDouble());
        await Task.Delay(x);

        return resultId;
    }
}
