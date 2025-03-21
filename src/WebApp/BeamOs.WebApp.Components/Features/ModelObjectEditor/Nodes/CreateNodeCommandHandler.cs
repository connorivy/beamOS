using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.EditorCommands;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.Nodes;

public sealed class CreateNodeCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    ISnackbar snackbar
) : CommandHandlerBase<CreateNodeCommand, NodeResponse>(snackbar)
{
    protected override async Task<Result<NodeResponse>> ExecuteCommandAsync(
        CreateNodeCommand command,
        CancellationToken ct = default
    )
    {
        var request = command.ToRequest();
        return await structuralAnalysisApiClientV1.CreateNodeAsync(command.ModelId, request, ct);
    }

    protected override void PostProcess(CreateNodeCommand command, Result<NodeResponse> response)
    {
        if (response.IsSuccess)
        {
            dispatcher.Dispatch(
                new CreateNodeClientCommand(response.Value) { HandledByServer = true }
            );
        }
    }
}

public record CreateNodeClientCommand(NodeResponse New) : IBeamOsClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new DeleteNodeClientCommand(this.New)
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer
        };

    public IBeamOsClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer
        };
}

public record DeleteNodeClientCommand(NodeResponse Previous) : IBeamOsClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new CreateNodeClientCommand(this.Previous)
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer
        };

    public IBeamOsClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer
        };
}
