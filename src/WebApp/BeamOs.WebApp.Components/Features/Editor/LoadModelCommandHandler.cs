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
    CacheModelResponseCommandHandler cacheModelResponseCommandHandler,
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
            return modelResponse.Error.ToBeamOsError();
        }

        await loadBeamOsEntityCommandHandler.ExecuteAsync(
            new LoadModelResponseHydratedCommand(
                modelResponse.Value,
                editorComponentState.Value.EditorApi
            ),
            ct
        );

        CacheModelCommand cacheModelCommand = new(command.CanvasId, modelResponse.Value);

        return await cacheModelResponseCommandHandler.ExecuteAsync(cacheModelCommand, ct);
    }

    protected override void PostProcess(
        LoadModelCommand command,
        Result<CachedModelResponse> response
    )
    {
        dispatcher.Dispatch(new EditorLoadingEnd(command.CanvasId));
    }
}

public sealed class CacheModelResponseCommandHandler(
    ISnackbar snackbar,
    IDispatcher dispatcher,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClient,
    ILogger<CacheModelResponseCommandHandler> logger
) : CommandHandlerBase<CacheModelCommand, CachedModelResponse>(snackbar, logger)
{
    protected override async Task<Result<CachedModelResponse>> ExecuteCommandAsync(
        CacheModelCommand command,
        CancellationToken ct = default
    )
    {
        var cachedModelResponse = new CachedModelResponse(command.ModelResponse);
        dispatcher.Dispatch(new ModelLoaded(cachedModelResponse));

        if (
            command.ModelResponse.ResultSets is not null
            && command.ModelResponse.ResultSets.Count > 0
        )
        {
            dispatcher.Dispatch(new EditorLoadingBegin(command.CanvasId, "Fetching Results"));
            var resultId = command.ModelResponse.ResultSets[0].Id;
            var diagrams = await structuralAnalysisApiClient.GetDiagramsAsync(
                command.ModelResponse.Id,
                resultId,
                "kn-m",
                ct
            );
            if (diagrams.IsError)
            {
                this.Snackbar.Add(diagrams.Error.Detail, Severity.Error);
                return diagrams.Error.ToBeamOsError();
            }
            else
            {
                dispatcher.Dispatch(
                    new AnalyticalResultsCreated() { AnalyticalResults = diagrams.Value }
                );
            }
        }
        return cachedModelResponse;
    }
}

public record struct LoadModelCommand(string CanvasId, Guid ModelId);

public readonly record struct CacheModelCommand(string CanvasId, ModelResponse ModelResponse);

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
        return command.EntityResponse switch
        {
            ModelResponse modelResponse => await DisplayModelResponse(
                modelResponse,
                command.EditorApi,
                ct
            ),
            ModelProposalResponse modelProposalResponse => await DisplayModelProposal(
                modelProposalResponse,
                command.EditorApi,
                ct
            ),
            _ => BeamOsError.Failure(
                description: $"Unsupported entity type: {command.EntityResponse.GetType()}"
            ),
        };
    }

    private static async Task<Result<CachedModelResponse>> DisplayModelResponse(
        ModelResponse modelResponse,
        IEditorApiAlpha editorApi,
        CancellationToken ct
    )
    {
        await editorApi.ClearAsync(ct);

        await editorApi.SetSettingsAsync(modelResponse.Settings, ct);

        await editorApi.CreateModelAsync(modelResponse, ct);

        // await editorApi.CreateNodesAsync(modelResponse.Nodes.Select(e => e.ToEditorUnits()), ct);

        // await editorApi.CreatePointLoadsAsync(
        //     modelResponse.PointLoads.Select(e => e.ToEditorUnits()),
        //     ct
        // );

        // await editorApi.CreateElement1dsAsync(modelResponse.Element1ds, ct);

        return new CachedModelResponse(modelResponse);
    }

    private static async Task<Result<CachedModelResponse>> DisplayModelProposal(
        ModelProposalResponse entity,
        IEditorApiAlpha editorApi,
        CancellationToken ct
    )
    {
        await editorApi.DisplayModelProposalAsync(entity, ct);
        CachedModelResponse cachedModelResponse = default;

        return cachedModelResponse;
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
