using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOs.WebApp.Client.Components.Components.Editor.Commands;

public record struct AddEntityToEditorCommand(string CanvasId, BeamOsEntityContractBase Entity)
    : IClientCommand;

public record struct AddModelToEditorCommand(string CanvasId, ModelResponse ModelResponse)
    : IClientCommand;

public record struct AddEntitiesToEditorCommand<TEntity>(
    string CanvasId,
    IEnumerable<TEntity> Entities
) : IClientCommand
    where TEntity : BeamOsEntityContractBase;
