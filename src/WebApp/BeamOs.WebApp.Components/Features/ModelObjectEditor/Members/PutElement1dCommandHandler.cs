using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Riok.Mapperly.Abstractions;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Members;

public sealed class PutElement1dCommandHandler(
    ILogger<PutElement1dCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : ClientCommandHandlerBase<PutElement1dClientCommand, Element1dResponse>(logger, snackbar)
{
    protected override async ValueTask<Result> UpdateEditor(PutElement1dClientCommand command)
    {
        var editorApi =
            editorState.Value.EditorApi
            ?? throw new InvalidOperationException("Editor API is not initialized");

        return await editorApi.UpdateElement1dAsync(
            new Element1dResponse(
                command.Element1dId,
                command.ModelId,
                command.New.StartNodeId,
                command.New.EndNodeId,
                command.New.MaterialId,
                command.New.SectionProfileId,
                command.New.SectionProfileRotation
            )
        );
    }

    protected override async ValueTask<Result<Element1dResponse>> UpdateServer(
        PutElement1dClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.PutElement1dAsync(
            command.ModelId,
            command.Element1dId,
            command.New.ToElement1dData(),
            ct
        );
    }

    protected override async ValueTask<Result> UpdateEditorAfterServerResponse(
        PutElement1dClientCommand command,
        Result<Element1dResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            var editorApi =
                editorState.Value.EditorApi
                ?? throw new InvalidOperationException("Editor API is not initialized");

            return await editorApi.UpdateElement1dAsync(serverResponse.Value);
        }

        return Result.Success;
    }

    protected override ValueTask<Result> UpdateClient(
        PutElement1dClientCommand command,
        Result<Element1dResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            dispatcher.Dispatch(command);
        }

        return new(Result.Success);
    }
}

public sealed class PutElement1dSimpleCommandHandler(
    PutElement1dCommandHandler putElement1dCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<
        ModelResourceWithIntIdRequest<Element1dData>,
        PutElement1dClientCommand,
        Element1dResponse
    >(putElement1dCommandHandler)
{
    protected override PutElement1dClientCommand CreateCommand(
        ModelResourceWithIntIdRequest<Element1dData> simpleCommand
    )
    {
        var element1d =
            editorState.Value.CachedModelResponse?.Element1ds.GetValueOrDefault(simpleCommand.Id)
            ?? throw new InvalidOperationException("Element1d not found in editor state");

        return new()
        {
            ModelId = simpleCommand.ModelId,
            Element1dId = simpleCommand.Id,
            Previous = element1d,
            New = simpleCommand.ToResponse(),
        };
    }
}

[Mapper]
internal static partial class Element1dMappers
{
    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial Element1dResponse ToResponse(
        this ModelResourceWithIntIdRequest<Element1dData> command
    );
}

public record PutElement1dClientCommand : IBeamOsUndoableClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
    public required Guid ModelId { get; init; }
    public int Element1dId { get; init; }
    public required Element1dResponse Previous { get; init; }
    public required Element1dResponse New { get; init; }

    public IBeamOsUndoableClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new PutElement1dClientCommand
        {
            ModelId = this.ModelId,
            Element1dId = this.Element1dId,
            Previous = this.New,
            New = this.Previous,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };

    public IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };
}
