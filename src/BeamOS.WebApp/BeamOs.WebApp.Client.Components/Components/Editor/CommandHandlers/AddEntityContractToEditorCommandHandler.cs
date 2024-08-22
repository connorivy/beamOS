using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Common.Api;
using BeamOs.Contracts.AnalyticalResults.Diagrams;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.WebApp.Client.Components.Components.Editor.Commands;
using BeamOs.WebApp.Client.Components.Extensions;
using BeamOs.WebApp.Client.Components.Repositories;

namespace BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;

public class AddEntityContractToEditorCommandHandler(
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    AddEntityContractToCacheCommandHandler addEntityContractToCacheCommandHandler,
    LoadModelCommandHandler loadModelCommandHandler
) : CommandHandlerBase<AddEntityToEditorCommand>
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
                await loadModelCommandHandler.ExecuteAsync(new(canvasId, modelResponse));
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
}

public abstract class AddEntityContractsToEditorCommandHandlerBase<TEntity>(
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    AddEntityContractToCacheCommandHandler addEntityContractToCacheCommandHandler
) : CommandHandlerBase<AddEntitiesToEditorCommand<TEntity>>
    where TEntity : BeamOsEntityContractBase
{
    protected override async Task<Result> ExecuteCommandAsync(
        AddEntitiesToEditorCommand<TEntity> command,
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

        await this.LoadEntities(command.Entities, editorApi);

        foreach (var entity in command.Entities)
        {
            await addEntityContractToCacheCommandHandler.ExecuteAsync(
                new(editorComponentState.LoadedModelId, entity)
            );
        }

        return Result.Success();
    }

    protected abstract Task LoadEntities(
        IEnumerable<TEntity> entities,
        IEditorApiAlpha editorApiAlpha
    );
}

public class AddNodesToEditorCommandHandler(
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    AddEntityContractToCacheCommandHandler addEntityContractToCacheCommandHandler
)
    : AddEntityContractsToEditorCommandHandlerBase<NodeResponse>(
        editorComponentStateRepository,
        addEntityContractToCacheCommandHandler
    )
{
    protected override async Task LoadEntities(
        IEnumerable<NodeResponse> entities,
        IEditorApiAlpha editorApiAlpha
    )
    {
        await editorApiAlpha.CreateNodesAsync(entities.Select(e => e.InMeters()));
    }
}

public class AddElement1dsToEditorCommandHandler(
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    AddEntityContractToCacheCommandHandler addEntityContractToCacheCommandHandler
)
    : AddEntityContractsToEditorCommandHandlerBase<Element1DResponse>(
        editorComponentStateRepository,
        addEntityContractToCacheCommandHandler
    )
{
    protected override async Task LoadEntities(
        IEnumerable<Element1DResponse> entities,
        IEditorApiAlpha editorApiAlpha
    )
    {
        await editorApiAlpha.CreateElement1dsAsync(entities);
    }
}

public class AddShearDiagramsToEditorCommandHandler(
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    AddEntityContractToCacheCommandHandler addEntityContractToCacheCommandHandler
)
    : AddEntityContractsToEditorCommandHandlerBase<ShearDiagramResponse>(
        editorComponentStateRepository,
        addEntityContractToCacheCommandHandler
    )
{
    protected override async Task LoadEntities(
        IEnumerable<ShearDiagramResponse> entities,
        IEditorApiAlpha editorApiAlpha
    )
    {
        await editorApiAlpha.CreateShearDiagramsAsync(entities);
    }
}

public class AddMomentDiagramsToEditorCommandHandler(
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    AddEntityContractToCacheCommandHandler addEntityContractToCacheCommandHandler
)
    : AddEntityContractsToEditorCommandHandlerBase<MomentDiagramResponse>(
        editorComponentStateRepository,
        addEntityContractToCacheCommandHandler
    )
{
    protected override async Task LoadEntities(
        IEnumerable<MomentDiagramResponse> entities,
        IEditorApiAlpha editorApiAlpha
    )
    {
        await editorApiAlpha.CreateMomentDiagramsAsync(entities);
    }
}

public class AddPointLoadsToEditorCommandHandler(
    IStateRepository<EditorComponentState> editorComponentStateRepository,
    AddEntityContractToCacheCommandHandler addEntityContractToCacheCommandHandler
)
    : AddEntityContractsToEditorCommandHandlerBase<PointLoadResponse>(
        editorComponentStateRepository,
        addEntityContractToCacheCommandHandler
    )
{
    protected override async Task LoadEntities(
        IEnumerable<PointLoadResponse> entities,
        IEditorApiAlpha editorApiAlpha
    )
    {
        await editorApiAlpha.CreatePointLoadsAsync(entities);
    }
}
