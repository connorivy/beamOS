using BeamOs.ApiClient;
using BeamOs.Common.Api;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOS.WebApp.Client.Components.Editor.Commands;
using BeamOS.WebApp.Client.Components.Editor.Flux.Events;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor.CommandHandlers;

public class LoadModelCommandHandler(
    IApiAlphaClient apiAlphaClient,
    HistoryManager historyManager,
    AddNodeCommandHandler addNodeCommandHandler,
    AddElement1dCommandHandler addElement1dCommandHandler,
    ModelIdRepository modelIdRepository
) : CommandHandlerBase<LoadModelCommand>(historyManager)
{
    protected override async Task<Result> ExecuteCommandAsync(
        LoadModelCommand command,
        CancellationToken ct = default
    )
    {
        ModelResponseHydrated response = await apiAlphaClient.GetModelHydratedAsync(
            command.ModelId,
            PreconfiguredUnits.N_M,
            ct
        );

        modelIdRepository.SetModelIdForCanvasId(command.CanvasId, command.ModelId);

        foreach (var node in response.Nodes)
        {
            await addNodeCommandHandler.ExecuteAsync(
                new AddNodeAction(command.CanvasId, node),
                CancellationToken.None
            );
        }

        foreach (var el in response.Element1Ds)
        {
            await addElement1dCommandHandler.ExecuteAsync(
                new AddElement1dAction(command.CanvasId, el),
                CancellationToken.None
            );
        }

        return Result.Success();
    }
}
