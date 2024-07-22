using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOS.WebApp.Client.Components.Editor.Commands;

public record struct AddEntityToEditorCommand(string CanvasId, BeamOsEntityContractBase Entity)
    : IClientCommand;
