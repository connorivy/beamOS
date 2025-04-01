using BeamOs.CodeGen.SpeckleConnectorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.AnalysisToolbar;

public sealed class ReceiveFromSpeckleCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    ISpeckleConnectorApi speckleConnectorApi,
    IDispatcher dispatcher,
    ISnackbar snackbar
) : CommandHandlerBase<ReceiveFromSpeckleCommand, CachedModelResponse>(snackbar)
{
    protected override async Task<Result<CachedModelResponse>> ExecuteCommandAsync(
        ReceiveFromSpeckleCommand command,
        CancellationToken ct = default
    )
    {
        dispatcher.Dispatch(
            new EditorLoadingBegin(command.CanvasId, "Receiving Data From Spackle")
        );

        var beamOsModelBuilderDto = await speckleConnectorApi.ConvertToBeamOsAsync(
            command.SpeckleReceiveParameters,
            ct
        );

        if (beamOsModelBuilderDto.IsError)
        {
            return beamOsModelBuilderDto.Error;
        }

        dispatcher.Dispatch(
            new EditorLoadingBegin(command.CanvasId, "Saving Changes To BeamOS Model")
        );

        BeamOsDynamicModelBuilder builder =
            new(
                command.ModelId.ToString(),
                new(UnitSettingsContract.K_IN),
                "na",
                "na",
                beamOsModelBuilderDto.Value
            );

        await builder.CreateOrUpdate(structuralAnalysisApiClientV1);

        var modelResponse = await structuralAnalysisApiClientV1.GetModelAsync(command.ModelId, ct);

        if (modelResponse.IsError)
        {
            return modelResponse.Error;
        }

        var result = new CachedModelResponse(modelResponse.Value);
        dispatcher.Dispatch(new ModelLoaded() { CachedModelResponse = result });

        return result;
    }

    protected override void PostProcess(
        ReceiveFromSpeckleCommand command,
        Result<CachedModelResponse> result
    )
    {
        dispatcher.Dispatch(new EditorLoadingEnd() { CanvasId = command.CanvasId });
    }
}


public readonly record struct ReceiveFromSpeckleCommand(
    string CanvasId,
    Guid ModelId,
    SpeckleReceiveParameters SpeckleReceiveParameters
);
