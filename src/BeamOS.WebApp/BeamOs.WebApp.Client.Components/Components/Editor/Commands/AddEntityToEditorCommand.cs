using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOs.WebApp.Client.Components.Components.Editor.Commands;

public record struct AddEntityToEditorCommand(string CanvasId, BeamOsEntityContractBase Entity)
    : IClientCommand;
