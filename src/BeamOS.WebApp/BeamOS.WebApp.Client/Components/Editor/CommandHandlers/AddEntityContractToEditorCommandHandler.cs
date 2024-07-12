using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Common.Api;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOS.WebApp.Client.Components.Editor.Commands;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class AddEntityContractToEditorCommandHandler(
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    HistoryManager historyManager
) : CommandHandlerBase<AddEntityToEditorCommand>(historyManager)
{
    protected override async Task<Result> ExecuteCommandAsync(
        AddEntityToEditorCommand command,
        CancellationToken ct = default
    )
    {
        IEditorApiAlpha? editorApi =
            editorComponentStateRepository.GetComponentStateByCanvasId(command.CanvasId)?.EditorApi
            ?? throw new Exception(
                $"Editor api does not exist for canvas with id {command.CanvasId}"
            );

        switch (command.Entity)
        {
            case Element1DResponse element1D:
                await editorApi.CreateElement1dAsync(element1D, CancellationToken.None);
                break;
            case NodeResponse nodeResponse:
                await editorApi.CreateNodeAsync(nodeResponse, CancellationToken.None);
                break;
            case PointLoadResponse pointLoadResponse:
                await editorApi.CreatePointLoadAsync(pointLoadResponse, CancellationToken.None);
                break;
            default:
                throw new Exception($"Unsupported type {command.Entity.GetType()}");
        }

        return Result.Success();
    }
}
