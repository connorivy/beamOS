using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Common.Api;
using BeamOs.Contracts.AnalyticalResults.Diagrams;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOS.WebApp.Client.Components.Editor.Commands;
using BeamOS.WebApp.Client.Extensions;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class AddEntityContractToEditorCommandHandler(
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    ChangeComponentStateCommandHandler<EditorComponentState> changeComponentStateCommandHandler,
    AddEntityContractToCacheCommandHandler addEntityContractToCacheCommandHandler,
    HistoryManager historyManager
) : CommandHandlerBase<AddEntityToEditorCommand>(historyManager)
{
    protected override async Task<Result> ExecuteCommandAsync(
        AddEntityToEditorCommand command,
        CancellationToken ct = default
    )
    {
        var editorComponentState =
            editorComponentStateRepository.GetComponentStateByCanvasId(command.CanvasId)
            ?? throw new Exception(
                $"Could not find editor component corrosponding to canvasId, {command.CanvasId}"
            );

        IEditorApiAlpha? editorApi =
            editorComponentState.EditorApi
            ?? throw new Exception(
                $"Editor api does not exist for canvas with id {command.CanvasId}"
            );

        await this.LoadSingleComponent(
            command.Entity,
            editorApi,
            command.CanvasId,
            editorComponentState.LoadedModelId
        );

        return Result.Success();
    }

    private async Task LoadSingleComponent(
        BeamOsEntityContractBase entity,
        IEditorApiAlpha editorApi,
        string canvasId,
        string? modelId
    )
    {
        switch (entity)
        {
            case ModelResponse modelResponse:
                await this.LoadModel(modelResponse, editorApi, canvasId);
                break;
            case Element1DResponse element1D:
                await editorApi.CreateElement1dAsync(element1D, CancellationToken.None);
                break;
            case NodeResponse nodeResponse:
                await editorApi.CreateNodeAsync(nodeResponse.InMeters(), CancellationToken.None);
                break;
            case PointLoadResponse pointLoadResponse:
                await editorApi.CreatePointLoadAsync(pointLoadResponse, CancellationToken.None);
                break;
            case ShearDiagramResponse shearDiagramResponse:
                await editorApi.CreateShearDiagramAsync(
                    shearDiagramResponse,
                    CancellationToken.None
                );
                break;
            case MomentDiagramResponse momentDiagramResponse:
                await editorApi.CreateMomentDiagramAsync(
                    momentDiagramResponse,
                    CancellationToken.None
                );
                break;
            default:
                throw new Exception($"Unsupported type {entity.GetType()}");
        }

        if (modelId is not null)
        {
            await addEntityContractToCacheCommandHandler.ExecuteAsync(new(modelId, entity));
        }
    }

    private async Task LoadModel(
        ModelResponse modelResponse,
        IEditorApiAlpha editorApi,
        string canvasId
    )
    {
        await editorApi.ClearAsync();
        await changeComponentStateCommandHandler.ExecuteAsync(
            new(canvasId, state => state with { LoadedModelId = modelResponse.Id, }),
            CancellationToken.None
        );

        foreach (var node in modelResponse.Nodes)
        {
            await this.LoadSingleComponent(node, editorApi, canvasId, modelResponse.Id);
        }

        foreach (var el in modelResponse.Element1ds)
        {
            await this.LoadSingleComponent(el, editorApi, canvasId, modelResponse.Id);
        }

        foreach (var el in modelResponse.PointLoads)
        {
            await this.LoadSingleComponent(el, editorApi, canvasId, modelResponse.Id);
        }
    }
}
